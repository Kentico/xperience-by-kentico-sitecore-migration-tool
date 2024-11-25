using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentFolder : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentFolder";
        public Guid ContentFolderGUID { get; set; }
        public Guid? ContentFolderParentFolderGUID { get; set; }
        public string ContentFolderName { get; set; }
        public string ContentFolderDisplayName { get; set; }
        public string ContentFolderTreePath { get; set; }
    }
}