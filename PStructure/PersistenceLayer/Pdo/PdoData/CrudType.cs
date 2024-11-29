
using PStructure.PersistenceLayer.Pdo.PdoToTableMapping.SimpleCrud;
using PStructure.PersistenceLayer.Utils;

namespace PStructure.PersistenceLayer.PdoToTableMapping
{
    /// <summary>
    /// Konfiguration, welche Zusammenstellung aus Mappingkomponenten dem PdoManager zur hand gegeben werden.
    /// Einschränkungen: Auch hier wurde absichtlich ein Enum gewählt, da das Grundverhalten der Persistenzschicht einfach und nachvollziehbar bleiben soll.
    /// Menge an Szenarien:
    /// -   Einfache Crud: Erste Generation, die zunächst der Entwicklung dienen soll und somit einfach und transparent sein muss.
    ///  
    /// </summary>
    public enum CrudType
    {
        [FactoryAttibutes.SqlGenerator(typeof(SimpleSqlGenerator<>))]
        [FactoryAttibutes.Crud(typeof(SimpleCrud<>))]
        [FactoryAttibutes.Mapper(typeof(SimpleMapper<>))]
        Simple,
    }
}