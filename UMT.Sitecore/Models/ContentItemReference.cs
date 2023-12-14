using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItemReference : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentItemReference";
        public Guid ContentItemReferenceGUID { get; set; }
        public Guid ContentItemReferenceSourceCommonDataGuid { get; set; }
        public Guid ContentItemReferenceTargetItemGuid { get; set; }
        public Guid ContentItemReferenceGroupGUID { get; set; } //Guid of the reference field
    }
}