using System;
using System.Linq;
using System.Reflection;
using PStructure.Models;

namespace PStructure.PersistenceLayer.PdoProperties
{
    public static class PdoPropertyCache<T>
    {

        #region Properties

        public static Models.TableLocation[] TableLocationData => _tableLocationData.Value;
        public static PropertyInfo[] Properties => _propertyData.Value;
        public static PropertyInfo[] PrimaryKeyProperties => _primaryKeyData.Value;

        #endregion
        
        #region Delegates

        private static readonly Lazy<PropertyInfo[]> _propertyData = new Lazy<PropertyInfo[]>(() => 
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<Column>() != null)
                .ToArray()
        );

        private static readonly Lazy<PropertyInfo[]> _primaryKeyData = new Lazy<PropertyInfo[]>(() => 
            typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<PrimaryKey>() != null)
                .ToArray()
        );

        private static readonly Lazy<Models.TableLocation[]> _tableLocationData = new Lazy<Models.TableLocation[]>(() =>
            typeof(T).GetCustomAttributes<Models.TableLocation>().ToArray()
        );

        #endregion


        #region GettersByProperty

        /// <summary>
        /// Retrieves the TableLocation for a given work mode.
        /// </summary>
        /// <param name="mode">The work mode (e.g., Test or Live).</param>
        /// <returns>The TableLocationAttribute for the specified mode, or null if not found.</returns>
        public static Models.TableLocation GetTableLocationByWorkMode(WorkMode mode)
        {
            return _tableLocationData.Value.FirstOrDefault(attr => attr.Mode == mode);
        }

        #endregion
        
        
    }
}