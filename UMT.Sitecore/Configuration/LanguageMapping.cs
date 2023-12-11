using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Xml;

namespace UMT.Sitecore.Configuration
{
    public class LanguageMapping
    {
        public List<LanguageMap> LanguageMaps { get; }

        public LanguageMapping()
        {
            LanguageMaps = new List<LanguageMap>();
        }

        public void AddLanguage(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("sourceId", node), out var sourceId) &&
                Guid.TryParse(XmlUtil.GetAttribute("targetId", node), out var targetId))
            {
                var language = new LanguageMap
                {
                    Name = name,
                    SourceId = sourceId,
                    TargetId = targetId
                };
                LanguageMaps.Add(language);
            }
        }
        
        public Guid GetTargetLanguageId(Guid sourceLanguageId)
        {
            return LanguageMaps.FirstOrDefault(x => x.SourceId == sourceLanguageId)?.TargetId ?? sourceLanguageId;
        }
    }
    
    public class LanguageMap
    {
        public string Name { get; set; }
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
    }
}