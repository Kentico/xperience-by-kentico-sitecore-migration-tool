using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class MapTemplates
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));
            Assert.ArgumentNotNull(args.SourceTemplates, nameof(args.SourceTemplates));

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor started");

            args.TargetTemplates = GetTargetTemplates(args.SourceTemplates, args.NameSpace, args.SourceChannel);
            UMTLog.Info($"{nameof(MapTemplates)}: " + args.TargetTemplates.Count + " templates have been mapped", true);

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor finished");
        }

        protected virtual Dictionary<Guid, TargetContentType> GetTargetTemplates(Dictionary<Guid, Template> templates,
            string nameSpace, ChannelMap channel)
        {
            var targetTemplates = new Dictionary<string, TargetContentType>();

            foreach (var template in templates)
            {
                var templateName = template.Value.Name.ToValidClassName(string.Empty);
                
                //in case there are different templates with the same name, add a number to the name
                if (targetTemplates.ContainsKey(templateName))
                {
                    var index = 2;
                    while (targetTemplates.ContainsKey($"{templateName}{index}"))
                    {
                        index++;
                    }

                    templateName = $"{templateName}{index}";
                    
                }
                
                var mappedTemplate = MapToTargetTemplate(template.Value, templateName, nameSpace, channel);
                targetTemplates.Add(mappedTemplate.Name, mappedTemplate);
            }

            return targetTemplates.ToDictionary(k => k.Value.Id, v => v.Value);
        }

        protected virtual TargetContentType MapToTargetTemplate(Template template, string templateName, string nameSpace, ChannelMap channel)
        {
            var fields = template.GetFields(true);
            var templateItem = Factory.GetDatabase(UMTSettings.Database).GetItem(template.ID);
            var isContentHubTemplate = UMTConfiguration.TemplateMapping.IsContentHubTemplate(template.ID.Guid);
            var className = templateName.ToValidClassName(nameSpace);
            var targetContentType = new TargetContentType
            {
                Id = template.ID.Guid,
                Name = templateName,
                ClassName = className,
                Elements = new List<ITargetItemElement>()
            };
            var targetTemplate = new DataClass
            {
                ClassDisplayName = template.Name,
                ClassName = className,
                ClassTableName = templateName.ToValidTableName(nameSpace),
                ClassLastModified = templateItem.Statistics.Updated,
                ClassGUID = template.ID.Guid,
                ClassHasUnmanagedDbSchema = false,
                ClassType = "Content",
                ClassContentTypeType = isContentHubTemplate ? "Reusable" : "Website",
                ClassWebPageHasUrl = HasPresentation(template),
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
            
            targetContentType.Elements.Add(targetTemplate);
            targetContentType.Elements.Add(new ContentTypeChannel
            {
                ContentTypeChannelChannelGuid = channel.Id,
                ContentTypeChannelContentTypeGuid = template.ID.Guid
            });

            return targetContentType;
        }

        protected virtual bool HasPresentation(Template template)
        {
            if (template.StandardValueHolderId != (ID)null)
            {
                var standardValueItem = Factory.GetDatabase(UMTSettings.Database).GetItem(template.StandardValueHolderId);
                return standardValueItem != null && standardValueItem.HasPresentationDetails();
            }

            return false;
        }

        protected virtual DataClassField MapTargetField(TemplateField field)
        {
            if (field == null || UMTConfiguration.FieldMapping.ShouldBeExcluded(field.ID.Guid)) return null;

            var fieldTypeMap = UMTConfiguration.FieldTypeMapping.GetByFieldType(field.TypeKey);

            //this is a known field type that should be extracted
            if (fieldTypeMap?.TypeConverter != null)
            {
                var dataClassField = new DataClassField
                {
                    AllowEmpty = true,
                    Column = field.Name.ToValidFieldName(),
                    Guid = field.ID.Guid,
                    ColumnSize = fieldTypeMap.TypeConverter.GetColumnSize(field),
                    ColumnType = fieldTypeMap.TypeConverter.GetColumnType(field),
                    Enabled = true,
                    Visible = true,
                    Properties = new DataClassFieldProperties { FieldCaption = field.Name },
                    Settings =  fieldTypeMap.TypeConverter.GetFieldSettings(field)
                };
                return dataClassField;
            }

            return null;
        }
    }
}
