using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Converters
{
    public class RichTextFieldTypeConverter : IFieldTypeConverter
    {
        public string Convert(Item item, string fieldName)
        {
            return FieldRenderer.Render(item, fieldName);
        }
    }
}
