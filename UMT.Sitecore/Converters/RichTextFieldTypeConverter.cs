using System;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class RichTextFieldTypeConverter : BaseFieldTypeConverter
    {
        protected const string ItemLinkStartPattern = "~/link.aspx?";
        protected const string ItemLinkEndPattern = "_z=z";
        protected const int GuidLength = 32;
        protected const string MediaHandlerExtension = "ashx";
        protected readonly Database Database = Factory.GetDatabase(UMTSettings.Database);
        
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var fieldValue = field?.Value;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = ReplaceItemLinks(fieldValue, UrlOptions.DefaultOptions);
                fieldValue = ReplaceMediaLinks(fieldValue);
            }
            return new TargetFieldValue(fieldValue);
        }

        protected virtual string ReplaceItemLinks(string fieldValue, UrlOptions urlOptions)
        {
            var linkStartIndex = fieldValue.IndexOf(ItemLinkStartPattern, StringComparison.OrdinalIgnoreCase);
            if (linkStartIndex == -1)
                return fieldValue;
            var stringBuilder = new StringBuilder(fieldValue.Length);
            var searchStartIndex = 0;
            for (; linkStartIndex >= 0; linkStartIndex = fieldValue.IndexOf(ItemLinkStartPattern, searchStartIndex, StringComparison.OrdinalIgnoreCase))
            {
                var linkEndIndex = fieldValue.IndexOf(ItemLinkEndPattern, linkStartIndex, StringComparison.InvariantCulture);
                if (linkEndIndex < 0)
                {
                    return stringBuilder.ToString();
                }
                var url = DynamicLink.Parse(fieldValue.Substring(linkStartIndex, linkEndIndex - linkStartIndex)).GetUrl(urlOptions);
                var textBeforeLink = fieldValue.Substring(searchStartIndex, linkStartIndex - searchStartIndex);
                stringBuilder.Append(textBeforeLink);
                stringBuilder.Append(url);
                searchStartIndex = linkEndIndex + ItemLinkEndPattern.Length;
            }
            stringBuilder.Append(fieldValue.Substring(searchStartIndex));
            return stringBuilder.ToString();
        }

        protected virtual string ReplaceMediaLinks(string fieldValue)
        {
            var stringBuilder = new StringBuilder();
            var searchStartIndex = 0;
            var firstMediaIndex = this.FindFirstMediaPrefix(fieldValue, 0, out var prefix);
            while (firstMediaIndex >= 0 && fieldValue.Length >= firstMediaIndex + prefix.Length + GuidLength)
            {
                var mediaId = fieldValue.Substring(firstMediaIndex + prefix.Length, GuidLength);
                if (!ShortID.IsShortID(mediaId))
                {
                    firstMediaIndex = FindFirstMediaPrefix(fieldValue, firstMediaIndex + 1, out prefix);
                }
                else
                {
                    MediaItem mediaItem = null;
                    if (Database != null)
                        mediaItem = Database.GetItem(new ID(mediaId));
                    if (mediaItem == null)
                    {
                        firstMediaIndex = FindFirstMediaPrefix(fieldValue, firstMediaIndex + 1, out prefix);
                    }
                    else
                    {
                        var mediaUrl = string.Format(UMTSettings.RichTextMediaLinkFormat,
                            mediaItem.ID.Guid.ToString("D"), mediaItem.Name, mediaItem.Extension);
                        var extensionStartIndex = firstMediaIndex + prefix.Length + GuidLength + 1;
                        var usesExtension = string.Equals(
                            fieldValue.Substring(extensionStartIndex, MediaHandlerExtension.Length),
                            MediaHandlerExtension,
                            StringComparison.OrdinalIgnoreCase);
                        stringBuilder.Append(fieldValue.Substring(searchStartIndex, firstMediaIndex - searchStartIndex));
                        searchStartIndex = firstMediaIndex + prefix.Length + GuidLength;
                        if (usesExtension)
                        {
                            searchStartIndex += MediaHandlerExtension.Length + 1;
                        }
                        
                        stringBuilder.Append(mediaUrl);
                        firstMediaIndex = FindFirstMediaPrefix(fieldValue, searchStartIndex, out prefix);
                    }
                }
            }

            stringBuilder.Append(fieldValue.Substring(searchStartIndex));
            return stringBuilder.ToString();
        }
        
        protected virtual int FindFirstMediaPrefix(string text, int startIndex, out string prefix)
        {
            var firstMediaPrefixIndex = int.MaxValue;
            prefix = null;
            foreach (var mediaPrefix in MediaManager.Config.MediaPrefixes)
            {
                var foundMediaPrefixIndex = text.IndexOf(mediaPrefix, startIndex, StringComparison.OrdinalIgnoreCase);
                if (foundMediaPrefixIndex >= 0 && foundMediaPrefixIndex < firstMediaPrefixIndex)
                {
                    firstMediaPrefixIndex = foundMediaPrefixIndex;
                    prefix = mediaPrefix;
                }
            }
            return prefix == null ? -1 : firstMediaPrefixIndex;
        }
    }
}
