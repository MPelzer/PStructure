using System;

namespace PStructure.TableLocation
{
    /// <summary>
    /// A factory class for creating instances of BaseTableLocation.
    /// </summary>
    public static class TableLocationFactory<T>
    {
        /// <summary>
        /// Creates an instance of BaseTableLocation with the specified schema and table name.
        /// </summary>
        /// <param name="schema">The schema where the table is located. Can be null or empty if no schema is required.</param>
        /// <param name="tableName">The name of the table in the database.</param>
        /// <returns>A new instance of BaseTableLocation.</returns>
        public static ITableLocation CreateTableLocation(string schema, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name must not be null or empty.", nameof(tableName));
                
            return new BaseTableLocation(schema, tableName);
        }

        /// <summary>
        /// Creates an instance of BaseTableLocation for a default schema.
        /// </summary>
        /// <param name="tableName">The name of the table in the default schema.</param>
        /// <returns>A new instance of BaseTableLocation with the default schema.</returns>
        public static ITableLocation CreateLiveSchemaTableLocation(string tableName)
        {
            const string defaultSchema = "dbo";  // Example default schema, can be customized.
            return CreateTableLocation(defaultSchema, tableName);
        }
        /// <summary>
        /// Creates an instance of BaseTableLocation for a test schema.
        /// </summary>
        /// <param name="tableName">The name of the table in the default schema.</param>
        /// <returns>A new instance of BaseTableLocation with the default schema.</returns>
        public static ITableLocation CreateTestSchemaTableLocation(string tableName)
        {
            const string defaultSchema = "dbo";  // Example default schema, can be customized.
            return CreateTableLocation(defaultSchema, tableName);
        }
    }
}