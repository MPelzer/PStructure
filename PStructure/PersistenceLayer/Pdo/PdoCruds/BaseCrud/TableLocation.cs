using System;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud
{
    /// <summary>
    ///     Defines the location of a database table for a specific work mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TableLocation : ITableLocation
    {
        public TableLocation(WorkMode mode, string schema, string tableName)
        {
            Mode = mode;
            Schema = schema;
            TableName = tableName;
        }

        public WorkMode Mode { get; }
        public string Schema { get; }
        public string TableName { get; }

        public string PrintTableLocation()
        {
            return $"{Schema}.{TableName} ";
        }
    }
}