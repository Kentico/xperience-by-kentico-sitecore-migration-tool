using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class ReferenceFieldTypeConverter : BaseFieldTypeConverter
    {
        public override string GetColumnType(TemplateField field)
        {
            var columnType = DefaultColumnType;
            if (!string.IsNullOrEmpty(field.Source))
            {
                bool isUnderPageRoot;
                if (ID.IsID(field.Source))
                {
                    var item = Factory.GetDatabase(UMTSettings.Database).GetItem(new ID(field.Source));
                    isUnderPageRoot = item != null && UMTConfiguration.ContentMapping.IsUnderPageRoot(item.Paths.FullPath);
                }
                else
                {
                    isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(field.Source.Replace("query:", "").Replace("fast:", ""));
                }

                if (isUnderPageRoot)
                {
                    columnType = "webpages";
                    UMTLog.Info($"Reference field {field.Name} ({field.ID}) with Source=\"{field.Source}\" has been identified as web page reference.");
                }
                else
                {
                    UMTLog.Info($"Reference field {field.Name} ({field.ID}) with Source=\"{field.Source}\" has been identified as content item reference.");
                }
                
            }
            return columnType;
        }
        
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = new DataClassFieldSettings
            {
                ControlName = DefaultControlName,
                MaximumPages = 1
            };
            if (!string.IsNullOrEmpty(field.Source))
            {
                bool isUnderPageRoot;
                if (ID.IsID(field.Source))
                {
                    var item = Factory.GetDatabase(UMTSettings.Database).GetItem(new ID(field.Source));
                    isUnderPageRoot = item != null && UMTConfiguration.ContentMapping.IsUnderPageRoot(item.Paths.FullPath);
                    if (isUnderPageRoot)
                    {
                        fieldSettings.TreePath = item.Paths.ContentPath;
                    }
                }
                else
                {
                    var sourcePath = field.Source.Replace("query:", "").Replace("fast:", "");
                    isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(sourcePath);
                    if (isUnderPageRoot)
                    {
                        fieldSettings.TreePath = sourcePath;
                    }
                }
                if (isUnderPageRoot)
                {
                    fieldSettings.ControlName = "Kentico.Administration.WebPageSelector";
                }
            }
            return fieldSettings;
        }

        public override TargetFieldValue Convert(Field field, Item item)
        {
            var result = new TargetFieldValue();
            var referenceField = (ReferenceField)field;
            if (referenceField?.TargetItem != null)
            {
                var isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(referenceField.TargetItem.Paths.FullPath);
                var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(referenceField.TargetItem.TemplateID.Guid);
                if (isUnderPageRoot && isContentHubItem)
                {
                    UMTLog.Warn($"Reference field {field.Name} ({field.ID}) contains a link to the item {referenceField.TargetItem.Name} ({referenceField.TargetItem.ID})," +
                                $"which is under one of page roots but configured as a Content Hub item." +
                                $"This item is skipped, check contentMapping/pageRoots and templateMapping/contentHubTemplates config sections.");
                }
                else
                {
                    var fieldValue = new List<KeyValuePair<string, Guid>>
                    {
                        new KeyValuePair<string, Guid>(
                            isContentHubItem ? "Identifier" : "WebPageGuid",
                            isContentHubItem ? referenceField.TargetItem.ID.Guid : referenceField.TargetItem.ID.Guid.ToWebPageItemGuid())
                    };
                    result.Value = JsonConvert.SerializeObject(fieldValue);
                    if (isContentHubItem)
                    {
                        result.References = new List<ContentItemReference>
                        {
                            new ContentItemReference
                            {
                                ContentItemReferenceGUID = field.ID.Guid.GenerateDerivedGuid("ContentItemReference", item.ID.Guid.ToString(), referenceField.TargetItem.ID.Guid.ToString()),
                                ContentItemReferenceSourceCommonDataGuid = item.ID.Guid.ToContentItemCommonDataGuid(item.Language.Origin.ItemId.Guid),
                                ContentItemReferenceTargetItemGuid = referenceField.TargetItem.ID.Guid,
                                ContentItemReferenceGroupGUID = field.ID.Guid
                            }
                        };
                    }
                }
            }
            return result;
        }
    }
}
