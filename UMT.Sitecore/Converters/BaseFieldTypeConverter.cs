using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using UMT.Sitecore.Abstractions;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Converters
{
    public abstract class BaseFieldTypeConverter : IFieldTypeConverter
    {
        public string DefaultColumnType { get; set; }
        public int DefaultColumnSize { get; set; }
        public string DefaultControlName { get; set; }

        public virtual string GetColumnType(TemplateField field)
        {
            return DefaultColumnType;
        }

        public virtual int GetColumnSize(TemplateField field)
        {
            return DefaultColumnSize;
        }

        public virtual DataClassFieldSettings GetFieldSettings(TemplateField field)
        {
            return new DataClassFieldSettings
            {
                ControlName = DefaultControlName
            };
        }

        public abstract object Convert(Field field, Item item);
    }
}