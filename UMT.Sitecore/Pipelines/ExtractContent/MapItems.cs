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

        protected virtual List<TargetItem> GetTargetItems(IList<Item> items, IList<Language> languages,
            ChannelMap channel, Dictionary<Guid, TargetContentType> templates)
        {
            var mappedItems = new List<TargetItem>();

            foreach (var item in items)
            {
                if (templates.ContainsKey(item.TemplateID.Guid))
                {
                    mappedItems.Add(MapToTargetItem(item, languages, channel, templates));
                }
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(Item item, IList<Language> languages, ChannelMap channel,
            Dictionary<Guid, TargetContentType> templates)
        {
            var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(item.TemplateID.Guid);
            var webPageItemId = item.ID.Guid.ToWebPageItemGuid();
            var itemName = item.Name.ToValidItemName();
            var targetItem = new TargetItem
            {
                Id = item.ID.Guid,
                Name = itemName,
                IsWebPage = !isContentHubItem
            };
            targetItem.Elements.Add(new ContentItem
            {
                ContentItemName = itemName,
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
                    WebPageItemName = itemName,
                    WebPageItemContentItemGuid = item.ID.Guid,
                    WebPageItemParentGuid = item.Parent.ID.Guid.ToWebPageItemGuid(),
                    WebPageItemWebsiteChannelGuid = channel.WebsiteId,
                    WebPageItemTreePath = item.Paths.ContentPath,
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

                    var targetItemFields = GetTargetItemFields(languageVersion);
                    targetItem.Elements.Add(new ContentItemData
                    {
                        ContentItemDataGUID = item.ID.Guid.ToContentItemDataGuid(languageId),
                        ContentItemDataCommonDataGuid = commonDataId,
                        ContentItemContentTypeName = templates[item.TemplateID.Guid].ClassName,
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

        protected virtual Dictionary<string, TargetFieldValue> GetTargetItemFields(Item item)
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
                        var fieldName = field.Name.ToValidFieldName();
                        if (!fields.ContainsKey(fieldName))
                        {
                            var mappedValue = fieldTypeMapper.TypeConverter.Convert(field, item);
                            fields.Add(fieldName, mappedValue);
                        }
                        else
                        {
                            UMTLog.Warn($"Field {field.Name} ({field.ID}) of item {item.Name} ({item.ID}) has been skipped because " +
                                        $"another field with the same name has already been added.");
                        }
                    }
                }
            }
            
            return fields;
        }
    }
}