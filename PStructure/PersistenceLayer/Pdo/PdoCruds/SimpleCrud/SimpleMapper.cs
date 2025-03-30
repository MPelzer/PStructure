using System;
using System.Reflection;
using Dapper;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud
{
    /// <summary>
    ///     Maps the properties of a PDO object to table columns.
    /// </summary>
    /// <typeparam name="T">The type of the PDO object.</typeparam>
    public class SimpleMapper<T> : IMapper<T>
    {
        /// <summary>
        ///     Adds parameters for all primary key properties of the PDO object to a set of <see cref="DynamicParameters" />.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoDataCache<T>.PrimaryKeyProperties)
            {
                // Use the Column attribute if present, otherwise fallback to property name
                var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                var value = prop.GetValue(item);

                parameters.Add("@" + columnName, value);
            }
        }

        /// <summary>
        ///     Adds parameters for all properties with a <see cref="DynamicParameters" /> attribute to a set of
        ///     <see cref="parameters" />.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        /// <exception cref="PdoPropertyAttributes">
        ///     Thrown if any property is missing a <see cref="PdoPropertyAttributes.Column" />
        ///     attribute.
        /// </exception>
        public void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoDataCache<T>.Properties)
            {
                var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                if (columnAttr == null)
                    throw new InvalidOperationException($"Property {prop.Name} does not have a Column attribute.");

                var columnName = columnAttr.ColumnName;
                var value = prop.GetValue(item);
                parameters.Add("@" + columnName, value);
            }
        }
    }
}