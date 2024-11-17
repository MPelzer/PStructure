using System;
using System.Reflection;
using PStructure.CRUDs;
using PStructure.Mapper;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.Pdo.CrudFactory
{
    public static class CrudFactory<T>
    {
        public static ICrud<T> GetCrud(CrudType crudType, ISqlGenerator<T> sqlGenerator, IMapper<T> mapper)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.CrudAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' is not supported.");

            var resultCrudType = attribute.CrudType.MakeGenericType(typeof(T));
            return (ICrud<T>)Activator.CreateInstance(resultCrudType, sqlGenerator, mapper);
        }
    }
}