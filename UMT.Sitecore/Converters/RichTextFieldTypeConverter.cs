using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace UMT.Sitecore.Converters
{
    public class RichTextFieldTypeConverter : BaseFieldTypeConverter
    {
        public override object Convert(Field field, Item item)
        {
            return field != null ? FieldRenderer.Render(item, field.Name) : string.Empty;
        }
    }
}
