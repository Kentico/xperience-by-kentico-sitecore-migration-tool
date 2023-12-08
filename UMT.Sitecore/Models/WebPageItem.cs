using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class WebPageItem : ITargetItem
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type { get; set; }
        public Guid WebPageItemGUID { get; set; }
        public Guid WebPageItemParentGuid { get; set; }
        public string WebPageItemName { get; set; }
        public string WebPageItemTreePath { get; set; }
        public Guid WebPageItemWebsiteChannelGuid { get; set; }
        public Guid WebPageItemContentItemGuid { get; set; }
        public int WebPageItemOrder { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> Properties { get; set; }

        public string GetName()
        {
            return WebPageItemName;
        }

        public Guid GetId()
        {
            return WebPageItemGUID;
        }
    }
}