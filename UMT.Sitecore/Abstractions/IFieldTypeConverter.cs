using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Abstractions
{
    public interface IFieldTypeConverter
    {
        object Convert(Field field, Item item);
    }
}
