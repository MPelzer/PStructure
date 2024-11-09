﻿using Microsoft.Extensions.Logging;

namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Interface für den SQLGenerator, welcher die grundlegendsten SQLs für übergebene PDOs generiert.
    /// </summary>
    public interface ISqlGenerator<T>
    {
        string GetInsertSql(ILogger logger, string tableLocation);

        string GetReadSqlByPrimaryKey(ILogger logger, string tableLocation);

        string GetDeleteSqlByPrimaryKey(ILogger logger, string tableLocation);

        string GetUpdateSqlByPrimaryKey(ILogger logger, string tableLocation);

        string GetReadAll(ILogger logger, string tableLocation);

        string LogGeneratedSql();
    }
}