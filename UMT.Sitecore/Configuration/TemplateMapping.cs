using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Xml;

namespace UMT.Sitecore.Configuration
{
    public class TemplateMapping
    {
        public Dictionary<Guid, string> ExcludedTemplates { get; }
        public Dictionary<Guid, string> ContentHubTemplates { get; }

        public TemplateMapping()
        {
            ExcludedTemplates = new Dictionary<Guid, string>();
            ContentHubTemplates = new Dictionary<Guid, string>();
        }

        public void AddExcludedTemplate(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                ExcludedTemplates.Add(id, name);
            }
        }

        public void AddContentHubTemplate(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                ContentHubTemplates.Add(id, name);
            }
        }

        public bool ShouldBeExcluded(Guid templateId)
        {
            return ExcludedTemplates.ContainsKey(templateId);
        }
        
        public bool IsContentHubTemplate(Guid fieldId)
        {
            return ContentHubTemplates.ContainsKey(fieldId);
        }
    }
}
