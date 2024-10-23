using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using PStructure.Models;

namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Generates standard CRUD SQL commands for items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSqlGenerator<T> : ISqlGenerator<T>
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

        private readonly PropertyInfo[] _properties;
        private readonly PropertyInfo[] _primaryKeyProperties;

        public BaseSqlGenerator()
        {
            // Cache property information at construction to reduce reflection overhead
            _properties = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();

            _primaryKeyProperties = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();
        }

        /// <summary>
        /// Generates the INSERT SQL command for a PDO.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetInsertSql(string tableLocation)
        {
            return _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Insert), _ => 
            {
                var columnNames = _properties.Select(prop => prop.GetCustomAttribute<ColumnAttribute>().ColumnName);
                var parameterNames = _properties.Select(prop => "@" + prop.GetCustomAttribute<ColumnAttribute>().ColumnName);

                return $"INSERT INTO {tableLocation} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", parameterNames)})";
            });
        }

        /// <summary>
        /// Generates the READ SQL command for a PDO by primary key.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetReadSqlByPrimaryKey(string tableLocation)
        {
            return _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Read), _ => 
            {
                if (!_primaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var whereClauses = _primaryKeyProperties.Select(prop => 
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"SELECT * FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
            });
        }

        /// <summary>
        /// Generates the DELETE SQL command for a PDO by primary key.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetDeleteSqlByPrimaryKey(string tableLocation)
        {
            return _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Delete), _ => 
            {
                if (!_primaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var whereClauses = _primaryKeyProperties.Select(prop => 
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"DELETE FROM {tableLocation} WHERE {string.Join(" AND ", whereClauses)}";
            });
        }

        /// <summary>
        /// Generates the UPDATE SQL command for a PDO by primary key.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetUpdateSqlByPrimaryKey(string tableLocation)
        {
            return _sqlCache.GetOrAdd((typeof(T), SqlCommandType.Update), _ => 
            {
                if (!_primaryKeyProperties.Any())
                {
                    throw new InvalidOperationException($"Type {typeof(T).Name} does not have any properties with [PrimaryKey] attribute.");
                }

                var setClauses = _properties.Except(_primaryKeyProperties).Select(prop => 
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var whereClauses = _primaryKeyProperties.Select(prop => 
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                return $"UPDATE {tableLocation} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
            });
        }

        /// <summary>
        /// Reads the entire table for a PDO type.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetSelectAll(string tableLocation)
        {
            return $"SELECT * FROM {tableLocation}";
        }
    }
}
