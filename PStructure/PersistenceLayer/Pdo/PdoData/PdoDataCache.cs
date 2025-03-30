using System;
using System.Linq;
using System.Reflection;
using PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;

namespace PStructure.PersistenceLayer.Pdo.PdoData
{
    public static class PdoDataCache<T>
    {
        #region Properties

        public static TableLocation[] TableLocationData => _tableLocationData.Value;
        public static PropertyInfo[] Properties => _propertyData.Value;
        public static PropertyInfo[] PrimaryKeyProperties => _primaryKeyData.Value;

        #endregion

        #region Delegates

        private static readonly Lazy<PropertyInfo[]> _propertyData = new Lazy<PropertyInfo[]>(() =>
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PdoPropertyAttributes.Column>() != null)
                .ToArray()
        );

        private static readonly Lazy<PropertyInfo[]> _primaryKeyData = new Lazy<PropertyInfo[]>(() =>
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PdoPropertyAttributes.PrimaryKey>() != null)
                .ToArray()
        );

        private static readonly Lazy<TableLocation[]> _tableLocationData = new Lazy<TableLocation[]>(() =>
            typeof(T).GetCustomAttributes<TableLocation>().ToArray()
        );

        #endregion
    }
}