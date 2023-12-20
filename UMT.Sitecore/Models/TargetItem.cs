using System;
using System.Collections.Generic;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    public class TargetItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DepthLevel { get; set; }
        public bool IsWebPage { get; set; }
        public List<ITargetItemElement> Elements { get; set; } = new List<ITargetItemElement>();
    }
}