using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractContent
{
    public class SaveChannel : BaseSaveProcessor
    {
        public virtual void Process(ExtractContentArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceChannel, nameof(args.SourceChannel));

            UMTLog.Info($"{nameof(SaveChannel)} pipeline processor started");

            var targetChannel = GetTargetChannel(args.SourceChannel);
            SerializeChannel(targetChannel, args.OutputFolderPath);
            UMTLog.Info($"Channel {targetChannel.Name} ({targetChannel.Id}) saved", true);
            
            UMTLog.Info($"{nameof(SaveChannel)} pipeline processor finished");
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
        
        
        protected virtual void SerializeChannel(TargetChannel channel, string outputFolderPath)
        {
            var folderPath = CreateFileExtractFolder($"{outputFolderPath}/01.Configuration");
            var fileName = $"{folderPath}/02.Channel.{channel.Name}.{channel.Id:D}.json";
            SerializeToFile(channel.Elements, fileName);
        }
    }
}