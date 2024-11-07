using System;

namespace PStructure.Models
{
    /// <summary>
    /// Specifies the name of the database column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Column : Attribute
    {
        public string ColumnName { get; }

        public Column(string columnName)
        {
            ColumnName = columnName;
        }
    }
}