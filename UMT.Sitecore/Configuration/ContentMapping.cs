﻿using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UMT.Sitecore.Configuration
{
    public class ContentMapping
    {
        public List<string> PageRoots { get; } = new List<string>();
        public List<string> ExcludedSubtrees { get; } = new List<string>();

        public void AddPageRoot(XmlNode node)
        {
            var path = XmlUtil.GetAttribute("path", node);
            PageRoots.Add(path);
        }

        public void AddExcludedSubtree(XmlNode node)
        {
            var path = XmlUtil.GetAttribute("path", node);
            if (!string.IsNullOrEmpty(path))
            {
                ExcludedSubtrees.Add(path);
            }
        }

        public bool PathContainsAnyPageRoot(string path)
        {
            return !string.IsNullOrEmpty(path) && PageRoots.Any(x => path.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        
        public bool ShouldBeExcluded(string path)
        {
            return ExcludedSubtrees.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
