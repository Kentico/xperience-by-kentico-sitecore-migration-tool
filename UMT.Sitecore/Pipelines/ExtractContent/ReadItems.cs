using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Jobs;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class ReadItems
    {
        protected Database Database { get; } = Factory.GetDatabase(UMTSettings.Database);

        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.ContentPaths, nameof(args.ContentPaths));
            
            UMTLog.Info($"{nameof(ReadItems)} pipeline processor started");
            UMTLog.Info($"Reading content items for paths: {string.Join(", ", args.ContentPaths)}...", true);

            try
            {
                var items = new List<Item>();
                AddSourceItems(args.ContentPaths, items);
                args.SourceItems = items;
                UMTLog.Info($"{args.SourceItems.Count} content items found", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error reading content items, please check logs for more details", true, e);
                args.AbortPipeline();
            }

            UMTLog.Info($"{nameof(ReadItems)} pipeline processor finished");
        }

        protected virtual void AddSourceItems(List<string> contentPaths, List<Item> items)
        {
            foreach (var contentPath in contentPaths)
            {
                var item = Database.GetItem(contentPath);
                AddChildItems(item, items);
            }
        }
        
        protected virtual void AddChildItems(Item parentItem, List<Item> items)
        {
            if (parentItem != null)
            {
                if (!ShouldBeExcluded(parentItem))
                {
                    items.Add(parentItem);
                    UMTJob.IncreaseTotalItems();
                }
                
                var children = parentItem.Children.InnerChildren;
                foreach (var child in children)
                {
                    AddChildItems(child, items);
                }
            }
        }

        protected virtual bool ShouldBeExcluded(Item item)
        {
            return UMTConfiguration.TemplateMapping.ShouldBeExcluded(item.TemplateID.Guid) ||
                   UMTConfiguration.ContentMapping.ShouldBeExcluded(item.Paths.FullPath);
        }
    }
}