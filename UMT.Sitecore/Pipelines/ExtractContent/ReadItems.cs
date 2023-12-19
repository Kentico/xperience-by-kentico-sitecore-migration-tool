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
            UMTLog.Info($"{nameof(ReadItems)} pipeline processor started");

            var items = new List<Item>();
            AddSourceItems(args.ContentPaths, items);
            args.SourceItems = items;

            UMTLog.Info($"{nameof(ReadItems)}: {args.SourceItems.Count} items have been found", true);
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
            return false; //TODO: check against the list of excluded templates
        }
    }
}