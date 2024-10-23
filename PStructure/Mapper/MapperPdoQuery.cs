using System;
using System.Linq;
using System.Reflection;
using Dapper;
using PStructure.Interfaces;
using PStructure.Models;

namespace PStructure.Mapper
{
    /// <summary>
    /// Bildet die PDO-Eigenschaften auf die Tabellenspalten ab.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MapperPdoQuery<T> : IMapperPdoQuery<T>
    {
        
        private static readonly PropertyInfo[] _propertyInfoCache;
        private static readonly PropertyInfo[] _primaryKeyInfoCache;

        // Static constructor to initialize the property caches
        static MapperPdoQuery()
        {
            _propertyInfoCache = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray();

            _primaryKeyInfoCache = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();
        }
        
        
        /// <summary>
        /// Fügt einem Set an DynamicParameters Parameter für alle Eigenschaften des PDO´s mit Attribut <see cref="PrimaryKeyAttribute"/>
        /// hinzu.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in _primaryKeyInfoCache)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var value = prop.GetValue(item);

                var columnName = columnAttr?.ColumnName ?? prop.Name;
                parameters.Add("@" + columnName, value);
            }
        }

        /// <summary>
        /// Fügt einem Set an Parametern Parameter für jedes Attribut hinzu. Dabei werden potenziell transformationen durch
        /// einen <see cref="ICustomHandler"/> vorgenommen
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MapPropertiesToParameters(T item, DynamicParameters parameters)
        {
            foreach (var prop in _propertyInfoCache)
            {
                var value = prop.GetValue(item);
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                
                if (columnAttr == null)
                {
                    throw new InvalidOperationException($"Property {prop.Name} does not have a ColumnAttribute.");
                }

                var columnName = columnAttr.ColumnName;
                parameters.Add("@" + columnName, value);
            }
        }
    }
}