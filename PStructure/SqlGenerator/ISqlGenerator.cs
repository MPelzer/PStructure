using System;

namespace PStructure.SqlGenerator
{
    public interface ISqlGenerator
    {
        string GetInsertSql(Type type, string tableLocation);

        string GetReadSqlByPrimaryKey(Type type, string tableLocation);

        string GetDeleteSqlByPrimaryKey(Type type, string tableLocation);

        string GetUpdateSqlByPrimaryKey(Type type, string tableLocation);

        string GetSelectAll(string tableLocation);
    }
}