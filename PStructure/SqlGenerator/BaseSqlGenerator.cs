using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PStructure.Models;

namespace PStructure.SqlGenerator
{
    public class BaseSqlGenerator<T> : ISqlGenerator
    {
        private readonly KeyValuePair<Type, string> _insertByInstanceSqls = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _updateByInstanceSql = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _readByPrimaryKeySql = new KeyValuePair<Type, string>();
        private readonly KeyValuePair<Type, string> _deleteByPrimaryKeySql = new KeyValuePair<Type, string>();
        
        public BaseSqlGenerator()
        {
            
        }
        public string GetInsertSql(Type type, string tableLocation)
        {
            if (type == _insertByInstanceSqls.Key)
            {
                return _insertByInstanceSqls.Value;
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

        public string GetSelectAll(string tableLocation)
        {
            return $"SELECT * FROM {tableLocation}";
        }
    }
}
