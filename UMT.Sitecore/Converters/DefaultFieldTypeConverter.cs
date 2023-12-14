using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class DefaultFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            return new TargetFieldValue(field != null ? field.Value : string.Empty);
        }
    }
}
