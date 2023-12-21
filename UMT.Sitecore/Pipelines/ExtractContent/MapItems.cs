using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MapItems
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));
            Assert.ArgumentNotNull(args.SourceLanguages, nameof(args.SourceLanguages));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));

            UMTLog.Info($"{nameof(MapItems)} pipeline processor started");

            args.TargetItems = GetTargetItems(args.SourceItems, args.SourceLanguages, args.SourceChannel, args.TargetTemplates);
            UMTLog.Info($"{nameof(MapItems)}: " + args.TargetItems.Count + " items have been mapped", true);

            UMTLog.Info($"{nameof(MapItems)} pipeline processor finished");
        }

        protected virtual Dictionary<string, TargetItem> GetTargetItems(IList<Item> items, IList<Language> languages,
            ChannelMap channel, Dictionary<Guid, TargetContentType> templates)
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
                    
                    mappedItems.Add(path, MapToTargetItem(item, path, index, languages, channel, templates));
                    UMTJob.IncreaseProcessedItems();
                }
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(Item item, string contentPath, int duplicateIndex, IList<Language> languages, ChannelMap channel,
            Dictionary<Guid, TargetContentType> templates)
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
                    WebPageItemParentGuid = item.Parent.ID.Guid.ToWebPageItemGuid(),
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
                        ContentItemLanguageMetadataGUID = item.ID.Guid.ToContentItemLanguageMetadataGuid(languageId),
                        ContentItemLanguageMetadataContentItemGuid = item.ID.Guid,
                        ContentItemLanguageMetadataContentLanguageGuid = languageId,
                        ContentItemLanguageMetadataDisplayName = languageVersion.DisplayName,
                        ContentItemLanguageMetadataCreatedWhen = languageVersion.Statistics.Created,
                        ContentItemLanguageMetadataModifiedWhen = languageVersion.Statistics.Updated,
                        ContentItemLanguageMetadataLatestVersionStatus = 2,
                        ContentItemLanguageMetadataHasImageAsset = false
                    });

                    var commonDataId = item.ID.Guid.ToContentItemCommonDataGuid(languageId);
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
                        }
                    }
                }
            }
            
            return fields;
        }
    }
}