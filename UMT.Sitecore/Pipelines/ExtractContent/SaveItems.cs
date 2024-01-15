using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Jobs;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SaveItems : BaseSaveProcessor
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));
            Assert.ArgumentNotNull(args.SourceLanguages, nameof(args.SourceLanguages));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));

            UMTLog.Info($"{nameof(SaveItems)} pipeline processor started");
            UMTLog.Info($"Saving content items JSON files...", true);

            try
            {
                var targetItems = GetTargetItems(args.SourceItems, args.SourceLanguages, args.SourceChannel,
                    args.TargetTemplates, args.OutputFolderPath);
                UMTLog.Info($"{targetItems.Count} content items mapped and saved", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving content items, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(SaveItems)} pipeline processor finished");
        }

        protected virtual Dictionary<string, TargetItem> GetTargetItems(IList<Item> items, IList<Language> languages,
            ChannelMap channel, Dictionary<Guid, TargetContentType> templates, string outputFolderPath)
        {
            var mappedItems = new Dictionary<string, TargetItem>();

            foreach (var item in items)
            {
                if (templates.ContainsKey(item.TemplateID.Guid))
                {
                    var index = 0;
                    var path = item.Paths.ContentPath.ToValidPath();
                    if (mappedItems.ContainsKey(path))
                    {
                        index = 2;
                        while (mappedItems.ContainsKey($"{path}{index}"))
                        {
                            index++;
                        }
                        path = $"{path}{index}";
                    }

                    var mappedItem = MapToTargetItem(item, path, index, languages, channel, templates, mappedItems);
                    mappedItems.Add(path, mappedItem);
                    SaveSerializedItem(mappedItem, outputFolderPath);
                    UMTJob.IncreaseProcessedItems();
                }
                else
                {
                    UMTLog.ManualCheck(new UMTJobManualCheck
                    {
                        EntityType = EntityType.Item,
                        EntityId = item.ID.Guid,
                        EntityName = item.Name,
                        Message = "Item is skipped because its template does not exist"
                    });
                }
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(Item item, string contentPath, int duplicateIndex,
            IList<Language> languages, ChannelMap channel,
            Dictionary<Guid, TargetContentType> templates, Dictionary<string, TargetItem> mappedItems)
        {
            var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(item.TemplateID.Guid);
            var webPageItemId = item.ID.Guid.ToWebPageItemGuid();
            var itemName = item.Name.ToValidItemName();
            var shortItemName = itemName;
            if (shortItemName.Length > 67)
            {
                shortItemName = shortItemName.Substring(0, 67);
            }
            var codeName = $"{shortItemName}-{item.ID.Guid:N}"; // CodeName should be 100 characters or less
            var targetItem = new TargetItem
            {
                Id = item.ID.Guid,
                Name = itemName,
                DepthLevel = contentPath.Trim('/').Count(x => x == '/'),
                IsWebPage = !isContentHubItem
            };
            targetItem.Elements.Add(new ContentItem
            {
                ContentItemName = codeName,
                ContentItemChannelGuid = channel.Id,
                ContentItemGUID = item.ID.Guid,
                ContentItemDataClassGuid = item.TemplateID.Guid,
                ContentItemIsSecured = false,
                ContentItemIsReusable = isContentHubItem
            });

            if (!isContentHubItem)
            {
                targetItem.Elements.Add(new WebPageItem
                {
                    WebPageItemGUID = webPageItemId,
                    WebPageItemName = codeName,
                    WebPageItemContentItemGuid = item.ID.Guid,
                    WebPageItemParentGuid = GetParent(item, mappedItems),
                    WebPageItemWebsiteChannelGuid = channel.WebsiteId,
                    WebPageItemTreePath = contentPath,
                    WebPageItemOrder = item.Appearance.Sortorder
                    
                });
            }

            foreach (var language in languages)
            {
                var languageVersion = Factory.GetDatabase(UMTSettings.Database).GetItem(item.ID, language);

                if (languageVersion != null && !languageVersion.IsFallback)
                {
                    var languageId = UMTConfiguration.LanguageMapping.GetTargetLanguageId(language.Origin.ItemId.Guid);
                    
                    targetItem.Elements.Add(new ContentItemLanguageMetadata
                    {
                        ContentItemLanguageMetadataGUID = item.ID.Guid.ToContentItemLanguageMetadataGuid(language.Name),
                        ContentItemLanguageMetadataContentItemGuid = item.ID.Guid,
                        ContentItemLanguageMetadataContentLanguageGuid = languageId,
                        ContentItemLanguageMetadataDisplayName = languageVersion.DisplayName,
                        ContentItemLanguageMetadataCreatedWhen = languageVersion.Statistics.Created,
                        ContentItemLanguageMetadataModifiedWhen = languageVersion.Statistics.Updated,
                        ContentItemLanguageMetadataLatestVersionStatus = 2,
                        ContentItemLanguageMetadataHasImageAsset = false
                    });

                    var commonDataId = item.ID.Guid.ToContentItemCommonDataGuid(language.Name);
                    targetItem.Elements.Add(new ContentItemCommonData
                    {
                        ContentItemCommonDataGUID = commonDataId,
                        ContentItemCommonDataContentItemGuid = item.ID.Guid,
                        ContentItemCommonDataContentLanguageGuid = languageId,
                        ContentItemCommonDataVersionStatus = 2,
                        ContentItemCommonDataIsLatest = true
                    });

                    var itemTemplate = templates[item.TemplateID.Guid];
                    var targetItemFields = GetTargetItemFields(languageVersion, itemTemplate);
                    targetItem.Elements.Add(new ContentItemData
                    {
                        ContentItemDataGUID = item.ID.Guid.ToContentItemDataGuid(languageId),
                        ContentItemDataCommonDataGuid = commonDataId,
                        ContentItemContentTypeName = itemTemplate.ClassName,
                        Properties = targetItemFields.ToDictionary(k => k.Key, v => v.Value.Value)
                    });

                    foreach (var targetFieldValue in targetItemFields)
                    {
                        if (targetFieldValue.Value.References != null && targetFieldValue.Value.References.Count > 0)
                        {
                            targetItem.Elements.AddRange(targetFieldValue.Value.References);
                        }
                    }

                    if (!isContentHubItem && item.HasPresentationDetails())
                    {
                        var url = LinkManager.GetItemUrl(item, new UrlOptions { AlwaysIncludeServerUrl = false, Language = language }).TrimStart('/');
                        if (duplicateIndex > 0)
                        {
                            url = $"{url}{duplicateIndex}";
                        }
                        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                        {
                            url = uri.AbsolutePath;
                        }
                        targetItem.Elements.Add(new WebPageUrlPath
                        {
                            WebPageUrlPathGUID = item.ID.Guid.ToWebPageUrlPathGuid(languageId),
                            WebPageUrlPathPath = url,
                            WebPageUrlPathWebPageItemGuid = webPageItemId,
                            WebPageUrlPathWebsiteChannelGuid = channel.WebsiteId,
                            WebPageUrlPathContentLanguageGuid = languageId,
                            WebPageUrlPathIsDraft = false,
                            WebPageUrlPathIsLatest = true
                        });
                    }
                }
            }
            
            return targetItem;
        }

        protected virtual Dictionary<string, TargetFieldValue> GetTargetItemFields(Item item, TargetContentType itemTemplate)
        {
            var fields = new Dictionary<string, TargetFieldValue>();

            //this is required to read all field values, including Standard Values
            item.Fields.ReadAll();
            
            foreach (Field field in item.Fields)
            {
                if (!UMTConfiguration.FieldMapping.ShouldBeExcluded(field.ID.Guid))
                {
                    var fieldTypeMapper = UMTConfiguration.FieldTypeMapping.GetByFieldType(field.TypeKey);
                    if (fieldTypeMapper != null)
                    {
                        var fieldName = itemTemplate.ContentType.Fields.FirstOrDefault(x => x.Guid == field.ID.Guid)?.Column;
                        
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            var mappedValue = fieldTypeMapper.TypeConverter.Convert(field, item);
                            fields.Add(fieldName, mappedValue);
                        }
                        else
                        {
                            UMTLog.Warn($"Field {field.Name} ({field.ID}) of item {item.Name} ({item.ID}) has been skipped because " +
                                        $"it is not present in the content type definition.");
                            UMTLog.ManualCheck(new UMTJobManualCheck
                            {
                                EntityType = EntityType.Field,
                                EntityId = field.ID.Guid,
                                EntityName = field.Name,
                                Message = "Field is skipped because it is not present in the content type definition"
                            });
                        }
                    }
                }
            }
            
            return fields;
        }

        protected virtual Guid? GetParent(Item item, Dictionary<string, TargetItem> mappedItems)
        {
            var parent = item.Parent;

            while (parent != null && parent.ID != ItemIDs.ContentRoot && 
                   !mappedItems.ContainsKey(parent.Paths.ContentPath.ToValidPath()))
            {
                parent = parent.Parent;
            }
            
            return parent?.ID.Guid.ToWebPageItemGuid();
        }
        
        protected virtual void SaveSerializedItem(TargetItem item, string outputFolderPath)
        {
            var folderName = item.IsWebPage ? "05.WebPages" : "04.ContentItems";
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/{folderName}");
            var fileName = $"{folderPath}/{item.DepthLevel:0000}.{item.Name}.{item.Id:D}.json";
            SerializeToFile(item.Elements, fileName);
        }
    }
}