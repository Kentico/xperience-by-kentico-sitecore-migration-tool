using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UMT.Sitecore.Configuration
{
    public class ContentMapping
    {
        public List<string> PageRoots { get; }

        public ContentMapping()
        {
            PageRoots = new List<string>();
        }

        public void AddPageRoot(XmlNode node)
        {
            var path = XmlUtil.GetAttribute("path", node);
            PageRoots.Add(path);
        }

        public bool IsUnderPageRoot(string path)
        {
            return PageRoots.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
