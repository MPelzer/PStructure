using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.Mapper;
using PStructure.PersistenceLayer;
using PStructure.SqlGenerator;

namespace PStructure.root
{
    public class PersistenceFactory<T>
    {
        PersistenceFactory()
        {
        }
        
        ItemManager<T> CreateTestItemManagerWithSimpleCrud(ILogger logger)
        {

            var sqlGenerator = new SimpleSqlGenerator<T>(logger);
            var mapper = new MapperPdoQuery<T>();
            
            
            var simpleCrud = new SimpleCrud<T>()
            return
        }
    }
}