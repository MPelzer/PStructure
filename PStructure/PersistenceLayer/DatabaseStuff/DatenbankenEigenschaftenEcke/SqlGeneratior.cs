using System;
using System.Collections.Generic;
using System.Linq;
using PStructure.PersistenceLayer.Pdo.PdoData;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    public interface ISqlGenerator<T>
    {
        (string Sql, Func<IEnumerable<object>, object> ParameterFactory) GetInsert();
        (string Sql, Func<IEnumerable<object>, object> ParameterFactory) GetUpdate();
        (string Sql, Func<IEnumerable<object>, object> ParameterFactory) GetDelete();
        (string Sql, Func<IEnumerable<object>, object> ParameterFactory) GetReadByPk();
        string GetReadAll();
    }

    public class DefaultSqlGenerator<T> : ISqlGenerator<T>
    {
        private readonly string _tableName;

        public DefaultSqlGenerator()
        {
            // Default: class name = table name
            // You could add a [Table("...")] attribute later if needed
            _tableName = typeof(T).Name;
        }

        public (string, Func<IEnumerable<object>, object>) GetInsert()
        {
            var cols = PdoDataCache<T>.ColumnNames;
            var sql = $"INSERT INTO {_tableName} ({string.Join(", ", cols)}) VALUES ({string.Join(", ", cols.Select(c => "@" + c))});";

            return (sql, items => items); // Dapper can take IEnumerable<T>
        }

        public (string, Func<IEnumerable<object>, object>) GetUpdate()
        {
            var cols = PdoDataCache<T>.ColumnNames;
            var pkCols = PdoDataCache<T>.PrimaryKeyNames;

            if (!pkCols.Any())
                throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var setClause = string.Join(", ", cols.Where(c => !pkCols.Contains(c)).Select(c => $"{c} = @{c}"));
            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));

            var sql = $"UPDATE {_tableName} SET {setClause} WHERE {whereClause};";

            return (sql, items => items);
        }

        public (string, Func<IEnumerable<object>, object>) GetDelete()
        {
            var pkCols = PdoDataCache<T>.PrimaryKeyNames;

            if (!pkCols.Any())
                throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));
            var sql = $"DELETE FROM {_tableName} WHERE {whereClause};";

            return (sql, items => items);
        }

        public (string, Func<IEnumerable<object>, object>) GetReadByPk()
        {
            var pkCols = PdoDataCache<T>.PrimaryKeyNames;
            var cols = PdoDataCache<T>.ColumnNames;

            if (!pkCols.Any())
                throw new InvalidOperationException($"No primary key defined on {typeof(T).Name}");

            var selectCols = string.Join(", ", cols);
            var whereClause = string.Join(" AND ", pkCols.Select(c => $"{c} = @{c}"));

            var sql = $"SELECT {selectCols} FROM {_tableName} WHERE {whereClause};";

            return (sql, items => items);
        }

        public string GetReadAll()
        {
            var cols = PdoDataCache<T>.ColumnNames;
            return $"SELECT {string.Join(", ", cols)} FROM {_tableName};";
        }
    }
}
