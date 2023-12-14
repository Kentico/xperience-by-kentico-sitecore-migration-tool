using System;
using System.Collections.Generic;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class DataClassFieldSettings
    {
        public string ControlName { get; set; }
        public List<Guid> AllowedContentItemTypeIdentifiers { get; set; }
    }
}
