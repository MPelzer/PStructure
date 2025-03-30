using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.DummyCrud
{
    public class DummySqlGenerator<T> : SqlGenerator<T>
    {
        private DummySqlGenerator(ITableLocation tableLocation) : base(tableLocation)
        {
        }

        public string GetInsertSql(ILogger logger, string tableLocation)
        {
            var sql = "Dummy";
            LogGeneratedSql(logger, sql, "Insert");
            return sql;
        }

        public string GetReadSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = "Dummy";
            LogGeneratedSql(logger, sql, "Read");
            return sql;
        }

        public string GetDeleteSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = "Dummy";
            LogGeneratedSql(logger, sql, "Delete");
            return sql;
        }

        public string GetUpdateSqlByPrimaryKey(ILogger logger, string tableLocation)
        {
            var sql = "Dummy";
            LogGeneratedSql(logger, sql, "Update");
            return sql;
        }

        public string GetReadAll(ILogger logger, string tableLocation)
        {
            var sql = "Dummy";
            LogGeneratedSql(logger, sql, "ReadAll");
            return sql;
        }
    }
}