using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Jobs;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SaveMediaItems : BaseSaveProcessor
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceMediaItems, nameof(args.SourceMediaItems));

            UMTLog.Info($"{nameof(SaveMediaItems)} pipeline processor started");
            UMTLog.Info($"Saving media items JSON files...", true);

            try
            {
                var targetMediaItems = GetTargetMediaItems(args.SourceMediaItems, args.SourceLanguages, args.OutputFolderPath);
                UMTLog.Info($"{targetMediaItems.Count} media items mapped and saved");
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving media items, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(SaveMediaItems)} pipeline processor finished");
        }

        protected virtual List<TargetItem> GetTargetMediaItems(IList<MediaItem> items, IList<Language> languages, string outputFolderPath)
        {
            var fileExtractFolder = CreateFileExtractFolder(UMTSettings.MediaLocationForExport.Replace("{outputFolder}", outputFolderPath));  
            var mappedItems = new List<TargetItem>();

            foreach (var item in items)
            {
                var mappedItem = MapToTargetItem(item, languages, fileExtractFolder);
                if (mappedItem != null)
                {
                    mappedItems.Add(mappedItem);
                    SaveSerializedMediaItem(mappedItem, outputFolderPath);
                }

                UMTJob.IncreaseProcessedItems();
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(MediaItem mediaItem, IList<Language> languages, string folderPath)
        {
            var itemName = mediaItem.Name.ToValidItemName();
            var codeName = itemName.ToValidCodename(mediaItem.ID.Guid);
            var mediaTemplate = UMTConfiguration.MediaMapping.GetMediaTemplate(mediaItem.Extension);

            if (mediaTemplate == null)
            {
                UMTLog.ManualCheck(new UMTJobManualCheck
                {
                    EntityType = EntityType.Item,
                    EntityId = mediaItem.ID.Guid,
                    EntityName = mediaItem.Name,
                    Message = "Media item is skipped because there is no matching media template"
                });
                
                return null;
            }

            var targetItem = new TargetItem
            {
                Id = mediaItem.ID.Guid,
                Name = itemName,
                IsWebPage = false
            };
            
            targetItem.Elements.Add(new ContentItem
            {
                ContentItemName = codeName,
                ContentItemGUID = mediaItem.ID.Guid,
                ContentItemDataClassGuid = mediaTemplate.Id,
                ContentItemIsSecured = false,
                ContentItemIsReusable = true
            });
            
            foreach (var language in languages)
            {
                var languageVersion = Factory.GetDatabase(UMTSettings.Database).GetItem(mediaItem.ID, language);

                if (languageVersion != null && !languageVersion.IsFallback)
                {
                    var languageId = UMTConfiguration.LanguageMapping.GetTargetLanguageId(language.Origin.ItemId.Guid);
                    
                    targetItem.Elements.Add(new ContentItemLanguageMetadata
                    {
                        ContentItemLanguageMetadataGUID = mediaItem.ID.Guid.ToContentItemLanguageMetadataGuid(language.Name),
                        ContentItemLanguageMetadataContentItemGuid = mediaItem.ID.Guid,
                        ContentItemLanguageMetadataContentLanguageGuid = languageId,
                        ContentItemLanguageMetadataDisplayName = languageVersion.DisplayName,
                        ContentItemLanguageMetadataCreatedWhen = languageVersion.Statistics.Created,
                        ContentItemLanguageMetadataModifiedWhen = languageVersion.Statistics.Updated,
                        ContentItemLanguageMetadataLatestVersionStatus = 2,
                        ContentItemLanguageMetadataHasImageAsset = false
                    });

                    var commonDataId = mediaItem.ID.Guid.ToContentItemCommonDataGuid(language.Name);
                    targetItem.Elements.Add(new ContentItemCommonData
                    {
                        ContentItemCommonDataGUID = commonDataId,
                        ContentItemCommonDataContentItemGuid = mediaItem.ID.Guid,
                        ContentItemCommonDataContentLanguageGuid = languageId,
                        ContentItemCommonDataVersionStatus = 2,
                        ContentItemCommonDataIsLatest = true
                    });

                    var targetItemFields = GetMediaItemFields(languageVersion, mediaTemplate, folderPath);
                    targetItem.Elements.Add(new ContentItemData
                    {
                        ContentItemDataGUID = mediaItem.ID.Guid.ToContentItemDataGuid(languageId),
                        ContentItemDataCommonDataGuid = commonDataId,
                        ContentItemContentTypeName = mediaTemplate.Name.ToValidClassName(mediaTemplate.Namespace),
                        Properties = targetItemFields.ToDictionary(k => k.Key, v => v.Value.Value)
                    });
                }
            }

            return targetItem;
        }

        protected virtual Dictionary<string, TargetFieldValue> GetMediaItemFields(MediaItem languageVersion, MediaTemplate mediaTemplate, string folderPath)
        {
            var fields = new Dictionary<string, TargetFieldValue>();
            var mediaItem = new MediaItem(languageVersion);
            var fileExtension = $".{mediaItem.Extension}";
            var fileName = mediaItem.Name.EndsWith(fileExtension)
                ? mediaItem.Name
                : $"{mediaItem.Name}.{mediaItem.Extension}";
            int.TryParse(mediaItem.InnerItem["Size"], out var size);

            if (!string.IsNullOrEmpty(mediaTemplate.AltFieldName) && mediaTemplate.AltFieldId != Guid.Empty)
            {
                var fieldName = mediaTemplate.AltFieldName.ToValidFieldName();
                var fieldValue = mediaItem.Alt;
                fields.Add(fieldName, new TargetFieldValue{ Value = fieldValue});
            }
            
            if (!string.IsNullOrEmpty(mediaTemplate.AssetFieldName) && mediaTemplate.AssetFieldId != Guid.Empty)
            {
                var fieldName = mediaTemplate.AssetFieldName.ToValidFieldName();
                BaseAssetSource assetSource;
                
                if (UMTSettings.ExportMediaAsUrls)
                {
                    string mediaUrl;
                    var siteContext = Factory.GetSite(UMTSettings.ExportMediaAsUrlsSiteName);
                    using (new SiteContextSwitcher(siteContext))
                    {
                        mediaUrl = MediaManager.GetMediaUrl(mediaItem, new MediaUrlOptions
                        {
                            IncludeExtension = true,
                            AlwaysIncludeServerUrl = true,
                            MediaLinkServerUrl = UMTSettings.ExportMediaAsUrlsServerUrl
                        });
                    }

                    assetSource = new AssetUrlSource
                    {
                        Url = mediaUrl
                    };
                }
                else
                {
                    var fileFolder = $"{folderPath}{mediaItem.MediaPath.Substring(0, mediaItem.MediaPath.LastIndexOf(mediaItem.Name))}".TrimEnd(' ', '/', '\\');
                    var filePath = SaveFile(mediaItem, fileFolder, fileName);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        filePath = string.IsNullOrEmpty(UMTSettings.MediaLocationForJson)
                            ? filePath
                            : filePath.Replace(folderPath, UMTSettings.MediaLocationForJson);
                    }
                    else
                    {
                        UMTLog.Warn(
                            $"Media file {mediaItem.MediaPath} ({mediaItem.ID}) was not saved to the output folder.");
                    }

                    assetSource = new AssetFileSource
                    {
                        FilePath = filePath
                    };
                }
                
                assetSource.Identifier = mediaItem.ID.Guid.ToAssetSourceGuid();
                assetSource.Extension = fileExtension;
                assetSource.Name = fileName;
                assetSource.Size = size;
                assetSource.LastModified = mediaItem.InnerItem.Statistics.Updated;
                assetSource.ContentItemGuid = mediaItem.ID.Guid;
                
                var fieldValue = new TargetFieldValue
                {
                    Value = assetSource
                };
                fields.Add(fieldName, fieldValue);
            }

            return fields;
        }

        protected virtual string SaveFile(MediaItem mediaItem, string folderPath, string fileName)
        {
            using (var stream = mediaItem.GetMediaStream())
            {
                if (stream != null)
                {
                    if (UMTSettings.TrimLongMediaFolderPaths && folderPath.Length + fileName.Length > UMTSettings.MaxFilePathLength)
                    {
                        folderPath = folderPath.Substring(0, UMTSettings.MaxFilePathLength - fileName.Length).TrimEnd(' ', '/', '\\');
                    }

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    
                    var filePath = MainUtil.MapPath(MainUtil.MakeFilePath(folderPath, fileName));
                    using (var file = File.Create(filePath))
                    {
                        stream.CopyTo(file);
                    }

                    return filePath;
                }
            }

            return string.Empty;
        }

        protected virtual void SaveSerializedMediaItem(TargetItem targetMediaItem, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/03.Assets");
            var fileName = $"{folderPath}/{targetMediaItem.Name}.{targetMediaItem.Id:D}.json";
            SerializeToFile(targetMediaItem.Elements, fileName);
        }
    }
}