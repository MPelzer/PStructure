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
        private readonly KeyValuePair<Type, string> _insertByInstanceSql = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _updateByInstanceSql = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _readByPrimaryKeySql = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _deleteByPrimaryKeySql = new KeyValuePair<Type, string>();
        
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
            if (type == _insertByInstanceSql.Key)
            {
                return _insertByInstanceSql.Value;
            }
            var properties = type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();

            var columnNames = properties.Select(prop => prop.GetCustomAttribute<ColumnAttribute>().ColumnName).ToArray();
            var parameterNames = properties.Select(prop => "@" + prop.GetCustomAttribute<ColumnAttribute>().ColumnName).ToArray();

            var columns = string.Join(", ", columnNames);
            var parameters = string.Join(", ", parameterNames);

            return $"INSERT INTO {tableLocation} ({columns}) VALUES ({parameters})";
        }

        /// <summary>
        /// Generiert den Lesebefehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetReadSqlByPrimaryKey(Type type, string tableLocation)
        {
            if (type == _readByPrimaryKeySql.Key)
            {
                return _readByPrimaryKeySql.Value;
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
            return $"SELECT * FROM {tableLocation} WHERE {whereClause}";
        }

        /// <summary>
        /// Generiert den Löschbefehl für ein PDO
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableLocation"></param>
        /// <returns></returns>
        public string GetDeleteSqlByPrimaryKey(Type type, string tableLocation)
        {
            if (type == _deleteByPrimaryKeySql.Key)
            {
                return _deleteByPrimaryKeySql.Value;
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
            return $"DELETE FROM {tableLocation} WHERE {whereClause}";
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
            if (type == _updateByInstanceSql.Key)
            {
                return _updateByInstanceSql.Value;
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

            return $"UPDATE {tableLocation} SET {setClause} WHERE {whereClause}";
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
