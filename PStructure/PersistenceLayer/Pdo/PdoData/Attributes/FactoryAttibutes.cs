using System;

namespace PStructure.PersistenceLayer.Utils
{
    public class FactoryAttibutes
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class CrudAttribute : Attribute
        {
            public Type CrudType { get; }

            public CrudAttribute(Type crudType)
            {
                CrudType = crudType;
            }
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class ValidatorAttribute : Attribute
        {
            public Type ValidatorType { get; }

            public ValidatorAttribute(Type validatorType)
            {
                ValidatorType = validatorType;
            }
        }
        
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class MapperAttribute : Attribute
        {
            public Type MapperType { get; }

            public MapperAttribute(Type mapperType)
            {
                MapperType = mapperType;
            }
        }
        
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class SqlGeneratorAttribute : Attribute
        {
            public Type GeneratorType { get; }

            public SqlGeneratorAttribute(Type generatorType)
            {
                GeneratorType = generatorType;
            }
        }
    }
}