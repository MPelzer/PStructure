namespace PStructure.SqlGenerator
{
    /// <summary>
    /// Interface für den SQLGenerator, welcher die grundlegendsten SQLs für übergebene PDOs generiert.
    /// </summary>
    public interface ISqlGenerator<T>
    {
        string GetInsertSql(string tableLocation);

        string GetReadSqlByPrimaryKey(string tableLocation);

        string GetDeleteSqlByPrimaryKey(string tableLocation);

        string GetUpdateSqlByPrimaryKey(string tableLocation);

        string GetSelectAll(string tableLocation);
    }
}