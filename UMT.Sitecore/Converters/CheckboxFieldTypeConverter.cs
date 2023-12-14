using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Converters
{
    public class CheckboxFieldTypeConverter : BaseFieldTypeConverter
    {
        public override object Convert(Field field, Item item)
        {
            var checkboxField = (CheckboxField)field;
            return (object)(checkboxField?.Checked ?? false);
        }
    }
}
