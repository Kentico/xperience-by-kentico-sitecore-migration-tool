using System;

namespace UMT.Sitecore.Models
{
    [Serializable]
    public class DataClassField
    {
        public bool AllowEmpty { get; set; }
        public string Column { get; set; }
        public int ColumnSize { get; set; }
        public string ColumnType { get; set; }
        public bool Enabled { get; set; }
        public Guid Guid { get; set; }
        public bool Visible { get; set; }
        public DataClassFieldProperties Properties { get; set; }
        public DataClassFieldSettings Settings { get; set; }
    }
}
