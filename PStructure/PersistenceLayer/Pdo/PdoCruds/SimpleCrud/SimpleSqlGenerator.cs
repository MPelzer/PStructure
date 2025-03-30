using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud
{
    /// <summary>
    ///     Generates standard CRUD SQL commands for items of type <typeparamref name="T" />.
    /// </summary>
    public class SimpleSqlGenerator<T> : SqlGenerator<T>
    {
        // SQL command cache for each type
        private static readonly ConcurrentDictionary<string, string> _sqlCache
            = new ConcurrentDictionary<string, string>();

        public SimpleSqlGenerator(ITableLocation tableLocation) : base(tableLocation)
        {
        }

        /// <summary>
        ///     Generates the INSERT SQL command for a PDO.
        /// </summary>
        public new string GetInsertSql(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Insert", _ =>
            {
                var columnNames = PdoDataCache<T>.Properties
                    .Select(prop => prop.GetCustomAttribute<PdoPropertyAttributes.Column>().ColumnName);
                var parameterNames = PdoDataCache<T>.Properties
                    .Select(prop => "@" + prop.GetCustomAttribute<PdoPropertyAttributes.Column>().ColumnName);
                var sql = $"INSERT INTO {GetTableLocation()} ({string.Join(", ", columnNames)}) " +
                          $"VALUES ({string.Join(", ", parameterNames)})";

                LogGeneratedSql(logger, sql, "Insert");
                return sql;
            });
        }

        /// <summary>
        ///     Generates the READ SQL command by primary key.
        /// </summary>
        public new string GetReadSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Read", _ =>
            {
                var whereClauses = PdoDataCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"SELECT * FROM {GetTableLocation()} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Read");
                return sql;
            });
        }

        /// <summary>
        ///     Generates the DELETE SQL command by primary key.
        /// </summary>
        public new string GetDeleteSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Delete", _ =>
            {
                var whereClauses = PdoDataCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql = $"DELETE FROM {GetTableLocation()} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Delete");
                return sql;
            });
        }

        /// <summary>
        ///     Generates the UPDATE SQL command by primary key.
        /// </summary>
        public new string GetUpdateSqlByPrimaryKey(ILogger logger)
        {
            return _sqlCache.GetOrAdd($"{typeof(T).FullName}_Update", _ =>
            {
                var setClauses = PdoDataCache<T>.Properties
                    .Except(PdoDataCache<T>.PrimaryKeyProperties)
                    .Select(prop =>
                    {
                        var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                        var columnName = columnAttr?.ColumnName ?? prop.Name;
                        return $"{columnName} = @{columnName}";
                    });

                var whereClauses = PdoDataCache<T>.PrimaryKeyProperties.Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                    var columnName = columnAttr?.ColumnName ?? prop.Name;
                    return $"{columnName} = @{columnName}";
                });

                var sql =
                    $"UPDATE {GetTableLocation()} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";
                LogGeneratedSql(logger, sql, "Update");
                return sql;
            });
        }

        /// <summary>
        ///     Generates the SQL command to read all records.
        /// </summary>
        public new string GetReadAll(ILogger logger)
        {
            var sql = $"SELECT * FROM {GetTableLocation()}";
            LogGeneratedSql(logger, sql, "ReadAll");
            return sql;
        }
    }
}