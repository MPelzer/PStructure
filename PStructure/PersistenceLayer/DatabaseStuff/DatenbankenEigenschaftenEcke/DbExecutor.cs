using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

public static class DbExecutor
{
    public static int RunExecute<T>(
        ExecutionContext context,
        ILogger logger,
        IExecutionHandler<T> handler)
    {
        int result = 0;

        DbContextHandler.ExecuteWithTransaction(
            context,
            logger,
            (log, ctx) =>
            {
                handler.PrepareStatement(ctx);
                handler.PrepareParameters(ctx);
                handler.Validate(ctx);
                result = handler.Execute(ctx);
            },
            (ctx, ex) =>
            {
                logger?.LogError(ex, "DbExecutor: Execute failed.");
                throw;
            });

        return result;
    }

    public static IEnumerable<T> RunQuery<T>(
        ExecutionContext context,
        ILogger logger,
        IExecutionHandler<T> handler)
    {
        IEnumerable<T> result = Enumerable.Empty<T>();

        DbContextHandler.ExecuteWithTransaction(
            context,
            logger,
            (log, ctx) =>
            {
                handler.PrepareStatement(ctx);
                handler.PrepareParameters(ctx);
                handler.Validate(ctx);
                result = handler.Query(ctx);
            },
            (ctx, ex) =>
            {
                logger?.LogError(ex, "DbExecutor: Query failed.");
                throw;
            });

        return result;
    }
}