using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class ReadItems
    {
        protected Database Database { get; }

        public ReadItems()
        {
            Database = Factory.GetDatabase(UMTSettings.Database);
        }
        
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(ReadItems)} pipeline processor started");

            var items = new List<Item>();
            AddTargetItems(args.ContentPaths, items);
            args.SourceItems = items;

            UMTLog.Info($"{nameof(ReadItems)}: {args.SourceItems.Count} items have been found");
            foreach (var sourceItem in args.SourceItems)
            {
                UMTLog.Debug(sourceItem.Paths.FullPath);
            }

            UMTLog.Info($"{nameof(ReadItems)} pipeline processor finished");
        }

        protected virtual void AddTargetItems(List<string> contentPaths, List<Item> items)
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