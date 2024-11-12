using System;
using Newtonsoft.Json;

namespace UMT.Sitecore.Models
{
    public abstract class BaseAssetSource
    {
        [JsonProperty(PropertyName = "$assetType")] 
        public abstract string Type { get; }
        
        public Guid? ContentItemGuid { get; set; }
        public Guid? Identifier { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime? LastModified { get; set; }
    }
}