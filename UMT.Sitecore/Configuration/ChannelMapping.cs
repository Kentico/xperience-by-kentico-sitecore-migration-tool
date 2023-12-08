using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Xml;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Configuration
{
    public class ChannelMapping
    {
        public List<Channel> Channels { get; set; }

        public ChannelMapping()
        {
            Channels = new List<Channel>();
        }

        public void AddChannel(XmlNode node)
        {
            var name = XmlUtil.GetAttribute("name", node);
            var displayName = XmlUtil.GetAttribute("displayName", node);
            int.TryParse(XmlUtil.GetAttribute("channelType", node), out int channelType);
            if (Guid.TryParse(XmlUtil.GetAttribute("id", node), out var id))
            {
                var channel = new Channel
                {
                    ChannelGUID = id,
                    ChannelName = name,
                    ChannelDisplayName = displayName,
                    ChannelType = channelType
                };
                Channels.Add(channel);
            }
        }
    }
}