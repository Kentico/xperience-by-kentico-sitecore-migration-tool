using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Globalization;

namespace UMT.Sitecore.Configuration
{
    public static class UMTConfiguration
    {
        public static ChannelMapping ChannelMapping { get; }
        public static LanguageMapping LanguageMapping { get; }
        public static ContentMapping ContentMapping { get; }
        public static TemplateMapping TemplateMapping { get; }
        public static FieldTypeMapping FieldTypeMapping { get; }
        public static FieldMapping FieldMapping { get; }
        public static List<Language> SitecoreLanguages { get; }

        static UMTConfiguration()
        {
            ChannelMapping = Factory.CreateObject("umt/channelMapping", true) as ChannelMapping;
            LanguageMapping = Factory.CreateObject("umt/languageMapping", true) as LanguageMapping;
            ContentMapping = Factory.CreateObject("umt/contentMapping", true) as ContentMapping;
            TemplateMapping = Factory.CreateObject("umt/templateMapping", true) as TemplateMapping;
            FieldTypeMapping = Factory.CreateObject("umt/fieldTypeMapping", true) as FieldTypeMapping;
            FieldMapping = Factory.CreateObject("umt/fieldMapping", true) as FieldMapping;
            SitecoreLanguages = LanguageManager.GetLanguages(Factory.GetDatabase(UMTSettings.Database)).OrderBy(x => x.Name).ToList();
        }
    }
}
