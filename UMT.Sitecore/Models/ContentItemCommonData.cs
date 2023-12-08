using System;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItemCommonData : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "ContentItemCommonData";
        public Guid ContentItemCommonDataGUID { get; set; }
        public Guid ContentItemCommonDataContentItemGuid { get; set; }
        public Guid ContentItemCommonDataContentLanguageGuid { get; set; }
        public int ContentItemCommonDataVersionStatus { get; set; }
        public bool ContentItemCommonDataIsLatest { get; set; }
        public string ContentItemCommonDataPageBuilderWidgets { get; set; }
        public string ContentItemCommonDataPageTemplateConfiguration { get; set; }
    }
}