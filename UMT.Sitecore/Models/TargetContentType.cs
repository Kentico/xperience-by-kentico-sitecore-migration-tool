using System;

namespace UMT.Sitecore.Models
{
    public class TargetContentType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public DataClass ContentType { get; set; }
        public ContentTypeChannel ContentTypeChannel { get; set; }
    }
}