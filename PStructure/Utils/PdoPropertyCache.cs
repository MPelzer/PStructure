using System;
using System.Linq;
using System.Reflection;
using PStructure.Models;

namespace PStructure.Utils
{
    public static class PdoPropertyCache<T>
    {
        private static readonly Lazy<PropertyInfo[]> _properties = new Lazy<PropertyInfo[]>(() => 
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
                .ToArray()
        );

        private static readonly Lazy<PropertyInfo[]> _primaryKeyProperties = new Lazy<PropertyInfo[]>(() => 
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray()
        );

        public static PropertyInfo[] Properties => _properties.Value;
        public static PropertyInfo[] PrimaryKeyProperties => _primaryKeyProperties.Value;
    }
}