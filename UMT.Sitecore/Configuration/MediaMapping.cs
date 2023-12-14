using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Xml;

namespace UMT.Sitecore.Configuration
{
    public class MediaMapping
    {
        public List<MediaMap> MediaMaps { get; }

        public MediaMapping()
        {
            MediaMaps = new List<MediaMap>();
        }

        public void AddMediaLibrary(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            var displayName = XmlUtil.GetAttribute("displayName", node);
            var description = XmlUtil.GetAttribute("description", node); 
            var libraryFolder = XmlUtil.GetAttribute("libraryFolder", node);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                var mediaMap = new MediaMap
                {
                    Id = id,
                    Name = name,
                    DisplayName = displayName,
                    Description = description,
                    LibraryFolder = libraryFolder
                };
                MediaMaps.Add(mediaMap);
            }
        }
    }
    
    public class MediaMap
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string LibraryFolder { get; set; }
    }
}