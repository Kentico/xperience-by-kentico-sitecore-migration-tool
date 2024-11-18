using System;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class AssetFileSource : BaseAssetSource
    {
        public override string Type => "AssetFile";
        public string FilePath { get; set; }
    }
}