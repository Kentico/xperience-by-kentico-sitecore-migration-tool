using Sitecore.Configuration;

namespace UMT.Sitecore.Configuration
{
    public static class UMTConfigurationManager
    {
        public static FieldTypeMapping FieldTypeMapping { get; }
        public static FieldMapping FieldMapping { get; }

        static UMTConfigurationManager()
        {
            FieldTypeMapping = Factory.CreateObject("umt/fieldTypeMapping", true) as FieldTypeMapping;
            FieldMapping = Factory.CreateObject("umt/fieldMapping", true) as FieldMapping;
        }
    }
}
