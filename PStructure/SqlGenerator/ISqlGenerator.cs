using System;

namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Interface für den SQLGenerator, welcher die grundlegendsten SQLs für übergebene PDOs generiert./>
    /// </summary>
    public interface ISqlGenerator
    {
        string GetInsertSql(Type type, string tableLocation);

        string GetReadSqlByPrimaryKey(Type type, string tableLocation);

        string GetDeleteSqlByPrimaryKey(Type type, string tableLocation);

        string GetUpdateSqlByPrimaryKey(Type type, string tableLocation);

        string GetSelectAll(string tableLocation);
    }
}