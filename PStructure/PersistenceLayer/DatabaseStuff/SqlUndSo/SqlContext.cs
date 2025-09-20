using System.Collections.Generic;
using System.Data;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    /// <summary>
    /// Enthält SQL-bezogene Informationen wie das SQL-Statement,
    /// die SQL-Parameter und die gewählte Parametrisierungsmethode.
    /// </summary>
    public class SqlContext
    {
        /// <summary>
        /// Das SQL-Statement, das ausgeführt werden soll.
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// Die Liste der SQL-Parameter.
        /// Wird vorwiegend bei manueller Parametrisierung verwendet.
        /// </summary>
        public List<IDataParameter> Parameters { get; set; } = new();

        /// <summary>
        /// Gibt an, wie die SQL-Parameter erzeugt und verarbeitet werden.
        /// </summary>
        public SqlParameterizingType ParameterizingType { get; set; } = SqlParameterizingType.Named;

        public SqlExecutionMode ExecutionMode { get; set; } = SqlExecutionMode.AutoFromItem;
        
        /// <summary>
        /// Fügt einen Parameter zur Parameterliste hinzu.
        /// </summary>
        public void AddParameter(IDataParameter parameter)
        {
            Parameters.Add(parameter);
        }
    }
}