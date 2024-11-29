using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo;
using PStructure.PersistenceLayer.Pdo.CrudFactory;
using PStructure.PersistenceLayer.PdoToTableMapping;

namespace PStructure.PersistenceLayer
{
    public class PersistenceLayer
    {
        private readonly ILogger _logger;
        private readonly WorkMode _workMode;

        // Dictionary to store ItemManagers for each type
        private readonly ConcurrentDictionary<Type, IItemManager<object>> _itemManagers;

        public PersistenceLayer(WorkMode workMode, ILogger logger = null)
        {
            _workMode = workMode;
            _logger = logger;
            _itemManagers = new ConcurrentDictionary<Type, IItemManager<object>>();
        }

        /// <summary>
        /// Registers or retrieves an ItemManager for a given type and CrudType.
        /// </summary>
        public IItemManager<T> CreateItemManager<T>(CrudType crudType, WorkMode workMode)
        {
            if (_itemManagers.ContainsKey(typeof(T))) return (IItemManager<T>)_itemManagers[typeof(T)];
            
            var manager = PdoManagerFactory<T>.CreateItemManager(crudType, workMode, _logger);
            _itemManagers.TryAdd(typeof(T), (IItemManager<object>)manager);

            // Cast to IItemManager<T> when retrieving it
            return (IItemManager<T>)_itemManagers[typeof(T)];
        }

        /// <summary>
        /// Pre-generates ItemManagers for a set of types and CrudTypes.
        /// </summary>
        public void PreGenerateItemManagers(IEnumerable<(Type pdoType, CrudType CrudType)> types, WorkMode workMode)
        {
            foreach (var (pdoType, crudType) in types)
            {
                // Use reflection to dynamically invoke the generic CreateItemManager method
                var createMethod = GetType()
                    .GetMethod(nameof(CreateItemManager), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                    ?.MakeGenericMethod(pdoType);

                if (createMethod == null)
                {
                    throw new InvalidOperationException($"Unable to resolve CreateItemManager for type {pdoType}");
                }

                createMethod.Invoke(this, new object[] { crudType, workMode });
            }
        }
        
        /// <summary>
        /// Dynamically updates the Crud of a registered ItemManager.
        /// </summary>
        public void SetCrud<T>(CrudType newCrudType)
        {
            if (_itemManagers.TryGetValue(typeof(T), out var manager))
            {
                var typedManager = (IItemManager<T>)manager;
                PdoManagerFactory<T>.UpdateItemManagerCrud(typedManager, newCrudType, _workMode, _logger);
            }
            else
            {
                throw new InvalidOperationException($"No ItemManager registered for type {typeof(T)}");
            }
        }
    }
}
