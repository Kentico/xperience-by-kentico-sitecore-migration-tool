using System.Collections.Generic;

namespace UMT.Sitecore.Models
{
    public class TargetFieldValue
    {
        public object Value { get; set; }
        public List<ContentItemReference> References { get; set; }

        public TargetFieldValue()
        {
        }

        public TargetFieldValue(object value)
        {
            Value = value;
        }
    }
}