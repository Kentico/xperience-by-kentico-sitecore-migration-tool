using System;
using Newtonsoft.Json;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class MediaLibrary
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "Media_Library";
        public string LibraryName { get; set; }
        public string LibraryDisplayName { get; set; }
        public string LibraryDescription { get; set; }
        public string LibraryFolder { get; set; }
        public Guid LibraryGUID { get; set; }
        public DateTime LibraryLastModified { get; set; }
    }
}