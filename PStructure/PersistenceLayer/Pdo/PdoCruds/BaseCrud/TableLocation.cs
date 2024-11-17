using System;
using PStructure.PersistenceLayer;
using PStructure.TableLocation;

namespace PStructure.Models
{
    /// <summary>
    /// Defines the location of a database table for a specific work mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TableLocation : Attribute, ITableLocation
    {
        public WorkMode Mode { get; }
        public string Schema { get; }
        public string TableName { get; }

        public TableLocation(WorkMode mode, string schema, string tableName)
        {
            Mode = mode;
            Schema = schema;
            TableName = tableName;
        }

        public string PrintTableLocation()
        {
            return $"{Schema}.{TableName} ";
        }
    }
}