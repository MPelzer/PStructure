using Dapper;

namespace PStructure.PersistenceLayer.DatabaseStuff
{
    public interface IDbRequest<T>
    {
        string GetSql();
        DynamicParameters GetParameters(T item);
    }
}
