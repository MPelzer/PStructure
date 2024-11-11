using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using PStructure.Models;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator
{
    /// <summary>
    /// Generates standard CRUD SQL commands for items of type <typeparamref name="T"/>.
    /// </summary>
    public class SimpleSqlGenerator<T> : ISqlGenerator<T> where T : new()
    {
        
        // SQL command cache for each type
        private static readonly ConcurrentDictionary<string, string> _sqlCache 
            = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Generates the INSERT SQL command for a PDO.
        /// </summary>
        public string GetInsertSql(ILogger logger, string tableLocation)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Insert", _ =>
            {
                var columnNames = PdoProperties<T>.Properties
                    .Select(prop => prop.GetCustomAttribute<Column>().ColumnName);
                var parameterNames = PdoProperties<T>.Properties
                    .Select(prop => "@" + prop.GetCustomAttribute<Column>().ColumnName);

                var sql = $"INSERT INTO {tableLocation} ({string.Join(", ", columnNames)}) " +
                          $"VALUES ({string.Join(", ", parameterNames)})";

                LogGeneratedSql(logger, sql, "Insert");
                return sql;
            });
        }

        /// <summary>
        /// Generates the READ SQL command by primary key.
        /// </summary>
        public string GetReadSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Read", _ =>
            {
                var whereClauses = PdoProperties<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"SELECT * FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Read");
                return sql;
            });
        }

        /// <summary>
        /// Generates the DELETE SQL command by primary key.
        /// </summary>
        public string GetDeleteSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Delete", _ =>
            {
                var whereClauses = PdoProperties<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"DELETE FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Delete");
                return sql;
            });
        }

        /// <summary>
        /// Generates the UPDATE SQL command by primary key.
        /// </summary>
        public string GetUpdateSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Update", _ =>
            {
                var setClauses = PdoProperties<T>.Properties
                    .Except(PdoProperties<T>.PrimaryKeyProperties)
                    .Select(prop =>
                    {
                        var columnAttr = prop.GetCustomAttribute<Column>();
                        var columnName = columnAttr?.ColumnName ?? prop.Name;
                        return $"{columnName} = @{columnName}";
                    });

                var whereClauses = PdoProperties<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"UPDATE {tableLocation} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Update");
                return sql;
            });
        }

        /// <summary>
        /// Generates the SQL command to read all records.
        /// </summary>
        public string GetReadAll(ILogger logger, string tableLocation)
        {
            var sql = $"SELECT * FROM {tableLocation}";
            LogGeneratedSql(logger, sql, "ReadAll");
            return sql;
        }

        /// <summary>
        /// Logs the generated SQL command.
        /// </summary>
        public void LogGeneratedSql(ILogger logger, string sql, string commandType)
        {
            logger?.LogDebug("{Location} SQL of type {Type} generated: {Sql}", 
                PrintLocation(), commandType, sql);
        }

        /// <summary>
        /// Retrieves the formatted location string for logging purposes.
        /// </summary>
        private string PrintLocation() => $"[{typeof(T).Name}]";
    }
}
