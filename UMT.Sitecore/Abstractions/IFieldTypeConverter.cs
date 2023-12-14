using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Abstractions
{
    public interface IFieldTypeConverter
    {
        string DefaultColumnType { get; set; }
        int DefaultColumnSize { get; set; }
        string DefaultControlName { get; set; }
        string GetColumnType(TemplateField field);
        int GetColumnSize(TemplateField field);
        DataClassFieldSettings GetFieldSettings(TemplateField field);
        TargetFieldValue Convert(Field field, Item item);
    }
}
