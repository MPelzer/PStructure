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
        public void MapPdoToTable(T item, DynamicParameters parameters)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = prop.GetValue(item);
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var handlerAttr = prop.GetCustomAttribute<TypeHandlerAttribute>();
                
                if (columnAttr == null)
                {
                    throw new InvalidOperationException($"Property {prop.Name} does not have a ColumnAttribute.");
                }
                var columnName = columnAttr.ColumnName;
                if (handlerAttr != null)
                {
                    var handler = (ICustomHandler)Activator.CreateInstance(handlerAttr.HandlerType);
                    value = handler.Format(value);
                }

                parameters.Add("@" + columnName, value);
            }
        }

        /// <summary>
        /// Erhält einen <see cref="IDataReader"/> und setzt anhand der Spalten-Eigenschaft-Zuweisung durch Attribute im
        /// PDO die aus der Datenbank ausgelesenen Werte.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="reader"></param>
        public void MapTableColumnsToPdo(T entity, IDataReader reader)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var columnName = prop.Name;

                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    columnName = columnAttr.ColumnName;
                }

                var value = reader[columnName];
                if (value == DBNull.Value) continue; //Wenn DB-Null, dann ignorieren. (Es bleibt der Factory-Wert)

                var handlerAttr = prop.GetCustomAttribute<TypeHandlerAttribute>();
                if (handlerAttr != null)
                {
                    var handler = (ICustomHandler)Activator.CreateInstance(handlerAttr.HandlerType);
                    value = handler.Parse(value);
                }

                prop.SetValue(entity, value);
            }
        }
    }
}