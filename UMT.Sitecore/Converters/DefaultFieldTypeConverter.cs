using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Converters
{
    public class DefaultFieldTypeConverter : BaseFieldTypeConverter
    {
        public override object Convert(Field field, Item item)
        {
            return field != null ? field.Value : string.Empty;
        }
    }
}
