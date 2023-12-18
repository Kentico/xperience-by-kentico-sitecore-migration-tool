using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class LinkFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var linkField = (LinkField)field;
            var result = new TargetFieldValue();
            if (linkField != null && !string.IsNullOrEmpty(linkField.Value))
            {
                result.Value = linkField.GetFriendlyUrl();
            }

            return result;
        }
    }
}
