using System;
using System.Reflection;
using Dapper;
using PStructure.Interfaces;
using PStructure.Models;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.Mapper
{
    /// <summary>
    /// Maps the properties of a PDO object to table columns.
    /// </summary>
    /// <typeparam name="T">The type of the PDO object.</typeparam>
    public class SimpleMapper<T> : IMapper<T>
    {
        public SimpleMapper()
        {
        }

        /// <summary>
        /// Adds parameters for all primary key properties of the PDO object to a set of <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoMetadata<T>.PrimaryKeyProperties)
            {
                // Use the Column attribute if present, otherwise fallback to property name
                var columnAttr = prop.GetCustomAttribute<Column>();
                var columnName = columnAttr?.ColumnName ?? prop.Name;
                var value = prop.GetValue(item);

                parameters.Add("@" + columnName, value);
            }
        }

        /// <summary>
        /// Adds parameters for all properties with a <see cref="Column"/> attribute to a set of <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        /// <exception cref="InvalidOperationException">Thrown if any property is missing a <see cref="Column"/> attribute.</exception>
        public void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoMetadata<T>.Properties)
            {
                var columnAttr = prop.GetCustomAttribute<Column>();
                if (columnAttr == null)
                {
                    throw new InvalidOperationException($"Property {prop.Name} does not have a Column attribute.");
                }

                var columnName = columnAttr.ColumnName;
                var value = prop.GetValue(item);
                parameters.Add("@" + columnName, value);
            }
        }
    }
}
