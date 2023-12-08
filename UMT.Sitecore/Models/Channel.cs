using System;
using Newtonsoft.Json;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class Channel
    {
        [JsonProperty(PropertyName = "$type")] 
        public string Type => "Channel";
        public string ChannelDisplayName { get; set; }
        public string ChannelName { get; set; }
        public Guid ChannelGUID { get; set; }
        public int ChannelType { get; set; }
    }
}