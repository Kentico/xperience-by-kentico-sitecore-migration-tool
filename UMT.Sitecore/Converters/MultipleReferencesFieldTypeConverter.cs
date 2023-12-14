using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
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
                var isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(dataSource);
                if (isUnderPageRoot)
                {
                    UMTLog.Info($"Reference field {field.Name} ({field.ID}) with Source=\"{field.Source}\" has been identified as web page reference.");
                }
                return isUnderPageRoot ? "webpages" : base.GetColumnType(field);
            }
            return DefaultColumnType;
        }
        
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = new DataClassFieldSettings
            {
                ControlName = DefaultControlName,
                Sortable = true
            };
            if (!string.IsNullOrEmpty(field.Source))
            {
                var dataSource = StringUtil.ExtractParameter("DataSource", field.Source).Trim();
                var isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(dataSource);
                if (isUnderPageRoot)
                {
                    fieldSettings.ControlName = "Kentico.Administration.WebPageSelector";
                    fieldSettings.TreePath = dataSource;
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
                    var isUnderPageRoot = UMTConfiguration.ContentMapping.IsUnderPageRoot(linkedItem.Paths.FullPath);
                    var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(linkedItem.TemplateID.Guid);
                    if (isUnderPageRoot && isContentHubItem)
                    {
                        UMTLog.Warn($"Reference field {field.Name} ({field.ID}) contains a link to the item {linkedItem.Name} ({linkedItem.ID})," +
                                    $"which is under one of page roots but configured as a Content Hub item." +
                                    $"This item is skipped, check contentMapping/pageRoots and templateMapping/contentHubTemplates config sections.");
                    }
                    else
                    {
                        fieldValue.Add(new KeyValuePair<string, Guid>(
                            isContentHubItem ? "Identifier" : "WebPageGuid",
                            isContentHubItem ? linkedItem.ID.Guid : linkedItem.ID.Guid.ToWebPageItemGuid()));
                    }
                };
                return JsonConvert.SerializeObject(fieldValue);
            }
            return string.Empty;
        }
    }
}
