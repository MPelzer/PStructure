using System;

namespace PStructure.PersistenceLayer.Pdo.PdoData.Attributes
{
    public class FactoryAttibutes
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class CrudAttribute : Attribute
        {
            public CrudAttribute(Type crudType)
            {
                CrudType = crudType;
            }

            public Type CrudType { get; }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class ValidatorAttribute : Attribute
        {
            public ValidatorAttribute(Type validatorType)
            {
                ValidatorType = validatorType;
            }

            public Type ValidatorType { get; }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class MapperAttribute : Attribute
        {
            public MapperAttribute(Type mapperType)
            {
                MapperType = mapperType;
            }

            public Type MapperType { get; }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class SqlGeneratorAttribute : Attribute
        {
            public SqlGeneratorAttribute(Type generatorType)
            {
                GeneratorType = generatorType;
            }

            public Type GeneratorType { get; }
        }
    }
}