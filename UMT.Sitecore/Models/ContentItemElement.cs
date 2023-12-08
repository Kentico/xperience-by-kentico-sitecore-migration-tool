using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItemElement : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentItem";
        public Guid ContentItemGUID { get; set; }
        public string ContentItemName { get; set; }
        public bool ContentItemIsReusable { get; set; }
        public bool ContentItemIsSecured { get; set; }
        public Guid ContentItemDataClassGuid { get; set; }
        public Guid ContentItemChannelGuid { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> Properties { get; set; }

        public string GetName()
        {
            return ContentItemName;
        }

        public Guid GetId()
        {
            return ContentItemGUID;
        }
    }
}