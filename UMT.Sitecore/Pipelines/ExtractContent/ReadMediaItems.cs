using System.Collections.Generic;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Jobs;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class ReadMediaItems
    {
        protected Database Database { get; }

        public ReadMediaItems()
        {
            Database = Factory.GetDatabase(UMTSettings.Database);
        }
        
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            UMTLog.Info($"{nameof(ReadMediaItems)} pipeline processor started");

            var items = new List<MediaItem>();
            AddMediaItems(args.MediaPaths, items);
            args.SourceMediaItems = items;

            UMTLog.Info($"{nameof(ReadMediaItems)}: {args.SourceMediaItems.Count} media items have been found", true);

            UMTLog.Info($"{nameof(ReadMediaItems)} pipeline processor finished");
        }

        protected virtual void AddMediaItems(List<string> contentPaths, List<MediaItem> items)
        {
            foreach (var contentPath in contentPaths)
            {
                var item = Database.GetItem(contentPath);
                AddChildItems(item, items);
            }
        }
        
        protected virtual void AddChildItems(Item parentItem, List<MediaItem> items)
        {
            if (parentItem != null)
            {
                if (!ShouldBeExcluded(parentItem))
                {
                    var mediaItem = new MediaItem(parentItem);
                    if (mediaItem.Size > 0)
                    {
                        items.Add(mediaItem);
                        UMTJob.IncreaseTotalItems();
                    }
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
            return item.TemplateID == TemplateIDs.MediaFolder;
        }
    }
}