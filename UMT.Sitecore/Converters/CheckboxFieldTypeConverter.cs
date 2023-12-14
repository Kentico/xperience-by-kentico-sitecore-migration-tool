using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class CheckboxFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            var checkboxField = (CheckboxField)field;
            return new TargetFieldValue((object)(checkboxField?.Checked ?? false));
        }
    }
}
