using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
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

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapItems
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));
            Assert.ArgumentNotNull(args.SourceLanguages, nameof(args.SourceLanguages));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));

            UMTLog.Info($"{nameof(MapItems)} pipeline processor started");

            args.TargetItems = GetTargetItems(args.SourceItems, args.SourceLanguages, args.SourceChannel);
            UMTLog.Info($"{nameof(MapItems)}: " + args.TargetItems.Count + " items have been mapped");

            UMTLog.Info($"{nameof(MapItems)} pipeline processor finished");
        }

        protected virtual List<TargetItem> GetTargetItems(IList<Item> items, IList<Language> languages, ChannelMap channel)
        {
            var mappedItems = new List<TargetItem>();

            foreach (var item in items)
            {
                mappedItems.Add(MapToTargetItem(item, languages, channel));
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(Item item, IList<Language> languages, ChannelMap channel)
        {
            var isWebPage = item.HasPresentationDetails();
            var webPageItemId = item.ID.Guid.GenerateDerivedGuid("WebPageItem");
            var targetItem = new TargetItem
            {
                Id = item.ID.Guid,
                Name = item.Name,
                IsWebPage = isWebPage
            };
            targetItem.Elements.Add(new ContentItem
            {
                ContentItemName = item.Name.ToValidName(),
                ContentItemChannelGuid = channel.Id,
                ContentItemGUID = item.ID.Guid,
                ContentItemDataClassGuid = item.TemplateID.Guid,
                ContentItemIsSecured = false,
                ContentItemIsReusable = true
            });

            if (isWebPage)
            {
                targetItem.Elements.Add(new WebPageItem
                {
                    WebPageItemGUID = webPageItemId,
                    WebPageItemName = item.Name.ToValidName(),
                    WebPageItemContentItemGuid = item.ID.Guid,
                    WebPageItemParentGuid = item.Parent.ID.Guid.GenerateDerivedGuid("WebPageItem"),
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
                    var languageId = UMTConfigurationManager.LanguageMapping.GetTargetLanguageId(language.Origin.ItemId.Guid);
                    
                    targetItem.Elements.Add(new ContentItemLanguageMetadata
                    {
                        ContentItemLanguageMetadataGUID = item.ID.Guid.GenerateDerivedGuid("ContentItemLanguageMetadata", languageId.ToString()),
                        ContentItemLanguageMetadataContentItemGuid = item.ID.Guid,
                        ContentItemLanguageMetadataContentLanguageGuid = languageId,
                        ContentItemLanguageMetadataDisplayName = languageVersion.DisplayName,
                        ContentItemLanguageMetadataCreatedWhen = languageVersion.Statistics.Created,
                        ContentItemLanguageMetadataModifiedWhen = languageVersion.Statistics.Updated,
                        ContentItemLanguageMetadataLatestVersionStatus = 2,
                        ContentItemLanguageMetadataHasImageAsset = false
                    });

                    var commonDataId = item.ID.Guid.GenerateDerivedGuid("ContentItemCommonData", languageId.ToString());
                    targetItem.Elements.Add(new ContentItemCommonData
                    {
                        ContentItemCommonDataGUID = commonDataId,
                        ContentItemCommonDataContentItemGuid = item.ID.Guid,
                        ContentItemCommonDataContentLanguageGuid = languageId,
                        ContentItemCommonDataVersionStatus = 2,
                        ContentItemCommonDataIsLatest = true
                    });
                    
                    targetItem.Elements.Add(new ContentItemData
                    {
                        ContentItemDataGUID = item.ID.Guid.GenerateDerivedGuid("ContentItemData", languageId.ToString()),
                        ContentItemDataCommonDataGuid = commonDataId,
                        ContentItemContentTypeName = item.TemplateName.ToValidClassName("UMT"),
                        Properties = GetTargetItemFields(languageVersion)
                    });

                    if (isWebPage)
                    {
                        var url = LinkManager.GetItemUrl(item, new UrlOptions { AlwaysIncludeServerUrl = false, Language = language }).TrimStart('/');
                        targetItem.Elements.Add(new WebPageUrlPath
                        {
                            WebPageUrlPathGUID = item.ID.Guid.GenerateDerivedGuid("WebPageUrlPath", languageId.ToString()),
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

        protected virtual Dictionary<string, object> GetTargetItemFields(Item item)
        {
            var fields = new Dictionary<string, object>();

            //this is required to read all field values, including Standard Values
            item.Fields.ReadAll();
            
            foreach (Field field in item.Fields)
            {
                if (!UMTConfigurationManager.FieldMapping.ShouldBeExcluded(field.ID.Guid))
                {
                    var fieldTypeMapper = UMTConfigurationManager.FieldTypeMapping.GetByFieldType(field.TypeKey);
                    if (fieldTypeMapper != null)
                    {
                        var mappedValue = fieldTypeMapper.TypeConverter.Convert(field, item);
                        fields.Add(field.Name.ToValidName(), mappedValue);
                    }
                }
            }
            
            return fields;
        }
    }
}