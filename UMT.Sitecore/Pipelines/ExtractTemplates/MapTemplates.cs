using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractTemplates
{
    public class MapTemplates
    {
        public virtual void Process(ExtractTemplatesArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));
            Assert.ArgumentNotNull(args.SourceTemplates, nameof(args.SourceTemplates));

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor started");

            args.TargetTemplates = GetTargetTemplates(args.SourceTemplates, args.SourceChannel);
            UMTLog.Info($"{nameof(MapTemplates)}: " + args.TargetTemplates.Count + " templates have been mapped");

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor finished");
        }

        protected virtual IList<TargetContentType> GetTargetTemplates(IList<Template> templates, ChannelMap channel)
        {
            var dataTemplates = new List<TargetContentType>();

            foreach (var template in templates)
            {
                dataTemplates.Add(MapToTargetTemplate(template, channel));
            }

            return dataTemplates;
        }

        protected virtual TargetContentType MapToTargetTemplate(Template template, ChannelMap channel)
        {
            var fields = template.GetFields(true);
            var templateItem = Factory.GetDatabase(UMTSettings.Database).GetItem(template.ID);
            var isPage = HasPresentation(template);
            var nameSpace = "UMT"; //TODO: pass from the form
            var templateName = template.Name.ToValidClassName(nameSpace);
            var targetContentType = new TargetContentType
            {
                Id = template.ID.Guid,
                Name = templateName,
                Elements = new List<ITargetItemElement>()
            };
            var targetTemplate = new DataClass
            {
                ClassDisplayName = template.Name,
                ClassName = template.Name.ToValidClassName(nameSpace),
                ClassTableName = template.Name.ToValidTableName(nameSpace),
                ClassLastModified = templateItem.Statistics.Updated,
                ClassGUID = template.ID.Guid,
                ClassHasUnmanagedDbSchema = false,
                ClassType = "Content",
                ClassContentTypeType = isPage ? "Website" : "Reusable",
                ClassWebPageHasUrl = isPage,
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
            if (field == null || UMTConfigurationManager.FieldMapping.ShouldBeExcluded(field.ID.Guid)) return null;

            var fieldTypeMap = UMTConfigurationManager.FieldTypeMapping.GetByFieldType(field.TypeKey);

            //this is a known field type that should be extracted
            if (fieldTypeMap != null)
            {
                var dataClassField = new DataClassField
                {
                    AllowEmpty = true,
                    Column = field.Name.ToValidName(),
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
        }
    }
}
