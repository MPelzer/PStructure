using System;
using System.Reflection;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds.Crud
{
    public class SqlGeneratorFactory<T>
    {
        public ISqlGenerator<T> GetSqlGenerator(CrudType crudType)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.SqlGeneratorAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' is not supported.");

            // Use reflection to instantiate the SQL generator directly
            var generatorType = attribute.GeneratorType.MakeGenericType(typeof(T));
            return (ISqlGenerator<T>)Activator.CreateInstance(generatorType);
        }
    }
}