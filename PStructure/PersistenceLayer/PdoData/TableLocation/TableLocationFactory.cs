using System;
using PStructure.Models;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.TableLocation
{
    /// <summary>
    /// Factory for retrieving table locations based on the model type and work mode.
    /// </summary>
    public static class TableLocationFactory<T> where T : class
    {
        /// <summary>
        /// Creates an ITableLocation instance based on the specified work mode.
        /// </summary>
        /// <param name="mode">The desired work mode (e.g., Test, Live).</param>
        /// <returns>Configured TableLocation object for the specified work mode.</returns>
        public static ITableLocation CreateTableLocationByMode(WorkMode mode)
        {
            var tableLocation = PdoProperties<T>.GetTableLocationByWorkMode(mode);

            if (tableLocation == null)
                throw new InvalidOperationException($"Table location for WorkMode '{mode}' is not defined on {typeof(T).Name}.");

            return tableLocation;
        }

        /// <summary>
        /// Utility method to create ITableLocation for Live environment.
        /// </summary>
        public static ITableLocation CreateLiveTableLocation() => CreateTableLocationByMode(WorkMode.Live);

        /// <summary>
        /// Utility method to create ITableLocation for Test environment.
        /// </summary>
        public static ITableLocation CreateTestTableLocation() => CreateTableLocationByMode(WorkMode.Test);

        /// <summary>
        /// Utility method to create ITableLocation for Dummy environment.
        /// </summary>
        public static ITableLocation CreateDummyTableLocation() => CreateTableLocationByMode(WorkMode.Dummy);
    }
}