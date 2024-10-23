using System;
using System.Data;
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
        /// <summary>
        /// Fügt einem Set an DynamicParameters Parameter für alle Eigenschaften des PDO´s mit Attribut <see cref="PrimaryKeyAttribute"/>
        /// hinzu.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            var primaryKeyProps = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            foreach (var prop in primaryKeyProps)
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
            foreach (var prop in typeof(T).GetProperties())
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