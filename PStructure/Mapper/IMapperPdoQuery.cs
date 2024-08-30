using System.Data;
using Dapper;

namespace PStructure.Mapper
{
    /// <summary>
    /// Interface, welches Funktionen definiert, um die PDO-Eigenschaften auf die Tabellenspalten abzubilden
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMapperPdoQuery<T>
    {
        void MapTableColumnsToPdo(T entity, IDataReader reader);
        void MapPdoToTable(T item, DynamicParameters parameters);
        void MapPrimaryKeysToParameters(T item, DynamicParameters parameters);
    }

}