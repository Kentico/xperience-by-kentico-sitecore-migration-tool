using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapMediaItems
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceMediaItems, nameof(args.SourceMediaItems));

            UMTLog.Info($"{nameof(MapMediaItems)} pipeline processor started");

            args.TargetMediaItems = GetTargetMediaItems(args.SourceMediaItems);
            UMTLog.Info($"{nameof(MapMediaItems)}: " + args.TargetMediaItems.Count + " items have been mapped");

            UMTLog.Info($"{nameof(MapMediaItems)} pipeline processor finished");
        }

        protected virtual List<MediaFile> GetTargetMediaItems(IList<MediaItem> items)
        {
            var mappedItems = new List<MediaFile>();

            foreach (var item in items)
            {
                mappedItems.Add(MapToTargetItem(item));
            }

            return mappedItems;
        }

        protected virtual MediaFile MapToTargetItem(MediaItem mediaItem)
        {
            int.TryParse(mediaItem.InnerItem["Width"], out var width);
            int.TryParse(mediaItem.InnerItem["Height"], out var height);
            var fileName = mediaItem.Name.EndsWith(mediaItem.Extension)
                ? mediaItem.Name
                : $"{mediaItem.Name}.{mediaItem.Extension}";
            var targetItem = new MediaFile
            {
                FileGUID = mediaItem.ID.Guid,
                FileDescription = mediaItem.Description,
                FileExtension = mediaItem.Extension,
                FileName = fileName,
                FilePath = $"{mediaItem.MediaPath}/{fileName}",
                FileTitle = mediaItem.Title,
                FileMimeType = mediaItem.MimeType,
                FileImageHeight = height,
                FileImageWidth = width,
                FileCreatedWhen = mediaItem.InnerItem.Statistics.Created,
                FileModifiedWhen = mediaItem.InnerItem.Statistics.Updated
            };
            
            return targetItem;
        }
    }
}