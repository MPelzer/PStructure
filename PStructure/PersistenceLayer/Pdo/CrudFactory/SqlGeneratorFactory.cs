using System;
using System.Reflection;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.PersistenceLayer.Utils;
using PStructure.TableLocation;

namespace PStructure.PersistenceLayer.Pdo.CrudFactory
{
    public static class SqlGeneratorFactory<T>
    {
        public static ISqlGenerator<T> GetSqlGenerator(CrudType crudType, ITableLocation tableLocation)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.SqlGeneratorAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' is not supported.");

            var generatorType = attribute.GeneratorType.MakeGenericType(typeof(T));
            return (ISqlGenerator<T>)Activator.CreateInstance(generatorType, tableLocation);
        }
    }
}