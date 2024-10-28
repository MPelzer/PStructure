using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using PStructure.Models;
using PStructure.Utils;

namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Generates standard CRUD SQL commands for items.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public class SimpleSqlGenerator<T> : ClassCore, ISqlGenerator<T> 
    {
        private enum SqlCommandType
        {
            Insert,
            Update,
            Read,
            Delete
        }

        /// <summary>
        /// Cache for SQL commands based on type and command type.
        /// </summary>
        private readonly ConcurrentDictionary<(Type Type, SqlCommandType CommandType), string> _sqlCache 
            = new ConcurrentDictionary<(Type, SqlCommandType), string>();

        public SimpleSqlGenerator(ILogger logger)
        {
            if (!PdoPropertyCache<T>.PrimaryKeyProperties.Any())
            {
                throw new InvalidOperationException($"{PrintLocation()} The PDO {typeof(T)} does not have any properties with a primary key attribute defined.");
            }

            logger?.LogDebug("{location}Start fetching property information for PDO {type}", PrintLocation(), typeof(T));
        }

        /// <summary>
        /// Generates the INSERT SQL command for a PDO.
        /// </summary>
        public string GetInsertSql(ILogger logger, string tableLocation)
        {
            var sql = _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Insert), _ =>
            {
                var columnNames = PdoPropertyCache<T>.Properties.Select(prop => prop.GetCustomAttribute<ColumnAttribute>().ColumnName);
                var parameterNames = PdoPropertyCache<T>.Properties.Select(prop => "@" + prop.GetCustomAttribute<ColumnAttribute>().ColumnName);

                return $"INSERT INTO {tableLocation} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", parameterNames)})";
            });
            LogGeneratedSql(logger, sql, SqlCommandType.Insert);
            return sql;
        }

        /// <summary>
        /// Generates the READ SQL command by primary key.
        /// </summary>
        public string GetReadSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Read), _ =>
            {
                if (!PdoPropertyCache<T>.PrimaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"{PrintLocation()} Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var whereClauses = PdoPropertyCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"SELECT * FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
            });
            LogGeneratedSql(logger, sql, SqlCommandType.Read);
            return sql;
        }

        /// <summary>
        /// Generates the DELETE SQL command by primary key.
        /// </summary>
        public string GetDeleteSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Delete), _ =>
            {
                if (!PdoPropertyCache<T>.PrimaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"{PrintLocation()} Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var whereClauses = PdoPropertyCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"DELETE FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
            });
            LogGeneratedSql(logger, sql, SqlCommandType.Delete);
            return sql;
        }

        /// <summary>
        /// Generates the UPDATE SQL command by primary key.
        /// </summary>
        public string GetUpdateSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Update), _ =>
            {
                if (!PdoPropertyCache<T>.PrimaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"{PrintLocation()} Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var setClauses = PdoPropertyCache<T>.Properties.Except(PdoPropertyCache<T>.PrimaryKeyProperties).Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var whereClauses = PdoPropertyCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"UPDATE {tableLocation} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
            });
            LogGeneratedSql(logger, sql, SqlCommandType.Update);
            return sql;
        }

        /// <summary>
        /// Reads the entire table for a PDO type.
        /// </summary>
        public string GetReadAll(ILogger logger, string tableLocation)
        {
            var sql = $"SELECT * FROM {tableLocation}";
            LogGeneratedSql(logger, sql, SqlCommandType.Read);
            return sql;
        }

        private void LogGeneratedSql(ILogger logger, string sql, SqlCommandType sqlCommandType)
        {
            logger.LogDebug("{location} SQL of type {type} generated: {sql}", PrintLocation(), sqlCommandType, sql);
        }
    }
}
