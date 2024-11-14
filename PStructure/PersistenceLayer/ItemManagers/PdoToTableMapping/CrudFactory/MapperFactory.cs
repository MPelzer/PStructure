using System;
using System.Reflection;
using PStructure.Mapper;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Mappers
{
    public class MapperFactory<T>
    {
        public IMapper<T> GetMapper(CrudType crudType)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.MapperAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' is not supported.");

            var mapperType = attribute.MapperType.MakeGenericType(typeof(T));
            return (IMapper<T>)Activator.CreateInstance(mapperType);
        }
    }
}