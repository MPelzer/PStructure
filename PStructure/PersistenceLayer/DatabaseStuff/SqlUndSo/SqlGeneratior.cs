using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    public interface ISqlGenerator<T>
    {
        string GetInsertSql(ILogger logger = null);
        string GetUpdateSqlByPrimaryKey(ILogger logger = null);
        string GetDeleteSqlByPrimaryKey(ILogger logger = null);
        string GetReadSqlByPrimaryKey(ILogger logger = null);
        string GetReadAll(ILogger logger = null);
    }

    public class DefaultSqlGenerator<T> : ISqlGenerator<T>
    {
        private readonly string _tableName;

        public DefaultSqlGenerator()
        {
            _tableName = typeof(T).Name; // optionally: infer via attribute or naming policy
        }

        public string GetInsertSql(ILogger logger = null)
        {
            var cols = PdoDataCache<T>.ColumnNames;
            var columnList = string.Join(", ", cols);
            var valueList = string.Join(", ", cols.Select(c => "@" + c));

            var sql = $"INSERT INTO {_tableName} ({columnList}) VALUES ({valueList});";
            logger?.LogDebug("[SqlGenerator] Insert SQL: {sql}", sql);
            return sql;
        }

        public string GetUpdateSqlByPrimaryKey(ILogger logger = null)
        {
            var cols = PdoDataCache<T>.ColumnNames;
            var pkCols = PdoDataCache<T>.PrimaryKeyProperties.Select(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>()?.ColumnName).ToArray();

            if (!pkCols.Any()) throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var setClause = string.Join(", ", cols.Where(c => !pkCols.Contains(c)).Select(c => $"{c} = @{c}"));
            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));

            var sql = $"UPDATE {_tableName} SET {setClause} WHERE {whereClause};";
            logger?.LogDebug("[SqlGenerator] Update SQL: {sql}", sql);
            return sql;
        }

        public string GetDeleteSqlByPrimaryKey(ILogger logger = null)
        {
            var pkCols = PdoDataCache<T>.PrimaryKeyProperties.Select(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>()?.ColumnName).ToArray();

            if (!pkCols.Any()) throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));
            var sql = $"DELETE FROM {_tableName} WHERE {whereClause};";

            logger?.LogDebug("[SqlGenerator] Delete SQL: {sql}", sql);
            return sql;
        }

        public string GetReadSqlByPrimaryKey(ILogger logger = null)
        {
            var pkCols = PdoDataCache<T>.PrimaryKeyProperties.Select(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>()?.ColumnName).ToArray();
            var cols = PdoDataCache<T>.ColumnNames;

            if (!pkCols.Any()) throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var selectCols = string.Join(", ", cols);
            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));

            var sql = $"SELECT {selectCols} FROM {_tableName} WHERE {whereClause};";
            logger?.LogDebug("[SqlGenerator] ReadByPK SQL: {sql}", sql);
            return sql;
        }

        public string GetReadAll(ILogger logger = null)
        {
            var cols = PdoDataCache<T>.ColumnNames;
            var selectCols = string.Join(", ", cols);

            var sql = $"SELECT {selectCols} FROM {_tableName};";
            logger?.LogDebug("[SqlGenerator] ReadAll SQL: {sql}", sql);
            return sql;
        }
    }
}
