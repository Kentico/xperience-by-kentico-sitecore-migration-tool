using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Reflection;
using Sitecore.Xml;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Configuration
{
    public class FieldTypeMapping
    {
        public List<FieldTypeMap> FieldTypeMaps { get; }

        public FieldTypeMapping()
        {
            FieldTypeMaps = new List<FieldTypeMap>();
        }

        public void AddFieldType(XmlNode node)
        {
            var typeName = XmlUtil.GetAttribute("typeName", node);
            var columnType = XmlUtil.GetAttribute("columnType", node);
            int.TryParse(XmlUtil.GetAttribute("columnType", node), out var columnSize);
            var controlName = XmlUtil.GetAttribute("controlName", node);
            var converterTypeName = XmlUtil.GetAttribute("type", node);
            var converter = ReflectionUtil.CreateObject(converterTypeName) as IFieldTypeConverter;

            var fieldTypeConverter = new FieldTypeMap
            {
                TypeName = typeName,
                ColumnType = columnType,
                ColumnSize = columnSize,
                ControlName = controlName,
                TypeConverter = converter
            };

            FieldTypeMaps.Add(fieldTypeConverter);
        }

        public FieldTypeMap GetByFieldType(string fieldType)
        {
            return FieldTypeMaps.FirstOrDefault(x => x.TypeName.Equals(fieldType, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class FieldTypeMap
    {
        public string TypeName { get; set; }
        public string ColumnType { get; set; }
        public int ColumnSize { get; set; }
        public string ControlName { get; set; }
        public IFieldTypeConverter TypeConverter { get; set; }
    }
}
