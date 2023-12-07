using Sitecore.Data.Items;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Converters
{
    public class DefaultFieldTypeConverter : IFieldTypeConverter
    {
        public virtual string Convert(Item item, string fieldName)
        {
            return item != null ? item[fieldName] : string.Empty;
        }
    }
}
