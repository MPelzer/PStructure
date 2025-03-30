using Microsoft.Extensions.Logging;

namespace PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator
{
    /// <summary>
    ///     Interface für den SQLGenerator, welcher die grundlegendsten SQLs für übergebene PDOs generiert.
    /// </summary>
    public interface ISqlGenerator<T>
    {
        string GetInsertSql(ILogger logger);

        string GetReadSqlByPrimaryKey(ILogger logger);

        string GetDeleteSqlByPrimaryKey(ILogger logger);

        string GetUpdateSqlByPrimaryKey(ILogger logger);

        string GetReadAll(ILogger logger);

        string GetTableLocation();

        void LogGeneratedSql(ILogger logger, string sql, string commandType);
    }
}