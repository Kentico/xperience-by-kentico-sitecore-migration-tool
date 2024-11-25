using System;
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
            Assert.ArgumentNotNull(args.MediaPaths, nameof(args.MediaPaths));
            
            UMTLog.Info($"{nameof(ReadMediaItems)} pipeline processor started");
            UMTLog.Info($"Reading media items for paths: {string.Join(", ", args.MediaPaths)}...", true);
            try
            {
                var items = new List<MediaItem>();
                var folders = new List<Item>();
                AddMediaItems(args.MediaPaths, items, folders);
                args.SourceMediaItems = items;
                args.SourceMediaFolders = folders;
                UMTLog.Info($"{args.SourceMediaItems.Count} media items found", true);
                UMTLog.Info($"{args.SourceMediaFolders.Count} media folders found", true);
            }
            catch (Exception e)
            {
                UMTLog.Error($"Error reading media items, please check logs for more details", true, e);
                args.AbortPipeline();
            }
            
            UMTLog.Info($"{nameof(ReadMediaItems)} pipeline processor finished");
        }

        protected virtual void AddMediaItems(List<string> contentPaths, List<MediaItem> items, List<Item> folders)
        {
            foreach (var contentPath in contentPaths)
            {
                var item = Database.GetItem(contentPath);
                AddChildItems(item, items, folders);
            }
        }
        
        protected virtual void AddChildItems(Item parentItem, List<MediaItem> items, List<Item> folders)
        {
            if (parentItem != null)
            {
                if (IsMediaFolder(parentItem))
                {
                    folders.Add(parentItem);
                    UMTJob.IncreaseTotalItems();
                }
                else
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
                    AddChildItems(child, items, folders);
                }
            }
        }

        protected virtual bool IsMediaFolder(Item item)
        {
            return item.TemplateID == TemplateIDs.MediaFolder;
        }
    }
}