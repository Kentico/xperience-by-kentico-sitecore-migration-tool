using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore;
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
                    fieldSettings.TreePath = dataSource.Substring("/sitecore/content".Length);
                }
                else
                {
                    fieldSettings = base.GetFieldSettings(field);
                    fieldSettings.MaximumPages = 0;
                }
            }
            return fieldSettings;
        }

        public override TargetFieldValue Convert(Field field, Item item)
        {
            var result = new TargetFieldValue();
            var referenceField = (MultilistField)field;
            var linkedItems = referenceField?.GetItems();
            if (linkedItems != null && linkedItems.Length > 0)
            {
                var fieldValue = new List<IReferenceField>();
                result.References = new List<ContentItemReference>();
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
                        if (isContentHubItem)
                        {
                            fieldValue.Add(new ContentItemReferenceField(linkedItem.ID.Guid));

                            result.References = new List<ContentItemReference>
                            {
                                new ContentItemReference
                                {
                                    ContentItemReferenceGUID = field.ID.Guid.GenerateDerivedGuid("ContentItemReference",
                                        item.ID.Guid.ToString(), linkedItem.ID.Guid.ToString()),
                                    ContentItemReferenceSourceCommonDataGuid =
                                        item.ID.Guid.ToContentItemCommonDataGuid(item.Language.Name),
                                    ContentItemReferenceTargetItemGuid = linkedItem.ID.Guid,
                                    ContentItemReferenceGroupGUID = field.ID.Guid
                                }
                            };
                        }
                        else
                        {
                            fieldValue.Add(new WebPageReferenceField(linkedItem.ID.Guid.ToWebPageItemGuid()));
                        }
                    }
                }
                result.Value = JsonConvert.SerializeObject(fieldValue);
            }
            return result;
        }
    }
}
