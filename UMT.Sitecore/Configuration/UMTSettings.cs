using Sitecore.Configuration;

namespace UMT.Sitecore.Configuration
{
    public static class UMTSettings
    {
        public static string Database => Settings.GetSetting("UMT.Database");

        public static string DataFolder => Settings.GetSetting("UMT.DataFolder");

        public static string DataFolderDateFormat => Settings.GetSetting("UMT.DataFolderDateFormat");
    }
}
