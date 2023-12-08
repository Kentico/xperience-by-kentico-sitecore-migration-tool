using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItemData : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentItemData";
        public Guid ContentItemDataGUID { get; set; }
        public Guid ContentItemDataCommonDataGuid { get; set; }
        public string ContentItemContentTypeName { get; set; }
    }
}