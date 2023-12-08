using Sitecore.Configuration;

namespace UMT.Sitecore.Configuration
{
    public static class UMTConfigurationManager
    {
        public static FieldTypeMapping FieldTypeMapping { get; }
        public static FieldMapping FieldMapping { get; }
        public static ChannelMapping ChannelMapping { get; }

        static UMTConfigurationManager()
        {
            FieldTypeMapping = Factory.CreateObject("umt/fieldTypeMapping", true) as FieldTypeMapping;
            FieldMapping = Factory.CreateObject("umt/fieldMapping", true) as FieldMapping;
            ChannelMapping = Factory.CreateObject("umt/channelMapping", true) as ChannelMapping;
        }
    }
}
