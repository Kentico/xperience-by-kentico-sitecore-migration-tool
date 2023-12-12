using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Converters
{
    public class RichTextFieldTypeConverter : IFieldTypeConverter
    {
        public string Convert(Field field, Item item)
        {
            return field != null ? FieldRenderer.Render(item, field.Name) : string.Empty;
        }
    }
}
