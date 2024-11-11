using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.Mapper;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.TableLocation;
using PStructure.Models;

namespace PStructure.PersistenceLayer
{
    /// <summary>
    /// Factory for creating SimpleCrud instances based on WorkMode.
    /// </summary>
    public static class SimpleCrudFactory<T> where T : class, new()
    {
        /// <summary>
        /// Creates a SimpleCrud instance for Test mode.
        /// </summary>
        public static ICrud<T> CreateTestCrud(ILogger logger = null)
        {
            return CreateCrud(WorkMode.Test, logger);
        }

        /// <summary>
        /// Creates a SimpleCrud instance for Live mode.
        /// </summary>
        public static ICrud<T> CreateLiveCrud(ILogger logger = null)
        {
            return CreateCrud(WorkMode.Live, logger);
        }

        /// <summary>
        /// Creates a SimpleCrud instance for Dummy mode.
        /// </summary>
        public static ICrud<T> CreateDummyCrud(ILogger logger = null)
        {
            return CreateCrud(WorkMode.Dummy, logger);
        }

        /// <summary>
        /// General method for creating a SimpleCrud instance based on the specified WorkMode.
        /// </summary>
        /// <param name="mode">The desired WorkMode (Test, Live, Dummy).</param>
        /// <param name="logger">An optional logger for logging purposes.</param>
        /// <returns>A configured instance of SimpleCrud.</returns>
        public static ICrud<T> CreateCrud(WorkMode mode, ILogger logger = null)
        {
            // Retrieve table location based on the mode
            ITableLocation tableLocation = TableLocationFactory<T>.CreateTableLocationByMode(mode);

            // Set up dependencies based on the table location
            ISqlGenerator<T> sqlGenerator = new SimpleSqlGenerator<T>();
            IMapperPdoQuery<T> mapperPdoQuery = new SimpleMapperPdoQuery<T>();

            // Return the configured SimpleCrud instance
            return new SimpleCrud<T>(sqlGenerator, mapperPdoQuery, tableLocation);
        }
    }
}
