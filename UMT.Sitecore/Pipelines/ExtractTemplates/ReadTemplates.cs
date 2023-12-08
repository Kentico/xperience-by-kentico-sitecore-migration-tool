using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;

namespace UMT.Sitecore.Pipelines.ExtractTemplates
{
    public class ReadTemplates
    {
        protected Database Database { get; }

        public List<string> IncludeTemplatePaths { get; } = new List<string>();

        public List<string> ExcludeTemplatePaths { get; } = new List<string>();

        public ReadTemplates()
        {
            Database = Factory.GetDatabase(UMTSettings.Database);
        }

        public virtual void Process(ExtractTemplatesArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(ReadTemplates)} pipeline processor started");

            var templates = new List<Template>();
            AddTemplates(templates);
            args.SourceTemplates = templates;

            UMTLog.Info($"{nameof(ReadTemplates)}: " + args.SourceTemplates.Count + " templates have been found");
            foreach (var sourceTemplate in args.SourceTemplates)
            {
                UMTLog.Debug(sourceTemplate.FullName);
            }

            UMTLog.Info($"{nameof(ReadTemplates)} pipeline processor finished");
        }

        protected virtual void AddTemplates(List<Template> templates)
        {
            foreach (var includeTemplatePath in IncludeTemplatePaths)
            {
                if (!ShouldBeExcluded(includeTemplatePath))
                {
                    var item = Database.GetItem(includeTemplatePath);
                    AddChildTemplates(item, templates);
                }
            }
        }

        protected virtual void AddChildTemplates(Item item, List<Template> templates)
        {
            if (item != null && !ShouldBeExcluded(item.Paths.FullPath))
            {
                if (TemplateManager.IsTemplate(item))
                {
                    var template = TemplateManager.GetTemplate(item.ID, Database);
                    templates.Add(template);
                }
                else
                {
                    var children = item.Children.InnerChildren;
                    foreach (var child in children)
                    {
                        AddChildTemplates(child, templates);
                    }
                }
            }
        }

        protected virtual bool ShouldBeExcluded(string templatePath)
        {
            return string.IsNullOrEmpty(templatePath) || ExcludeTemplatePaths.Any(x => templatePath.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
