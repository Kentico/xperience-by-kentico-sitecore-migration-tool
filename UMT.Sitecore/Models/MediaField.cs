using System;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class MediaField
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public MediaFieldDimensions Dimensions { get; set; }
    }

    [Serializable]
    public class MediaFieldDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}