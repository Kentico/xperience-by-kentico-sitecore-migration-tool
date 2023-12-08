using System;
using Newtonsoft.Json;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentLanguage
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentLanguage";
        public Guid ContentLanguageGUID { get; set; }
        public string ContentLanguageDisplayName { get; set; }
        public string ContentLanguageName { get; set; }
        public bool ContentLanguageIsDefault { get; set; }
        public Guid ContentLanguageFallbackContentLanguageGuid { get; set; }
        public string ContentLanguageCultureFormat { get; set; }
    }
}