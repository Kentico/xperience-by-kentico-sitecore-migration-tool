using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Xml;

namespace UMT.Sitecore.Configuration
{
    public class ChannelMapping
    {
        public List<ChannelMap> ChannelMaps { get; }

        public ChannelMapping()
        {
            ChannelMaps = new List<ChannelMap>();
        }

        public void AddChannel(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            var displayName = XmlUtil.GetAttribute("displayName", node);
            int.TryParse(XmlUtil.GetAttribute("channelType", node), out int channelType);
            var sitecoreSiteName = XmlUtil.GetAttribute("sitecoreSiteName", node);
            Guid.TryParse(XmlUtil.GetAttribute("websiteId", node), out var websiteId);
            var domain = XmlUtil.GetAttribute("domain", node); 
            var homePage = XmlUtil.GetAttribute("homePage", node); 
            Guid.TryParse(XmlUtil.GetAttribute("language", node), out var primaryLanguage);
            int.TryParse(XmlUtil.GetAttribute("defaultCookieLevel", node), out int defaultCookieLevel);
            bool.TryParse(XmlUtil.GetAttribute("storeFormerUrls", node), out bool storeFormerUrls);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                var channel = new ChannelMap
                {
                    Id = id,
                    Name = name,
                    DisplayName = displayName,
                    ChannelType = channelType,
                    SitecoreSiteName = sitecoreSiteName,
                    WebsiteId = websiteId,
                    Domain = domain,
                    HomePage = homePage,
                    PrimaryLanguage = primaryLanguage,
                    DefaultCookieLevel = defaultCookieLevel,
                    StoreFormerUrls = storeFormerUrls
                };
                ChannelMaps.Add(channel);
            }
        }
    }
    
    public class ChannelMap
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int ChannelType { get; set; }
        public string SitecoreSiteName { get; set; }
        
        //properties required for WebsiteChannel model
        public Guid WebsiteId { get; set; }
        public string Domain { get; set; }
        public string HomePage { get; set; }
        public Guid PrimaryLanguage { get; set; }
        public int DefaultCookieLevel { get; set; }
        public bool StoreFormerUrls { get; set; }
    }
}