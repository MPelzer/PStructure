using PStructure.CRUDs;
using PStructure.Mapper;
using PStructure.PersistenceLayer.Pdo.PdoToTableMapping.SimpleCrud;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.PdoToTableMapping
{
    public enum CrudType
    {
        [FactoryAttibutes.SqlGenerator(typeof(SimpleSqlGenerator<>))]
        [FactoryAttibutes.Crud(typeof(SimpleCrud<>))]
        [FactoryAttibutes.Mapper(typeof(SimpleMapper<>))]
        Simple,
    }
}