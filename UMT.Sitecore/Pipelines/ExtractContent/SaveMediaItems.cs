using System;
using System.Collections.Generic;
using System.IO;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
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
            UMTLog.Info($"Saving media library JSON file...", true);

            try
            {
                var targetMediaLibrary = GetTargetMediaLibrary(args.SourceMediaLibrary);
                SaveSerializedMediaLibrary(targetMediaLibrary, args.OutputFolderPath);
                UMTLog.Info($"Media library {targetMediaLibrary.LibraryName} ({targetMediaLibrary.LibraryGUID}) saved", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving media library, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"Saving media items JSON files...", true);

            try
            {
                var targetMediaItems = GetTargetMediaItems(args.SourceMediaItems, args.SourceMediaLibrary, args.OutputFolderPath);
                UMTLog.Info($"{targetMediaItems.Count} media items mapped and saved");
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving media items, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(SaveMediaItems)} pipeline processor finished");
        }

        protected virtual MediaLibrary GetTargetMediaLibrary(MediaMap sourceMediaLibrary)
        {
            var mediaLibrary = new MediaLibrary
            {
                LibraryGUID = sourceMediaLibrary.Id,
                LibraryName = sourceMediaLibrary.Name,
                LibraryDisplayName = sourceMediaLibrary.DisplayName,
                LibraryDescription = sourceMediaLibrary.Description,
                LibraryFolder = sourceMediaLibrary.LibraryFolder
            };

            return mediaLibrary;
        }

        protected virtual List<MediaFile> GetTargetMediaItems(IList<MediaItem> items, MediaMap sourceMediaLibrary, string outputFolderPath)
        {
            var fileExtractFolder = CreateFileExtractFolder(UMTSettings.MediaLocationForExport.Replace("{outputFolder}", outputFolderPath));  
            var mappedItems = new List<MediaFile>();

            foreach (var item in items)
            {
                var mappedItem = MapToTargetItem(item, sourceMediaLibrary, fileExtractFolder);
                mappedItems.Add(mappedItem);
                SaveSerializedMediaItem(mappedItem, outputFolderPath);
                UMTJob.IncreaseProcessedItems();
            }

            return mappedItems;
        }

        protected virtual MediaFile MapToTargetItem(MediaItem mediaItem, MediaMap sourceMediaLibrary, string folderPath)
        {
            int.TryParse(mediaItem.InnerItem["Width"], out var width);
            int.TryParse(mediaItem.InnerItem["Height"], out var height);
            var fileExtension = $".{mediaItem.Extension}";
            var fileName = mediaItem.Name.EndsWith(fileExtension)
                ? mediaItem.Name
                : $"{mediaItem.Name}.{mediaItem.Extension}";
            var targetItem = new MediaFile
            {
                FileGUID = mediaItem.ID.Guid,
                FileDescription = mediaItem.Description,
                FileExtension = fileExtension,
                FileName = mediaItem.Name,
                FilePath = $"{mediaItem.MediaPath.Trim('/')}{fileExtension}",
                FileTitle = !string.IsNullOrEmpty(mediaItem.Title) ? mediaItem.Title : mediaItem.Alt,
                FileMimeType = mediaItem.MimeType,
                FileImageHeight = height,
                FileImageWidth = width,
                FileCreatedWhen = mediaItem.InnerItem.Statistics.Created,
                FileModifiedWhen = mediaItem.InnerItem.Statistics.Updated,
                FileLibraryGuid = sourceMediaLibrary.Id
            };

            if (UMTSettings.ExportMediaAsUrls)
            {
                var siteContext = Factory.GetSite(UMTSettings.ExportMediaAsUrlsSiteName);
                using (new SiteContextSwitcher(siteContext))
                {
                    var mediaUrl = MediaManager.GetMediaUrl(mediaItem, new MediaUrlOptions
                    {
                        IncludeExtension = true,
                        AlwaysIncludeServerUrl = true,
                        MediaLinkServerUrl = UMTSettings.ExportMediaAsUrlsServerUrl
                    });
                    targetItem.DataSourceUrl = mediaUrl;
                }
            }
            else
            {
                var fileFolder = $"{folderPath}{mediaItem.MediaPath.Substring(0, mediaItem.MediaPath.LastIndexOf(mediaItem.Name))}".TrimEnd(' ', '/', '\\');
                var dataSourcePath = SaveFile(mediaItem, fileFolder, fileName);
                if (!string.IsNullOrEmpty(dataSourcePath))
                {
                    targetItem.DataSourcePath = string.IsNullOrEmpty(UMTSettings.MediaLocationForJson)
                        ? dataSourcePath
                        : dataSourcePath.Replace(folderPath, UMTSettings.MediaLocationForJson);
                }
                else
                {
                    UMTLog.Warn(
                        $"Media file {targetItem.FilePath} ({mediaItem.ID}) was not saved to the output folder.");
                }
            }

            return targetItem;
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
                
        protected virtual void SaveSerializedMediaLibrary(MediaLibrary mediaLibrary, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/01.Configuration");
            var fileName =  $"{folderPath}/03.MediaLibrary.json";
            SerializeToFile(new[] { mediaLibrary }, fileName);
        }

        protected virtual void SaveSerializedMediaItem(MediaFile mediaItem, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/03.Media");
            var fileName = $"{folderPath}/{mediaItem.FileName}.{mediaItem.FileGUID:D}.json";
            SerializeToFile(new[] { mediaItem }, fileName);
        }
    }
}