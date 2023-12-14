using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class DecimalFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            return new TargetFieldValue((object)(decimal.TryParse(field.Value, out var number) ? number : 0));
        }
    }
}
