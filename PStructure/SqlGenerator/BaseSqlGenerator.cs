using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PStructure.Models;

namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Generiert für Items die Standard CRUD SQL-Befehle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSqlGenerator<T> : ISqlGenerator
    {
        /// <summary>
        /// Minicache, sollte das gleiche Item hintereinander angefragt werden.
        /// </summary>
        private readonly Dictionary<Type, string> _insertByInstanceSqlCache = new Dictionary<Type, string>();
        private readonly Dictionary<Type, string> _updateByInstanceSqlCache = new Dictionary<Type, string>();
        private readonly Dictionary<Type, string> _readByPrimaryKeySqlCache = new Dictionary<Type, string>();
        private readonly Dictionary<Type, string> _deleteByPrimaryKeySqlCache = new Dictionary<Type, string>();

        public BaseSqlGenerator()
        {
        }

        /// <summary>
        /// Generiert den Einfüge Befehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetInsertSql(Type type, string tableLocation)
        {
            if (_insertByInstanceSqlCache.TryGetValue(type, out var cachedSql))
            {
                return cachedSql;
            }
            
            var properties = type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();

            var columnNames = properties.Select(prop => prop.GetCustomAttribute<ColumnAttribute>().ColumnName).ToArray();
            var parameterNames = properties.Select(prop => "@" + prop.GetCustomAttribute<ColumnAttribute>().ColumnName).ToArray();

            var columns = string.Join(", ", columnNames);
            var parameters = string.Join(", ", parameterNames);

            var sql = $"INSERT INTO {tableLocation} ({columns}) VALUES ({parameters})";
            _insertByInstanceSqlCache[type] = sql;
            return sql;
        }

        /// <summary>
        /// Generiert den Lesebefehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetReadSqlByPrimaryKey(Type type, string tableLocation)
        {
            if (_readByPrimaryKeySqlCache.TryGetValue(type, out var cachedSql))
            {
                return cachedSql;
            }

            var primaryKeyProps = type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();

            if (!primaryKeyProps.Any())
            {
                throw new InvalidOperationException($"Type {type.Name} does not have any properties with [PrimaryKey] attribute.");
            }

            var whereClauses = primaryKeyProps.Select(prop =>
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                return $"{columnName} = @{columnName}";
            });

            var whereClause = string.Join(" AND ", whereClauses);
            var sql = $"SELECT * FROM {tableLocation} WHERE {whereClause}";
            _readByPrimaryKeySqlCache[type] = sql;
            return sql;
        }

        /// <summary>
        /// Generiert den Löschbefehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetDeleteSqlByPrimaryKey(Type type, string tableLocation)
        {
            if (_deleteByPrimaryKeySqlCache.TryGetValue(type, out var cachedSql))
            {
                return cachedSql;
            }

            var primaryKeyProps = type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();

            if (!primaryKeyProps.Any())
            {
                throw new InvalidOperationException($"Type {type.Name} does not have any properties with [PrimaryKey] attribute.");
            }

            var whereClauses = primaryKeyProps.Select(prop =>
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                return $"{columnName} = @{columnName}";
            });

            var whereClause = string.Join(" AND ", whereClauses);
            var sql = $"DELETE FROM {tableLocation} WHERE {whereClause}";
            _deleteByPrimaryKeySqlCache[type] = sql;
            return sql;
        }

        /// <summary>
        /// Generiert den Aktualisierungsbefehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string GetUpdateSqlByPrimaryKey(Type type, string tableLocation)
        {
            if (_updateByInstanceSqlCache.TryGetValue(type, out var cachedSql))
            {
                return cachedSql;
            }

            var properties = type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();

            var primaryKeyProps = properties
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();

            if (!primaryKeyProps.Any())
            {
                throw new InvalidOperationException($"Type {type.Name} does not have any properties with [PrimaryKey] attribute.");
            }

            var nonPrimaryKeyProps = properties.Except(primaryKeyProps).ToArray();

            var setClauses = nonPrimaryKeyProps.Select(prop =>
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                return $"{columnName} = @{columnName}";
            });

            var whereClauses = primaryKeyProps.Select(prop =>
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                return $"{columnName} = @{columnName}";
            });

            var setClause = string.Join(", ", setClauses);
            var whereClause = string.Join(" AND ", whereClauses);

            var sql = $"UPDATE {tableLocation} SET {setClause} WHERE {whereClause}";
            _updateByInstanceSqlCache[type] = sql;
            return sql;
        }

        /// <summary>
        /// Liest die gesamte Tabelle für ein PDO-Typ aus.
        /// </summary>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetSelectAll(string tableLocation)
        {
            return $"SELECT * FROM {tableLocation}";
        }
    }
}
