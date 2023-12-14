using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Configuration;
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
                    isUnderPageRoot = item != null && UMTConfigurationManager.ContentMapping.IsUnderPageRoot(item.Paths.FullPath);
                }
                else
                {
                    isUnderPageRoot = UMTConfigurationManager.ContentMapping.IsUnderPageRoot(field.Source.Replace("query:", "").Replace("fast:", ""));
                }

                if (isUnderPageRoot)
                {
                    columnType = "webpages";
                }
                
            }
            return columnType;
        }
        
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = new DataClassFieldSettings
            {
                ControlName = DefaultControlName
            };
            if (!string.IsNullOrEmpty(field.Source))
            {
                bool isUnderPageRoot;
                if (ID.IsID(field.Source))
                {
                    var item = Factory.GetDatabase(UMTSettings.Database).GetItem(new ID(field.Source));
                    isUnderPageRoot = item != null && UMTConfigurationManager.ContentMapping.IsUnderPageRoot(item.Paths.FullPath);
                }
                else
                {
                    isUnderPageRoot = UMTConfigurationManager.ContentMapping.IsUnderPageRoot(field.Source.Replace("query:", "").Replace("fast:", ""));
                }
                if (isUnderPageRoot)
                {
                    fieldSettings.ControlName = "Kentico.Administration.WebPageSelector";
                }
            }
            return fieldSettings;
        }

        public override object Convert(Field field, Item item)
        {
            var referenceField = (ReferenceField)field;
            if (referenceField?.TargetItem != null)
            {
                var fieldValue = new List<KeyValuePair<string, Guid>>
                {
                    new KeyValuePair<string, Guid>(
                        referenceField.TargetItem.HasPresentationDetails() ? "WebPageGuid" : "Identifier",
                        referenceField.TargetItem.HasPresentationDetails() ? referenceField.TargetItem.ID.Guid.ToWebPageItemGuid() : referenceField.TargetItem.ID.Guid)
                };
                return JsonConvert.SerializeObject(fieldValue);
            }
            return string.Empty;
        }
    }
}
