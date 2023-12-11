using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItem : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentItem";
        public Guid ContentItemGUID { get; set; }
        public string ContentItemName { get; set; }
        public bool ContentItemIsReusable { get; set; }
        public bool ContentItemIsSecured { get; set; }
        public Guid ContentItemDataClassGuid { get; set; }
        public Guid ContentItemChannelGuid { get; set; }
    }
}