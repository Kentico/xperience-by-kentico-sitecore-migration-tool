using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Converters
{
    public class DateTimeFieldTypeConverter : BaseFieldTypeConverter
    {
        public override object Convert(Field field, Item item)
        {
            var dateField = (DateField)field;
            return dateField != null ? dateField.DateTime : (object)null;
        }
    }
}
