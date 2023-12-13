using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentTypeChannel: ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")] 
        public string Type => "ContentTypeChannel";
        public Guid ContentTypeChannelChannelGuid { get; set; }
        public Guid ContentTypeChannelContentTypeGuid { get; set; }
    }
}