using System;
using System.Linq;
using System.Reflection;
using PStructure.Models;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;

namespace PStructure.PersistenceLayer.PdoData
{
    public static class PdoDataCache<T>
    {

        #region Properties

        
        public static Models.TableLocation[] TableLocationData => _tableLocationData.Value;
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

        private static readonly Lazy<Models.TableLocation[]> _tableLocationData = new Lazy<Models.TableLocation[]>(() =>
            typeof(T).GetCustomAttributes<Models.TableLocation>().ToArray()
        );

        #endregion
        
    }
}