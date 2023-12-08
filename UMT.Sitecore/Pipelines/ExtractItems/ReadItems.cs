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
        
        public void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(ReadItems)} pipeline processor started");

            args.SourceItems = GetItems(args.ContentPaths);

            UMTLog.Info($"{nameof(ReadItems)}: " + args.SourceItems.Count + " items have been found");
            foreach (var sourceItem in args.SourceItems)
            {
                UMTLog.Debug(sourceItem.Paths.FullPath);
            }

            UMTLog.Info($"{nameof(ReadItems)} pipeline processor finished");
        }

        public List<Item> GetItems(List<string> contentPaths)
        {
            var items = new List<Item>();

            foreach (var contentPath in contentPaths)
            {
                var item = Database.GetItem(contentPath);
                items.Add(item);
            }

            return items;
        }
    }
}