using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapItems
    {
        public void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));

            UMTLog.Info($"{nameof(MapItems)} pipeline processor started");

            args.TargetItems = GetTargetItems(args.SourceItems, args.Channel);
            UMTLog.Info($"{nameof(MapItems)}: " + args.SourceItems.Count + " items have been mapped");

            UMTLog.Info($"{nameof(MapItems)} pipeline processor finished");
        }

        public List<ITargetItem> GetTargetItems(IList<Item> items, Channel channel)
        {
            var mappedItems = new List<ITargetItem>();

            foreach (var item in items)
            {
                mappedItems.Add(MapToTargetItem(item, channel));
            }

            return mappedItems;
        }

        public ITargetItem MapToTargetItem(Item item, Channel channel)
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
            return new ContentItem
            {
                ContentItemName = item.Name,
                ContentItemChannelGuid = channel.ChannelGUID,
                ContentItemGUID = item.ID.Guid,
                ContentItemDataClassGuid = item.TemplateID.Guid
            };
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