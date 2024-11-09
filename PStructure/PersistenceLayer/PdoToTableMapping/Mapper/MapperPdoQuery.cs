using System;
using System.Linq;
using System.Reflection;
using Dapper;
using PStructure.Interfaces;
using PStructure.Models;
using PStructure.PersistenceLayer.PdoProperties;

namespace PStructure.Mapper
{
    /// <summary>
    /// Maps the properties of a PDO object to table columns.
    /// </summary>
    /// <typeparam name="T">The type of the PDO object.</typeparam>
    public static class MapperPdoQuery<T> where T : class
    {
        
        /// <summary>
        /// Adds parameters for all primary key properties of the PDO object to a set of <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        public static void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoPropertyCache<T>.PrimaryKeyProperties)
            {
                var columnAttr = prop.GetCustomAttribute<Column>();
                var value = prop.GetValue(item);

                var columnName = columnAttr?.ColumnName ?? prop.Name;
                parameters.Add("@" + columnName, value);
            }
        }

        /// <summary>
        /// Adds parameters for all properties with a <see cref="Column"/> attribute to a set of <see cref="DynamicParameters"/>.
        /// </summary>
        /// <param name="item">The PDO object.</param>
        /// <param name="parameters">The dynamic parameters set to add to.</param>
        /// <exception cref="InvalidOperationException">Thrown if any property is missing a <see cref="Column"/> attribute.</exception>
        public static void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in PdoPropertyCache<T>.Properties)
            {
                var value = prop.GetValue(item);
                var columnAttr = prop.GetCustomAttribute<Column>();

                if (columnAttr == null)
                {
                    throw new InvalidOperationException($"Property {prop.Name} does not have a Column attribute.");
                }

                var columnName = columnAttr.ColumnName;
                parameters.Add("@" + columnName, value);
            }
        }
    }
}
