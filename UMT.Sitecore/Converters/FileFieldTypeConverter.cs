﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class FileFieldTypeConverter : BaseFieldTypeConverter
    {
        public override DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            var fieldSettings = base.GetFieldSettings(field);
            var mediaTemplate = UMTConfiguration.MediaMapping.GetMediaTemplate();
            if (mediaTemplate != null)
            {
                fieldSettings.AllowedContentItemTypeIdentifiers = new List<Guid>
                {
                    mediaTemplate.Id
                };
            }

            return fieldSettings;
        }

        public override TargetFieldValue Convert(Field field, Item item)
        {
            var result = new TargetFieldValue();
            var mediaField = (FileField)field;
            
            if (mediaField?.MediaItem != null)
            {
                var fieldValue = new List<IReferenceField> { new ContentItemReferenceField(mediaField.MediaItem.ID.Guid) };

                result.References = new List<ContentItemReference>
                {
                    new ContentItemReference
                    {
                        ContentItemReferenceGUID = field.ID.Guid.GenerateDerivedGuid("ContentItemReference",
                            item.ID.Guid.ToString(), mediaField.MediaItem.ID.Guid.ToString()),
                        ContentItemReferenceSourceCommonDataGuid =
                            item.ID.Guid.ToContentItemCommonDataGuid(item.Language.Name),
                        ContentItemReferenceTargetItemGuid = mediaField.MediaItem.ID.Guid,
                        ContentItemReferenceGroupGUID = field.ID.Guid
                    }
                };
                
                result.Value = JsonConvert.SerializeObject(fieldValue);
            }

            return result;
        }
    }
}
