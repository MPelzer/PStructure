using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds.Crud
{
    public class SqlGenerator<T> : ClassCore, ISqlGenerator<T>
    {
        private readonly ITableLocation _tableLocation;

        protected SqlGenerator(ITableLocation tableLocation)
        {
            _tableLocation = tableLocation;
        }
        public string GetInsertSql(ILogger logger)
        {
            throw new System.NotImplementedException();
        }

        public string GetReadSqlByPrimaryKey(ILogger logger)
        {
            throw new System.NotImplementedException();
        }

        public string GetDeleteSqlByPrimaryKey(ILogger logger)
        {
            throw new System.NotImplementedException();
        }

        public string GetUpdateSqlByPrimaryKey(ILogger logger)
        {
            throw new System.NotImplementedException();
        }

        public string GetReadAll(ILogger logger)
        {
            throw new System.NotImplementedException();
        }
        
        public string GetTableLocation()
        {
            return _tableLocation.PrintTableLocation();
        }

        public void LogGeneratedSql(ILogger logger, string sql, string commandType)
        {
            logger?.LogDebug("{Location} SQL of type {Type} generated: {Sql}", 
                GetLoggingClassName(), commandType, sql);
        }
    }
}