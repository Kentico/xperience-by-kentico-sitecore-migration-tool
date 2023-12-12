using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class WebPageItem : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "WebPageItem";
        public Guid WebPageItemGUID { get; set; }
        public Guid WebPageItemParentGuid { get; set; }
        public string WebPageItemName { get; set; }
        public string WebPageItemTreePath { get; set; }
        public Guid WebPageItemWebsiteChannelGuid { get; set; }
        public Guid WebPageItemContentItemGuid { get; set; }
        public int WebPageItemOrder { get; set; }
    }
}