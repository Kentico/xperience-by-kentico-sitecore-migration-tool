using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class DateTimeFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var dateField = (DateField)field;
            return new TargetFieldValue(dateField != null ? dateField.DateTime : (object)null);
        }
    }
}
