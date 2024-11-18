using System;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class AssetUrlSource : BaseAssetSource
    {
        public override string Type => "AssetUrl";
        public string Url { get; set; }
    }
}