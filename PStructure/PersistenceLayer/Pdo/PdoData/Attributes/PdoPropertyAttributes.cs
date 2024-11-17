using System;

namespace PStructure.PersistenceLayer.Pdo.PdoData.Attributes
{
    public class PdoPropertyAttributes
    {
        /// <summary>
        /// Indicates that the PDO property is part of the primary key for the table representation.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class PrimaryKey : Attribute
        {
        }
        
        /// <summary>
        /// Specifies a custom type handler for transformation between PDO properties and database table values.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class TypeHandler : Attribute
        {
            public Type HandlerType { get; }

            public TypeHandler(Type handlerType)
            {
                HandlerType = handlerType;
            }
        }
        
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
}