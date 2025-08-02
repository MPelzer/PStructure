using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke
{
    namespace PStructure.PersistenceLayer.DatabaseStuff
    {
        public enum SqlParameterizingType
        {
            Named,
            Indexed
        }

        public enum ProcessingType
        {
            Single,
            Bulk
        }
    }
    public enum SqlExecutionMode
    {
        AutoFromItem,       // From T + attribute-mapped
        ManualFromSql,      // Manually written SQL + parameters
        ManualFromSqlAndObjectArray // Like Insert into t(a,b) values (?, ?) — positional
    }
    /// <summary>
    /// Basisinterface für den Ausführungskontext.
    /// </summary>
    public interface IExecutionContext
    {
        DbContext DbContext { get; set; }
        SqlContext SqlContext { get; set; }
        ILogger Logger { get; set; }

        void Validate();
        void AddValidationError(string message);
        void AddValidationError(System.Exception exception);
        bool HasErrors { get; }
    }
}