using System;
using PStructure.Models;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.PdoProperties;

namespace PStructure.TableLocation
{
    /// <summary>
    /// A factory class for creating instances of BaseTableLocation.
    /// </summary>
    public static class TableLocationFactory<T>
    {
        /// <summary>
        /// Creates an instance of BaseTableLocation based on a specified WorkMode.
        /// </summary>
        /// <param name="mode">The WorkMode (e.g., Test, Live, etc.).</param>
        /// <returns>A new instance of BaseTableLocation, or throws an exception if configuration is missing.</returns>
        public static ITableLocation CreateTableLocationByMode(WorkMode mode)
        {
            var tableLocationAttribute = PdoPropertyCache<T>.GetTableLocationByWorkMode(mode);

            if (tableLocationAttribute == null)
                throw new InvalidOperationException($"Table location for WorkMode '{mode}' is not defined.");

            return tableLocationAttribute;
        }
        
        /// <summary>
        /// Creates an instance of BaseTableLocation for the Live environment.
        /// </summary>
        /// <returns>A new instance of BaseTableLocation configured for the Live environment.</returns>
        public static ITableLocation CreateLiveTableLocation()
        {
            return CreateTableLocationByMode(WorkMode.Live);
        }
        
        /// <summary>
        /// Creates an instance of BaseTableLocation for the Test environment.
        /// </summary>
        /// <returns>A new instance of BaseTableLocation configured for the Test environment.</returns>
        public static ITableLocation CreateForTest()
        {
            return CreateTableLocationByMode(WorkMode.Test);
        }

        /// <summary>
        /// Creates an instance of BaseTableLocation for a dummy configuration, or test cases that simulate database interactions.
        /// </summary>
        /// <returns>A new instance of BaseTableLocation configured for a dummy environment.</returns>
        public static ITableLocation CreateForDummy()
        {
            return CreateTableLocationByMode(WorkMode.Dummy); // Assuming Dummy mode exists in WorkMode enum
        }
    }
}
