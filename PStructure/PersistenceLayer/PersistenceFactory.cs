using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.Mapper;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure.PersistenceLayer
{
    public static class PersistenceFactory<T> where T : new()
    {
        /// <summary>
        /// Erstellt eine ItemManager-Instanz für den Testmodus.
        /// </summary>
        public static ItemManager<T> CreateTestItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Test, logger);
        }

        /// <summary>
        /// Erstellt eine ItemManager-Instanz für den Livemodus.
        /// </summary>
        public static ItemManager<T> CreateLiveItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Live, logger);
        }

        /// <summary>
        /// Erstellt eine ItemManager-Instanz für den Dummy-Modus.
        /// </summary>
        public static ItemManager<T> CreateDummyItemManager(ILogger logger = null)
        {
            return CreateItemManager(WorkMode.Dummy, logger);
        }

        /// <summary>
        /// Zentrale Methode zur Erstellung eines ItemManagers basierend auf dem Modus.
        /// </summary>
        private static ItemManager<T> CreateItemManager(WorkMode mode, ILogger logger)
        {
            // Konfiguriert den Tabellenstandort basierend auf dem Arbeitsmodus
            ITableLocation tableLocation = TableLocationFactory<T>.CreateTableLocationByMode(mode);

            // Richtet die Abhängigkeiten mit dem Tabellenstandort ein
            var sqlGenerator = new SimpleSqlGenerator<T>();
            IMapperPdoQuery<T> mapperPdoQuery = new SimpleMapperPdoQuery<T>();
            ICrud<T> crud = new SimpleCrud<T>(sqlGenerator, mapperPdoQuery, tableLocation);

            // Gibt die konfigurierte ItemManager-Instanz zurück
            return new ItemManager<T>(crud, logger);
        }
    }
}