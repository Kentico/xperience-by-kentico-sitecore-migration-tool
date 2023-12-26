using Sitecore.Configuration;

namespace UMT.Sitecore.Configuration
{
    public static class UMTSettings
    {
        public static string Database => Settings.GetSetting("UMT.Database");

        public static string DataFolder => Settings.GetSetting("UMT.DataFolder");

        public static string DataFolderDateFormat => Settings.GetSetting("UMT.DataFolderDateFormat");
        
        public static bool ExportMediaAsUrls => Settings.GetBoolSetting("UMT.ExportMediaAsUrls", false);
        
        public static string ExportMediaAsUrlsServerUrl => Settings.GetSetting("UMT.ExportMediaAsUrls.ServerUrl");
        
        public static int MaxFilePathLength => Settings.GetIntSetting("UMT.MaxFilePathLength", 260);
        
        public static string MediaLocationForExport => Settings.GetSetting("UMT.MediaLocationForExport");
        
        public static string MediaLocationForJson => Settings.GetSetting("UMT.MediaLocationForJson");

        public static bool TrimLongMediaFolderPaths => Settings.GetBoolSetting("UMT.TrimLongMediaFolderPaths", true);
    }
}
