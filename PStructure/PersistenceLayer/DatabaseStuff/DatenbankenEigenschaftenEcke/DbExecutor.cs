using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.Handler;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;

public static class DbExecutor
{
    public static int RunExecute<TRequest, TResult>(
        ExecutionContext<TRequest> context,
        IExecutionHandler<TRequest, TResult> handler,
        ILogger logger)
        where TRequest : RequestContext
    {
        int result = 0;

        DbContextHandler.ExecuteWithTransaction(
            context,
            (log, ctx) => result = handler.Execute(ctx),
            (ctx, ex) => logger?.LogError(ex, "DbExecutor: Execute failed."),
            logger);

        return result;
    }

    public static IEnumerable<TResult> RunQuery<TRequest, TResult>(
        ExecutionContext<TRequest> context,
        IExecutionHandler<TRequest, TResult> handler,
        ILogger logger)
        where TRequest : RequestContext
    {
        IEnumerable<TResult> result = Enumerable.Empty<TResult>();

        DbContextHandler.ExecuteWithTransaction(
            context,
            (log, ctx) => result = handler.Query(ctx),
            (ctx, ex) => logger?.LogError(ex, "DbExecutor: Query failed."),
            logger);

        return result;
    }
}