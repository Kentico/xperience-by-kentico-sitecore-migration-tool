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
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = base.GetFieldSettings(field);
            fieldSettings.MaximumPages = 0;
            
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
                    var isContentHubItem = UMTConfiguration.TemplateMapping.IsContentHubTemplate(linkedItem.TemplateID.Guid);

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
                result.Value = JsonConvert.SerializeObject(fieldValue);
            }
            return result;
        }
    }
}
