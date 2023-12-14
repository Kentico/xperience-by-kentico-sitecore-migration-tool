using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Xml;

namespace UMT.Sitecore.Configuration
{
    public class TemplateMapping
    {
        public Dictionary<Guid, string> ExcludedTemplates { get; }

        public TemplateMapping()
        {
            ExcludedTemplates = new Dictionary<Guid, string>();
        }

        public void AddExcludedTemplate(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                ExcludedTemplates.Add(id, name);
            }
        }

        public bool ShouldBeExcluded(Guid fieldId)
        {
            return ExcludedTemplates.ContainsKey(fieldId);
        }
    }
}
