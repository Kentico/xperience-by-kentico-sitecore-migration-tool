using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapChannel
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));

            UMTLog.Info($"{nameof(MapChannel)} pipeline processor started");

            args.TargetChannel = GetTargetChannel(args.SourceChannel);

            UMTLog.Info($"{nameof(MapChannel)} pipeline processor finished");
        }

        protected virtual TargetChannel GetTargetChannel(ChannelMap channelMap)
        {
            var targetChannel = new TargetChannel
            {
                Id = channelMap.Id,
                Name = channelMap.Name
            };
            targetChannel.Elements.Add(new Channel
            {
                ChannelGUID = channelMap.Id,
                ChannelName = channelMap.Name,
                ChannelDisplayName = channelMap.DisplayName,
                ChannelType = channelMap.ChannelType
            });
            targetChannel.Elements.Add(new WebSiteChannel
            {
                WebsiteChannelGUID = channelMap.WebsiteId,
                WebsiteChannelChannelGuid = channelMap.Id,
                WebsiteChannelDomain = channelMap.Domain,
                WebsiteChannelHomePage = channelMap.HomePage,
                WebsiteChannelDefaultCookieLevel = channelMap.DefaultCookieLevel,
                WebsiteChannelStoreFormerUrls = channelMap.StoreFormerUrls,
                WebsiteChannelPrimaryContentLanguageGuid = UMTConfiguration.LanguageMapping.GetTargetLanguageId(channelMap.PrimaryLanguage)
            });

            return targetChannel;
        }
    }
}