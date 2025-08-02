using System;
using System.Data;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling
{
    public delegate void DbAction(ILogger logger, IExecutionContext context);
    public delegate void DbExceptionAction(IExecutionContext context, Exception ex);

    /// <summary>
    /// Führt eine DB-Operation im Transaktionskontext aus und verwaltet Feedback.
    /// </summary>
    public static class DbFeedbackHandler
    {
        public static void ExecuteWithTransaction(
            IExecutionContext context,
            ILogger logger,
            DbAction action,
            DbExceptionAction onException,
            Action onFinally = null)
        {
            var transactionStartedHere = true;

            try
            {
                transactionStartedHere = PrepareDbContext(context, logger);
                action(logger, context);
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

        private static bool PrepareDbContext(IExecutionContext context, ILogger logger)
        {
            var conn = context.DbContext.DbConnection;

            if (conn.State != ConnectionState.Open)
            {
                logger?.LogDebug("DbFeedbackHandler: Opening DB connection.");
                conn.Open();
            }

            context.DbContext.RequestAnswer = false;

            if (context.DbContext.DbTransaction != null)
            {
                logger?.LogDebug("DbFeedbackHandler: Reusing existing transaction.");
                return false;
            }

            logger?.LogDebug("DbFeedbackHandler: Starting new transaction.");
            context.DbContext.DbTransaction = conn.BeginTransaction();
            return true;
        }

        private static void ApplyException(bool startedHere, Exception exception, IExecutionContext context, ILogger logger)
        {
            context.DbContext.RequestAnswer = false;
            context.DbContext.RequestException = exception;

            logger?.LogError(exception, "DbFeedbackHandler: Exception during transaction.");

            if (!startedHere) return;

            context.DbContext.DbTransaction?.Rollback();
            logger?.LogDebug("DbFeedbackHandler: Transaction rolled back.");

            context.DbContext.DbTransaction = null;
        }

        private static void ApplyActionResult(bool startedHere, IExecutionContext context, ILogger logger)
        {
            context.DbContext.RequestAnswer = true;

            if (!startedHere) return;

            context.DbContext.DbTransaction?.Commit();
            logger?.LogDebug("DbFeedbackHandler: Transaction committed.");

            context.DbContext.DbTransaction = null;
        }

        private static void PostprocessDbContext(IExecutionContext context, ILogger logger)
        {
            var conn = context.DbContext.DbConnection;

            if (conn.State == ConnectionState.Open)
            {
                logger?.LogDebug("DbFeedbackHandler: Closing DB connection.");
                conn.Close();
            }
        }
    }
}
