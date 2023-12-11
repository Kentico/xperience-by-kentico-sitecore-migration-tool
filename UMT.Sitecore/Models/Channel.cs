using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class Channel : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")] 
        public string Type => "Channel";
        public string ChannelDisplayName { get; set; }
        public string ChannelName { get; set; }
        public Guid ChannelGUID { get; set; }
        public int ChannelType { get; set; }
    }
}