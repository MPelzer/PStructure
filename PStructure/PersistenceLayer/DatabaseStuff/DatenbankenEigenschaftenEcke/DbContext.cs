using System.Data;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke
{
    /// <summary>
    /// Repräsentiert den Datenbankkontext mit Verbindung und Transaktion.
    /// </summary>
    public class DbContext : DbResult
    {
        public IDbConnection DbConnection { get; set; }

        public IDbTransaction DbTransaction { get; set; }

        public ProcessingType ProcessingType { get; set; } = ProcessingType.Single;

        public DbContext(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }
    }
}