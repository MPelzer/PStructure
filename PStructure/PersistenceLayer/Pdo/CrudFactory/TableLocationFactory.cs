using System;
using System.Linq;
using PStructure.Models;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.TableLocation
{
    /// <summary>
    /// Factory for retrieving table locations based on the model type and work mode.
    /// </summary>
    public static class TableLocationFactory<T>
    {
        /// <summary>
        /// Creates an ITableLocation instance based on the specified work mode.
        /// </summary>
        /// <param name="mode">The desired work mode (e.g., Test, Live).</param>
        /// <returns>Configured TableLocation object for the specified work mode.</returns>
        public static ITableLocation CreateTableLocation(WorkMode mode)
        {
            var tableLocation = PdoDataCache<T>.TableLocationData.FirstOrDefault(attr => attr.Mode == mode);

            if (tableLocation == null)
                throw new InvalidOperationException($"Table location for WorkMode '{mode}' is not defined on {typeof(T).Name}.");

            return tableLocation;
        }
    }
}