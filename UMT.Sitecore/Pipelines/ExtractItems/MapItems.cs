using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapItems
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));

            UMTLog.Info($"{nameof(MapItems)} pipeline processor started");

            args.TargetItems = GetTargetItems(args.SourceItems, args.SourceChannel);
            UMTLog.Info($"{nameof(MapItems)}: " + args.SourceItems.Count + " items have been mapped");

            UMTLog.Info($"{nameof(MapItems)} pipeline processor finished");
        }

        protected virtual List<TargetItem> GetTargetItems(IList<Item> items, ChannelMap channel)
        {
            var mappedItems = new List<TargetItem>();

            foreach (var item in items)
            {
                mappedItems.Add(MapToTargetItem(item, channel));
            }

            return mappedItems;
        }

        protected virtual TargetItem MapToTargetItem(Item item, ChannelMap channel)
        {
            /*var fields = template.GetFields(true);
            var templateItem = Factory.GetDatabase(UMTSettings.Database).GetItem(template.ID);
            var className = template.Name.Replace(" ", ""); //TODO: sanitize
            var isPage = true;  //TODO: detect by presentation layout
            var nameSpace = "UMT"; //TODO: pass from the form
            var targetTemplate = new DataClass
            {
                ClassDisplayName = template.Name,
                ClassName = $"{nameSpace}.{className}",
                ClassTableName = $"{nameSpace}_{className}",
                ClassLastModified = templateItem.Statistics.Updated,
                ClassGUID = template.ID.Guid,
                ClassHasUnmanagedDbSchema = false,
                ClassType = "Content",
                ClassContentTypeType = isPage ? "Website" : "Reusable",
                ClassWebPageHasUrl = isPage,
                Type = "DataClass",
                Fields = new List<DataClassField>()
            };

            foreach (var field in fields)
            {
                var mappedField = MapTargetField(field);
                if (mappedField != null)
                {
                    targetTemplate.Fields.Add(mappedField);
                }
            }

            return targetTemplate;*/
            var targetItem = new TargetItem
            {
                Id = item.ID.Guid,
                Name = item.Name
            };
            targetItem.Elements.Add(new ContentItemElement
            {
                ContentItemName = item.Name,
                ContentItemChannelGuid = channel.Id,
                ContentItemGUID = item.ID.Guid,
                ContentItemDataClassGuid = item.TemplateID.Guid
            });
            return targetItem;
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