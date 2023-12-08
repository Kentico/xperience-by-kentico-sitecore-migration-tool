using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Xml;

namespace UMT.Sitecore.Configuration
{
    public class FieldMapping
    {
        public Dictionary<Guid, string> ExcludedFields { get; set; }

        public FieldMapping()
        {
            ExcludedFields = new Dictionary<Guid, string>();
        }

        public void AddExcludedField(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                ExcludedFields.Add(id, name);
            }
        }

        public bool ShouldBeExcluded(Guid fieldId)
        {
            return ExcludedFields.ContainsKey(fieldId);
        }
    }
}
