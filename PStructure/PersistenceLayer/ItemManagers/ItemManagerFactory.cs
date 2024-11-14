using System;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.Models;

namespace PStructure.PersistenceLayer
{
    /// <summary>
    /// Factory for creating ItemManager instances based on WorkMode.
    /// </summary>
    public static class ItemManagerFactory<T> where T : class, new()
    {
        /// <summary>
        /// Creates an ItemManager instance for Test mode.
        /// </summary>
        public static ItemManager<T> CreateTestItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Test, logger);
        }

        /// <summary>
        /// Creates an ItemManager instance for Live mode.
        /// </summary>
        public static ItemManager<T> CreateLiveItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Live, logger);
        }

        /// <summary>
        /// Creates an ItemManager instance for Dummy mode.
        /// </summary>
        public static ItemManager<T> CreateDummyItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Dummy, logger);
        }

        /// <summary>
        /// General method to create an ItemManager instance based on specified WorkMode.
        /// </summary>
        /// <param name="mode">The desired WorkMode (Test, Live, Dummy, etc.).</param>
        /// <param name="logger">An optional logger for logging.</param>
        /// <returns>A configured instance of ItemManager.</returns>
        public static ItemManager<T> CreateItemManager(WorkMode mode, ILogger logger = null)
        {
            // Use the SimpleCrudFactory to create the appropriate ICrud instance based on WorkMode
            ICrud<T> crud = CrudFactory<T>.CreateCrud(mode, logger);

            // Return a new instance of ItemManager with the configured CRUD
            return new ItemManager<T>(crud, logger);
        }
    }
}