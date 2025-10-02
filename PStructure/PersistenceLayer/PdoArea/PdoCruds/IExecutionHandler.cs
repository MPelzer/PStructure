using System.Collections.Generic;
using Dapper;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    /// <summary>
    /// Abstraction for execution strategies. 
    /// Each implementation knows how to prepare SQL, bind parameters, validate, and execute/query.
    /// </summary>
    public interface IExecutionHandler<T>
    {
        /// <summary>
        /// Prepares the SQL statement(s).
        /// </summary>
        void PrepareStatement(ExecutionContext context);

        /// <summary>
        /// Prepares Dapper-compatible parameters.
        /// </summary>
        void PrepareParameters(ExecutionContext context);

        /// <summary>
        /// Optional validation of prepared state before execution.
        /// </summary>
        void Validate(ExecutionContext context);

        /// <summary>
        /// Execute a non-query command (INSERT/UPDATE/DELETE).
        /// </summary>
        int Execute(ExecutionContext context);

        /// <summary>
        /// Run a SELECT query returning T.
        /// </summary>
        IEnumerable<T> Query(ExecutionContext context);
    }
}