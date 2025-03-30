using System;
using Dapper;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud
{
    public class Mapper<T> : IMapper<T>
    {
        public void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            throw new NotImplementedException();
        }

        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}