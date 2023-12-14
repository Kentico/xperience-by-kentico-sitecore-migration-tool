using System;
using Newtonsoft.Json;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class MediaFile
    {
        [JsonProperty(PropertyName = "$type")]
        public string Type => "Media_File";
        public string DataSourcePath { get; set; }
        public Guid FileGUID { get; set; }
        public Guid FileLibraryGuid { get; set; }
        public Guid FileCreatedByUserGuid { get; set; }
        public Guid FileModifiedByUserGuid { get; set; }
        public string FileName { get; set; }
        public string FileTitle { get; set; }
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public string FileMimeType { get; set; }
        public string FilePath { get; set; }
        public int FileImageWidth { get; set; }
        public int FileImageHeight { get; set; }
        public DateTime FileCreatedWhen { get; set; }
        public DateTime FileModifiedWhen { get; set; }
    }
}