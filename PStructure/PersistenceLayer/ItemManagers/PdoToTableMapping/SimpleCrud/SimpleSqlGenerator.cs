using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using PStructure.Models;
using PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds.Crud;
using PStructure.PersistenceLayer.PdoData;
using PStructure.TableLocation;

namespace PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator
{
    /// <summary>
    /// Generates standard CRUD SQL commands for items of type <typeparamref name="T"/>.
    /// </summary>
    public class SimpleSqlGenerator<T> : SqlGenerator<T>
    {
        
        // SQL command cache for each type
        private static readonly ConcurrentDictionary<string, string> _sqlCache 
            = new ConcurrentDictionary<string, string>();

        public SimpleSqlGenerator(ITableLocation tableLocation) : base(tableLocation) {}
        
        /// <summary>
        /// Generates the INSERT SQL command for a PDO.
        /// </summary>
        public new string GetInsertSql(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Insert", _ =>
            {
                var columnNames = PdoMetadata<T>.Properties
                    .Select(prop => prop.GetCustomAttribute<Column>().ColumnName);
                var parameterNames = PdoMetadata<T>.Properties
                    .Select(prop => "@" + prop.GetCustomAttribute<Column>().ColumnName);
                var sql = $"INSERT INTO {GetTableLocation()} ({string.Join(", ", columnNames)}) " +
                          $"VALUES ({string.Join(", ", parameterNames)})";

                LogGeneratedSql(logger, sql, "Insert");
                return sql;
            });
        }

        /// <summary>
        /// Generates the READ SQL command by primary key.
        /// </summary>
        public new string GetReadSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Read", _ =>
            {
                var whereClauses = PdoMetadata<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"SELECT * FROM {GetTableLocation()} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Read");
                return sql;
            });
        }

        /// <summary>
        /// Generates the DELETE SQL command by primary key.
        /// </summary>
        public new string GetDeleteSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Delete", _ =>
            {
                var whereClauses = PdoMetadata<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"DELETE FROM {GetTableLocation()} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Delete");
                return sql;
            });
        }

        /// <summary>
        /// Generates the UPDATE SQL command by primary key.
        /// </summary>
        public new string GetUpdateSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Update", _ =>
            {
                var setClauses = PdoMetadata<T>.Properties
                    .Except(PdoMetadata<T>.PrimaryKeyProperties)
                    .Select(prop =>
                    {
                        var columnAttr = prop.GetCustomAttribute<Column>();
                        var columnName = columnAttr?.ColumnName ?? prop.Name;
                        return $"{columnName} = @{columnName}";
                    });

                var whereClauses = PdoMetadata<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"UPDATE {GetTableLocation()} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Update");
                return sql;
            });
        }

        /// <summary>
        /// Generates the SQL command to read all records.
        /// </summary>
        public new string GetReadAll(ILogger logger)
        {
            var sql = $"SELECT * FROM {GetTableLocation()}";
            LogGeneratedSql(logger, sql, "ReadAll");
            return sql;
        }
    }
}
