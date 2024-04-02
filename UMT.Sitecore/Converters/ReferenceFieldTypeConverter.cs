using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Abstractions;
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
                bool isUnderPageRoot = true;
                if (ID.IsID(field.Source))
                {
                    var item = Factory.GetDatabase(UMTSettings.Database).GetItem(new ID(field.Source));
                    if (item != null && !UMTConfiguration.ContentMapping.PathContainsAnyPageRoot(item.Paths.FullPath))
                    {
                        isUnderPageRoot = false;
                    }
                }
                else
                {
                    if (!UMTConfiguration.ContentMapping.PathContainsAnyPageRoot(field.Source))
                    {
                        isUnderPageRoot = false;
                    }
                }

                if (isUnderPageRoot)
                {
                    columnType = "webpages";
                    UMTLog.Info($"Reference field {field.Name} ({field.ID}) with Source=\"{field.Source}\" has been identified as web page reference.");
                }
                else
                {
                    columnType = "contentitemreference";
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
                bool isUnderPageRoot = true;
                if (ID.IsID(field.Source))
                {
                    var item = Factory.GetDatabase(UMTSettings.Database).GetItem(new ID(field.Source));
                    if (item != null && !UMTConfiguration.ContentMapping.PathContainsAnyPageRoot(item.Paths.FullPath))
                    {
                        isUnderPageRoot = false;
                    }
                }
                else
                {
                    if (!UMTConfiguration.ContentMapping.PathContainsAnyPageRoot(field.Source))
                    {
                        isUnderPageRoot = false;
                    }
                }

                if (isUnderPageRoot)
                {
                    fieldSettings.TreePath = "/";
                }
                
                fieldSettings.ControlName = isUnderPageRoot 
                    ? "Kentico.Administration.WebPageSelector" 
                    : "Kentico.Administration.ContentItemSelector";
            }
            return fieldSettings;
        }

        public override TargetFieldValue Convert(Field field, Item item)
        {
            var result = new TargetFieldValue();
            var referenceField = (ReferenceField)field;
            
            if (referenceField?.TargetItem != null)
            {
                var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(referenceField.TargetItem.TemplateID.Guid);

                var fieldValue = new List<IReferenceField>();
                if (isContentHubItem)
                {
                    fieldValue.Add(new ContentItemReferenceField(referenceField.TargetItem.ID.Guid));

                    result.References = new List<ContentItemReference>
                    {
                        new ContentItemReference
                        {
                            ContentItemReferenceGUID = field.ID.Guid.GenerateDerivedGuid("ContentItemReference",
                                item.ID.Guid.ToString(), referenceField.TargetItem.ID.Guid.ToString()),
                            ContentItemReferenceSourceCommonDataGuid =
                                item.ID.Guid.ToContentItemCommonDataGuid(item.Language.Name),
                            ContentItemReferenceTargetItemGuid = referenceField.TargetItem.ID.Guid,
                            ContentItemReferenceGroupGUID = field.ID.Guid
                        }
                    };
                }
                else
                {
                    fieldValue.Add(new WebPageReferenceField(referenceField.TargetItem.ID.Guid.ToWebPageItemGuid()));
                }

                result.Value = JsonConvert.SerializeObject(fieldValue);

            }

            return result;
        }
    }
}
