using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractTemplates
{
    public class MapTemplates
    {
        public void Process(ExtractTemplatesArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceTemplates, nameof(args.SourceTemplates));

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor started");

            args.TargetTemplates = GetTargetTemplates(args.SourceTemplates);
            UMTLog.Info($"{nameof(MapTemplates)}: " + args.SourceTemplates.Count + " templates have been mapped");

            UMTLog.Info($"{nameof(MapTemplates)} pipeline processor finished");
        }

        public IList<DataClass> GetTargetTemplates(IList<Template> templates)
        {
            var dataTemplates = new List<DataClass>();

            foreach (var template in templates)
            {
                dataTemplates.Add(MapToTargetTemplate(template));
            }

            return dataTemplates;
        }

        public DataClass MapToTargetTemplate(Template template)
        {
            var fields = template.GetFields(true);
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

            return targetTemplate;
        }

        public DataClassField MapTargetField(TemplateField field)
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
        }
    }
}
