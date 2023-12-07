using Sitecore.Data.Items;

namespace UMT.Sitecore.Abstractions
{
    public interface IFieldTypeConverter
    {
        string Convert(Item item, string fieldName);
    }
}
