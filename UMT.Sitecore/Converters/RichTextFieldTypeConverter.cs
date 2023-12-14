using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public class RichTextFieldTypeConverter : BaseFieldTypeConverter
    {
        public override TargetFieldValue Convert(Field field, Item item)
        {
            return new TargetFieldValue(field != null ? FieldRenderer.Render(item, field.Name) : string.Empty);
        }
    }
}
