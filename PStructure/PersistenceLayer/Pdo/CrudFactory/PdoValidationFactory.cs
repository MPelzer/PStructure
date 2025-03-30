using System;
using System.Reflection;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces;

namespace PStructure.PersistenceLayer.Pdo.CrudFactory
{
    public static class PdoValidatorFactory<T>
    {
        public static IValidator<T> GetValidator(CrudType crudType)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.ValidatorAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' does not have a validator.");

            var validatorType = attribute.ValidatorType.MakeGenericType(typeof(T));
            return (IValidator<T>)Activator.CreateInstance(validatorType);
        }
    }
}