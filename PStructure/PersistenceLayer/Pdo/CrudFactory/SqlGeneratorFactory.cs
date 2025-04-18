﻿using System;
using System.Reflection;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

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