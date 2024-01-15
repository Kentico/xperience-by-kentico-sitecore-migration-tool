using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Jobs;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class ReadTemplates
    {
        protected Database Database { get; } = Factory.GetDatabase(UMTSettings.Database);

        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceItems, nameof(args.SourceItems));
            
            UMTLog.Info($"{nameof(ReadTemplates)} pipeline processor started");
            UMTLog.Info($"Reading templates from the found content items...", true);

            try
            {
                args.SourceTemplates = GetTemplates(args.SourceItems);
                UMTLog.Info($"{args.SourceTemplates.Count} templates found", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error reading templates, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(ReadTemplates)} pipeline processor finished");
        }

        protected virtual Dictionary<Guid, Template> GetTemplates(List<Item> sourceItems)
        {
            var templates = new Dictionary<Guid, Template>();

            foreach (var sourceItem in sourceItems)
            {
                if (!templates.ContainsKey(sourceItem.TemplateID.Guid))
                {
                    var template = TemplateManager.GetTemplate(sourceItem.TemplateID, Database);
                    if (template != null)
                    {
                        templates.Add(template.ID.Guid, template);
                        UMTJob.IncreaseTotalItems();
                    }
                }
            }

            return templates;
        }
    }
}
