using Sitecore.Configuration;

namespace UMT.Sitecore.Configuration
{
    public static class UMTConfigurationManager
    {
        public static FieldTypeMapping FieldTypeMapping { get; }

        static UMTConfigurationManager()
        {
            FieldTypeMapping = Factory.CreateObject("umt/fieldTypeMapping", true) as FieldTypeMapping;
        }
    }
}
