using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using PStructure.Interfaces;
using PStructure.Models;

namespace PStructure
{
    public class MapperPdoQuery<T> : IMapperPdoQuery<T>
    {
        
        public void MapPrimaryKeysToParameters(T item, DynamicParameters parameters)
        {
            var primaryKeyProps = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            foreach (var prop in primaryKeyProps)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var value = prop.GetValue(item);

                string columnName = columnAttr?.ColumnName ?? prop.Name;

                parameters.Add("@" + columnName, value);
            }
        }

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

                string columnName = columnAttr.ColumnName;

                if (handlerAttr != null)
                {
                    var handler = (ICustomHandler)Activator.CreateInstance(handlerAttr.HandlerType);
                    value = handler.Format(value);
                }

                parameters.Add("@" + columnName, value);
            }
        }

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
                if (value == DBNull.Value) continue;

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