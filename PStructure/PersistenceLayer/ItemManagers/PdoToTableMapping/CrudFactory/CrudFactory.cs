using System;
using System.Reflection;
using PStructure.CRUDs;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds.SimpleCrud
{
    public class CrudFactory<T>
    {
        public ICrud<T> GetCrud(CrudType crudType)
        {
            var attribute = crudType.GetType()
                .GetField(crudType.ToString())
                .GetCustomAttribute<FactoryAttibutes.CrudAttribute>();

            if (attribute == null)
                throw new NotSupportedException($"CrudType '{crudType}' is not supported.");

            var returnCrudType = attribute.CrudType.MakeGenericType(typeof(T));
            return (ICrud<T>)Activator.CreateInstance(returnCrudType);
        }
    }
}