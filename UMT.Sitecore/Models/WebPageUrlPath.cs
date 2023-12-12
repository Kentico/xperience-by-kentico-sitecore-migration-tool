using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class WebPageUrlPath : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "WebPageUrlPath";
        public Guid WebPageUrlPathGUID { get; set; }
        [JsonProperty(PropertyName = "WebPageUrlPath")]
        public string WebPageUrlPathPath { get; set; }
        public string WebPageUrlPathHash { get; set; }
        public Guid WebPageUrlPathWebPageItemGuid { get; set; }
        public Guid WebPageUrlPathWebsiteChannelGuid { get; set; }
        public Guid WebPageUrlPathContentLanguageGuid { get; set; }
        public bool WebPageUrlPathIsLatest { get; set; }
        public bool WebPageUrlPathIsDraft { get; set; }
    }
}