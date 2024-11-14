using Dapper;
using PStructure.Mapper;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds.Crud
{
    public class Mapper<T> : IMapper<T>
    {
        public void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            throw new System.NotImplementedException();
        }

        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}