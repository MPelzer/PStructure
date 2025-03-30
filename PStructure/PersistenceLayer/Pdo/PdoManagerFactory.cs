using System;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.CrudFactory;
using PStructure.PersistenceLayer.PdoToTableMapping;
using PStructure.TableLocation;

namespace PStructure.PersistenceLayer.Pdo
{
    public static class PdoManagerFactory<T>
    {
        /// <summary>
        ///     Creates an ItemManager instance dynamically using the provided CrudType and TableLocation.
        /// </summary>
        public static IItemManager<T> CreateItemManager(
            CrudType crudType,
            WorkMode workMode,
            ILogger logger = null)
        {
            var tableLocation = TableLocationFactory<T>.CreateTableLocation(workMode);
            var sqlGenerator = SqlGeneratorFactory<T>.GetSqlGenerator(crudType, tableLocation);
            var mapper = MapperFactory<T>.GetMapper(crudType);
            var crud = CrudFactory<T>.GetCrud(crudType, sqlGenerator, mapper);

            return new PdoManager<T>(crud, logger);
        }

        /// <summary>
        ///     Updates the Crud of an existing ItemManager with a new CrudType and TableLocation.
        /// </summary>
        public static void UpdateItemManagerCrud(
            IItemManager<T> itemManager,
            CrudType newCrudType,
            WorkMode workMode,
            ILogger logger = null)
        {
            if (itemManager == null)
                throw new ArgumentNullException(nameof(itemManager), "ItemManager instance cannot be null.");

            var tableLocation = TableLocationFactory<T>.CreateTableLocation(workMode);
            var sqlGenerator = SqlGeneratorFactory<T>.GetSqlGenerator(newCrudType, tableLocation);
            var mapper = MapperFactory<T>.GetMapper(newCrudType);
            var newCrud = CrudFactory<T>.GetCrud(newCrudType, sqlGenerator, mapper);

            itemManager.SetCrud(newCrud);
        }
    }
}