using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Text;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class MultipleReferencesFieldTypeConverter : ReferenceFieldTypeConverter
    {
        public override string GetColumnType(TemplateField field)
        {
            if (!string.IsNullOrEmpty(field.Source))
            {
                var dataSource = StringUtil.ExtractParameter("DataSource", field.Source).Trim();
                var isUnderPageRoot = UMTConfigurationManager.ContentMapping.IsUnderPageRoot(dataSource);
                return isUnderPageRoot ? "webpages" : base.GetColumnType(field);
            }
            return DefaultColumnType;
        }
        
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = new DataClassFieldSettings
            {
                ControlName = DefaultControlName
            };
            if (!string.IsNullOrEmpty(field.Source))
            {
                var dataSource = StringUtil.ExtractParameter("DataSource", field.Source).Trim();
                var isUnderPageRoot = UMTConfigurationManager.ContentMapping.IsUnderPageRoot(dataSource);
                if (isUnderPageRoot)
                {
                    fieldSettings.ControlName = "Kentico.Administration.WebPageSelector";
                }
                else
                {
                    fieldSettings = base.GetFieldSettings(field);
                }
            }
            return fieldSettings;
        }

        public override object Convert(Field field, Item item)
        {
            var referenceField = (MultilistField)field;
            var linkedItems = referenceField?.GetItems();
            if (linkedItems != null && linkedItems.Length > 0)
            {
                var fieldValue = new List<KeyValuePair<string, Guid>>();
                foreach (var linkedItem in linkedItems)
                {
                    fieldValue.Add(new KeyValuePair<string, Guid>(
                                                           linkedItem.HasPresentationDetails() ? "WebPageGuid" : "Identifier",
                                                           linkedItem.ID.Guid));
                };
                return JsonConvert.SerializeObject(fieldValue);
            }
            return string.Empty;
        }
    }
}
