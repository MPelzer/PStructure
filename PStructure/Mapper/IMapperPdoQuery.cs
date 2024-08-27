using System.Data;
using Dapper;

namespace PStructure
{
    
    public interface IMapperPdoQuery<T>
    {
        void MapTableColumnsToPdo(T entity, IDataReader reader);
        void MapPdoToTable(T item, DynamicParameters parameters);
        void MapPrimaryKeysToParameters(T item, DynamicParameters parameters);
    }

}