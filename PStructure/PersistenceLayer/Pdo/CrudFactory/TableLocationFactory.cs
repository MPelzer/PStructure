using System;
using System.Linq;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;

namespace PStructure.PersistenceLayer.Pdo.CrudFactory
{
    /// <summary>
    ///     Factory for retrieving table locations based on the model type and work mode.
    /// </summary>
    public static class TableLocationFactory<T>
    {
        /// <summary>
        ///     Creates an ITableLocation instance based on the specified work mode.
        /// </summary>
        /// <param name="mode">The desired work mode (e.g., Test, Live).</param>
        /// <returns>Configured TableLocation object for the specified work mode.</returns>
        public static ITableLocation CreateTableLocation(WorkMode mode)
        {
            // Attempt to retrieve the TableLocation from the cache or configuration dynamically
            try
            {
                var tableLocation = PdoDataCache<T>.GetTableLocation(mode); // Dynamically loads and caches if needed
                return tableLocation;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Table location for WorkMode '{mode}' is not defined for {typeof(T).Name}.");
            }
        }
    }
}