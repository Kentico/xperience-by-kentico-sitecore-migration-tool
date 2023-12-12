using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Converters
{
    public class CheckboxFieldTypeConverter : IFieldTypeConverter
    {
        public virtual object Convert(Field field, Item item)
        {
            var checkboxField = (CheckboxField)field;
            return (object)(checkboxField?.Checked ?? false);
        }
    }
}
