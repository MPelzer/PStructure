using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    /// <summary>
    /// Delegate type for executing a database action within a managed transaction scope.
    /// </summary>
    public delegate void DbAction<TRequest>(ILogger logger, ExecutionContext<TRequest> context)
        where TRequest : RequestContext;

    /// <summary>
    /// Delegate type for handling exceptions during transaction execution.
    /// </summary>
    public delegate void DbExceptionAction<TRequest>(ExecutionContext<TRequest> context, Exception ex)
        where TRequest : RequestContext;

    /// <summary>
    /// Manages connection and transaction lifecycle for database operations.
    /// Handles transaction commit, rollback, and connection open/close automatically.
    /// </summary>
    public static class DbContextHandler
    {
        /// <summary>
        /// Executes a database operation with transaction management, logging, and cleanup.
        /// </summary>
        public static void ExecuteWithTransaction<TRequest>(
            ExecutionContext<TRequest> context,
            DbAction<TRequest> action,
            DbExceptionAction<TRequest>? onException = null,
            ILogger? logger = null,
            Action? onFinally = null)
            where TRequest : RequestContext
        {
            bool transactionStartedHere = true;

            try
            {
                transactionStartedHere = PrepareDbContext(context, logger);
                action(logger ?? context.Logger, context);
                ApplyActionResult(transactionStartedHere, context, logger);
            }
            catch (Exception exception)
            {
                ApplyException(transactionStartedHere, exception, context, logger);
                onException?.Invoke(context, exception);
            }
            finally
            {
                PostprocessDbContext(context, logger);
                onFinally?.Invoke();
            }
        }

        #region 🔧 Internal Transaction Management

        private static bool PrepareDbContext<TRequest>(ExecutionContext<TRequest> context, ILogger? logger)
            where TRequest : RequestContext
        {
            var conn = context.DbContext.DbConnection;

            if (conn.State != ConnectionState.Open)
            {
                logger?.LogDebug("DbContextHandler: Opening DB connection.");
                conn.Open();
            }

            context.DbContext.RequestAnswer = false;

            if (context.DbContext.DbTransaction != null)
            {
                logger?.LogDebug("DbContextHandler: Reusing existing transaction.");
                return false;
            }

            logger?.LogDebug("DbContextHandler: Starting new transaction.");
            context.DbContext.DbTransaction = conn.BeginTransaction();
            return true;
        }

        private static void ApplyActionResult<TRequest>(
            bool startedHere,
            ExecutionContext<TRequest> context,
            ILogger? logger)
            where TRequest : RequestContext
        {
            context.DbContext.RequestAnswer = true;

            if (!startedHere) return;

            context.DbContext.DbTransaction?.Commit();
            logger?.LogDebug("DbContextHandler: Transaction committed.");

            context.DbContext.DbTransaction = null;
        }

        private static void ApplyException<TRequest>(
            bool startedHere,
            Exception exception,
            ExecutionContext<TRequest> context,
            ILogger? logger)
            where TRequest : RequestContext
        {
            context.DbContext.RequestAnswer = false;
            context.DbContext.RequestException = exception;

            logger?.LogError(exception, "DbContextHandler: Exception during transaction.");

            if (!startedHere) return;

            context.DbContext.DbTransaction?.Rollback();
            logger?.LogDebug("DbContextHandler: Transaction rolled back.");

            context.DbContext.DbTransaction = null;
        }

        private static void PostprocessDbContext<TRequest>(
            ExecutionContext<TRequest> context,
            ILogger? logger)
            where TRequest : RequestContext
        {
            var conn = context.DbContext.DbConnection;

            if (conn.State == ConnectionState.Open)
            {
                logger?.LogDebug("DbContextHandler: Closing DB connection.");
                conn.Close();
            }
        }

        #endregion
    }
}
