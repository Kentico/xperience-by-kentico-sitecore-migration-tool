using System;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class WebPageReferenceField : IReferenceField
    {
        public WebPageReferenceField(Guid webPageGuid)
        {
            WebPageGuid = webPageGuid;
        }

        public Guid WebPageGuid { get; set; }
    }
}