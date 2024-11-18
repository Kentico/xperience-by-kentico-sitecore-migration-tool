using System;
using System.Collections.Generic;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class DataClassFieldSettings
    {
        public string ControlName { get; set; }
        public string AllowedExtensions { get; set; }
        public List<Guid> AllowedContentItemTypeIdentifiers { get; set; }
        public int MaximumPages { get; set; }
        public string TreePath { get; set; }
        public bool Sortable { get; set; }
    }
}
