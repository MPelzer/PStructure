using System.Collections.Generic;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    /// <summary>
    /// A handler that executes database operations against a given execution context.
    /// </summary>
    public interface IExecutionHandler<TRequest, TResult>
        where TRequest : RequestContext
    {
        /// <summary>
        /// Executes a non-query operation (INSERT/UPDATE/DELETE).
        /// </summary>
        int Execute(ExecutionContext<TRequest> context);

        /// <summary>
        /// Executes a query operation (SELECT).
        /// </summary>
        IEnumerable<TResult> Query(ExecutionContext<TRequest> context);
    }
}