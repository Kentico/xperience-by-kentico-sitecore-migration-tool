using System;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class ContentItemReferenceField : IReferenceField
    {
        public ContentItemReferenceField(Guid identifier)
        {
            Identifier = identifier;
        }

        public Guid Identifier { get; set; }
    }
}