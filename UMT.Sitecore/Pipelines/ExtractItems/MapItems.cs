using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
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
            var targetItem = new TargetItem
            {
                Id = item.ID.Guid,
                Name = item.Name
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
                }
            }
            
            return targetItem;
        }

        protected virtual Dictionary<string, object> GetTargetItemFields(Item item)
        {
            var fields = new Dictionary<string, object>();

            foreach (var field in item.Fields)
            {
                
            }
            
            return fields;
        }

        /*public DataClassField MapTargetField(TemplateField field)
        {
            if (field == null || UMTConfigurationManager.FieldMapping.ShouldBeExcluded(field.ID.Guid)) return null;

            var fieldTypeMap = UMTConfigurationManager.FieldTypeMapping.GetByFieldType(field.TypeKey);

            //this is a known field type that should be extracted
            if (fieldTypeMap != null)
            {
                var dataClassField = new DataClassField
                {
                    AllowEmpty = true,
                    Column = field.Name.Replace(" ", ""),
                    Guid = field.ID.Guid,
                    ColumnSize = fieldTypeMap.ColumnSize,
                    ColumnType = fieldTypeMap.ColumnType,
                    Enabled = true,
                    Visible = true,
                    Properties = new DataClassFieldProperties { FieldCaption = field.Name },
                    Settings = new DataClassFieldSettings { ControlName = fieldTypeMap.ControlName }
                };
                return dataClassField;
            }


            return null;
        }*/
    }
}