using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class DataClass : ITargetItemElement
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "DataClass";
        public string ClassDisplayName { get; set; }
        public string ClassName { get; set; }
        public string ClassTableName { get; set; }
        public bool ClassShowTemplateSelection { get; set; }
        public DateTime ClassLastModified { get; set; }
        public Guid ClassGUID { get; set; }
        public bool ClassHasUnmanagedDbSchema { get; set; }
        public string ClassType { get; set; }
        public string ClassContentTypeType { get; set; }
        public bool ClassWebPageHasUrl { get; set; }
        public List<DataClassField> Fields { get; set; }
        
    }
}
