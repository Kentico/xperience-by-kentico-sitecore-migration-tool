using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Xml;

namespace UMT.Sitecore.Configuration
{
    public class MediaMapping
    {
        public List<MediaTemplate> MediaTemplates { get; }

        public MediaMapping()
        {
            MediaTemplates = new List<MediaTemplate>();
        }
        
        public void AddMediaTemplate(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            var nameSpace = XmlUtil.GetAttribute("namespace", node);
            var fileExtensions = XmlUtil.GetAttribute("fileExtensions", node);
            bool.TryParse(XmlUtil.GetAttribute("defaultMediaTemplate", node), out var defaultMediaTemplate);
            bool.TryParse(XmlUtil.GetAttribute("imageTemplate", node), out var imageTemplate);
            var assetFieldName = XmlUtil.GetAttribute("assetFieldName", node); 
            Guid.TryParse(XmlUtil.GetAttribute("assetFieldId", node), out var assetFieldId);
            var altFieldName = XmlUtil.GetAttribute("altFieldName", node);
            Guid.TryParse(XmlUtil.GetAttribute("altFieldId", node), out var altFieldId);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                var mediaTemplate = new MediaTemplate
                {
                    Id = id,
                    Name = name,
                    Namespace = nameSpace,
                    FileExtensions = fileExtensions,
                    DefaultMediaTemplate = defaultMediaTemplate,
                    ImageTemplate = imageTemplate,
                    AssetFieldName = assetFieldName,
                    AssetFieldId = assetFieldId,
                    AltFieldName = altFieldName,
                    AltFieldId = altFieldId
                };
                MediaTemplates.Add(mediaTemplate);
            }
        }

        public MediaTemplate GetMediaTemplate(string fileExtension)
        {
            return MediaTemplates.FirstOrDefault(x => x.FileExtensions.IndexOf(fileExtension, StringComparison.OrdinalIgnoreCase) > 0 ||
                                                      x.DefaultMediaTemplate);
        }

        public MediaTemplate GetMediaTemplate(bool imageTemplate = false)
        {
            return MediaTemplates.FirstOrDefault(x => imageTemplate && x.ImageTemplate || x.DefaultMediaTemplate);
        }
    }
    
    public class MediaTemplate
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Guid Id { get; set; }
        public string FileExtensions { get; set; }
        public bool ImageTemplate { get; set; }
        public bool DefaultMediaTemplate { get; set; }
        public string AltFieldName { get; set; }
        public Guid AltFieldId { get; set; }
        public string AssetFieldName { get; set; }
        public Guid AssetFieldId { get; set; }
    }
}