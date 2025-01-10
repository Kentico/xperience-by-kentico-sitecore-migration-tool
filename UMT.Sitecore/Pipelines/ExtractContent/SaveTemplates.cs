using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Extensions;
using UMT.Sitecore.Jobs;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SaveTemplates : BaseSaveProcessor
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));
            Assert.ArgumentNotNull(args.SourceTemplates, nameof(args.SourceTemplates));

            UMTLog.Info($"{nameof(SaveTemplates)} pipeline processor started");
            UMTLog.Info($"Saving templates JSON files...", true);

            try
            {
                args.TargetTemplates = GetTargetTemplates(args.SourceTemplates, args.NameSpace, args.SourceChannel,
                    args.OutputFolderPath);
                SaveMediaTemplates(args.SourceChannel, args.NameSpace, args.OutputFolderPath);
                UMTLog.Info($"{args.TargetTemplates.Count} templates mapped and saved", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error saving templates, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(SaveTemplates)} pipeline processor finished");
        }

        protected virtual Dictionary<Guid, TargetContentType> GetTargetTemplates(Dictionary<Guid, Template> templates,
            string nameSpace, ChannelMap channel, string outputFolderPath)
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
                    UMTLog.ManualCheck(new UMTJobManualCheck
                    {
                        EntityType = EntityType.Template,
                        EntityId = template.Value.ID.Guid,
                        EntityName = template.Value.Name,
                        Message = $"Duplicate template is renamed to {templateName} to avoid conflicts"
                    });
                }
                
                var mappedTemplate = MapToTargetTemplate(template.Value, templateName, nameSpace, channel);
                targetTemplates.Add(mappedTemplate.Name, mappedTemplate);
                SaveSerializedTemplate(mappedTemplate, outputFolderPath);
                
                UMTJob.IncreaseProcessedItems();
            }

            return targetTemplates.ToDictionary(k => k.Value.Id, v => v.Value);
        }

        protected virtual void SaveMediaTemplates(ChannelMap channel, string nameSpace, string outputFolderPath)
        {
            foreach (var mediaTemplate in UMTConfiguration.MediaMapping.MediaTemplates)
            {
                var templateName = mediaTemplate.Name.ToValidClassName(nameSpace);
                var fields = new List<DataClassField>();
                if (!string.IsNullOrEmpty(mediaTemplate.AssetFieldName) && mediaTemplate.AssetFieldId != Guid.Empty)
                {
                    var fieldName = mediaTemplate.AssetFieldName.ToValidFieldName();
                    fields.Add(new DataClassField
                    {
                        AllowEmpty = true,
                        Column = fieldName,
                        Guid = mediaTemplate.AssetFieldId,
                        ColumnSize = 0,
                        ColumnType = "contentitemasset",
                        Enabled = true,
                        Visible = true,
                        Properties = new DataClassFieldProperties { FieldCaption = mediaTemplate.AssetFieldName },
                        Settings =  new DataClassFieldSettings
                        {
                            ControlName = "Kentico.Administration.ContentItemAssetUploader",
                            AllowedExtensions = "_INHERITED_"
                        }
                    });
                }
                if (!string.IsNullOrEmpty(mediaTemplate.AltFieldName) && mediaTemplate.AltFieldId != Guid.Empty)
                {
                    var fieldName = mediaTemplate.AltFieldName.ToValidFieldName();
                    fields.Add(new DataClassField
                    {
                        AllowEmpty = true,
                        Column = fieldName,
                        Guid = mediaTemplate.AltFieldId,
                        ColumnSize = 0,
                        ColumnType = "longtext",
                        Enabled = true,
                        Visible = true,
                        Properties = new DataClassFieldProperties { FieldCaption = mediaTemplate.AltFieldName },
                        Settings =  new DataClassFieldSettings
                        {
                            ControlName = "Kentico.Administration.TextInput"
                        }
                    });
                }
                
                var targetTemplate = new TargetContentType
                {
                    Id = mediaTemplate.Id,
                    ClassName = templateName,
                    Name = templateName,
                    ContentType = new DataClass
                    {
                        ClassDisplayName = mediaTemplate.Name,
                        ClassName = templateName,
                        ClassTableName = templateName.ToValidTableName(nameSpace),
                        ClassGUID = mediaTemplate.Id,
                        ClassHasUnmanagedDbSchema = false,
                        ClassType = "Content",
                        ClassContentTypeType = "Reusable",
                        ClassWebPageHasUrl = false,
                        Fields = fields
                    },
                    ContentTypeChannel = new ContentTypeChannel
                    {
                        ContentTypeChannelChannelGuid = channel.Id,
                        ContentTypeChannelContentTypeGuid = mediaTemplate.Id
                    }
                };
                
                SaveSerializedTemplate(targetTemplate, outputFolderPath);
            }
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
                ClassName = className
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
                var fieldName = field.Name.ToValidFieldName();

                //in case there are duplicate fields with the same name, add a number to the name
                if (targetTemplate.Fields.Any(x => string.Equals(x.Column, fieldName)))
                {
                    var index = 2;
                    while (targetTemplate.Fields.Any(x => string.Equals(x.Column, $"{fieldName}{index}")))
                    {
                        index++;
                    }
                    fieldName = $"{fieldName}{index}";
                }
                
                var mappedField = MapTargetField(field, fieldName);
                if (mappedField != null)
                {
                    targetTemplate.Fields.Add(mappedField);
                }
            }
            
            targetContentType.ContentType = targetTemplate;
            targetContentType.ContentTypeChannel = new ContentTypeChannel
            {
                ContentTypeChannelChannelGuid = channel.Id,
                ContentTypeChannelContentTypeGuid = template.ID.Guid
            };

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

        protected virtual DataClassField MapTargetField(TemplateField field, string fieldName)
        {
            if (field == null || UMTConfiguration.FieldMapping.ShouldBeExcluded(field.ID.Guid)) return null;

            var fieldTypeMap = UMTConfiguration.FieldTypeMapping.GetByFieldType(field.TypeKey);

            //this is a known field type that should be extracted
            if (fieldTypeMap?.TypeConverter != null)
            {
                var dataClassField = new DataClassField
                {
                    AllowEmpty = true,
                    Column = fieldName,
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

        protected virtual void SaveSerializedTemplate(TargetContentType template, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/02.ContentTypes");
            var fileName = $"{folderPath}/{template.Name}.{template.Id:D}.json";
            SerializeToFile(new List<object>
            {
                template.ContentType,
                template.ContentTypeChannel
            }, fileName);
        }
    }
}
