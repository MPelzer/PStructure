using System;
using System.Data;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;

namespace PStructure.PersistenceLayer.DatabaseStuff
{
    public delegate void DbAction(ILogger logger, ref DbContext dbCom);

    public delegate void DbExceptionAction(ref DbContext dbCom, Exception ex);

    public static class DbFeedbackHandler
    {
        public static void ExecuteWithTransaction(
            ref DbContext dbContext,
            ILogger logger,
            DbAction action,
            DbExceptionAction onException,
            Action onFinally = null)
        {
            var transactionStartedHere = true;
            try
            {
                transactionStartedHere = PrepareDbFeedback(ref dbContext, logger);
                action(logger, ref dbContext);
                ApplyActionResult(transactionStartedHere, ref dbContext, logger);
            }
            catch (Exception exception)
            {
                ApplyException(transactionStartedHere, exception, ref dbContext, logger);
                onException?.Invoke(ref dbContext, exception);
            }
            finally
            {
                PostprocessDbFeedback(ref dbContext, logger);
                onFinally?.Invoke();
            }
        }

        private static void ApplyException(bool transactionStartedHere, Exception exception, ref DbContext dbContext,
            ILogger logger)
        {
            dbContext.SetRequestAnswer(false);
            dbContext.SetRequestException(exception);
            logger?.LogError(exception,
                "DbFeedbackHandler.ApplyException: An error occurred during database transaction.");
            if (!transactionStartedHere) return;
            dbContext.GetDbTransaction()?.Rollback();
            logger?.LogDebug("DbFeedbackHandler.ApplyException: Transaction rolled back.");
            dbContext.SetDbTransaction(null);
            logger?.LogDebug("DbFeedbackHandler.ApplyException: Transaction removed.");
        }

        private static void ApplyActionResult(bool transactionStartedHere, ref DbContext dbContext, ILogger logger)
        {
            dbContext.SetRequestAnswer(true);
            if (!transactionStartedHere) return;
            dbContext.GetDbTransaction()?.Commit();
            logger?.LogDebug("DbFeedbackHandler.ApplyActionResult: Committed the transaction.");
            dbContext.SetDbTransaction(null);
            logger?.LogDebug("DbFeedbackHandler.ApplyActionResult: Set transaction back to null.");
        }

        private static bool PrepareDbFeedback(ref DbContext dbContext, ILogger logger)
        {
            if (dbContext.GetDbConnection().State != ConnectionState.Open)
            {
                logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Opening the database connection.");
                dbContext.GetDbConnection().Open();
            }

            dbContext.SetRequestAnswer(false);

            if (dbContext.GetDbTransaction() != null)
            {
                logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Using the given transaction.");
                return false;
            }

            logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Starting a new transaction.");
            dbContext.SetDbTransaction(dbContext.GetDbConnection().BeginTransaction());
            return true;
        }

        private static void PostprocessDbFeedback(ref DbContext dbContext, ILogger logger)
        {
            if (dbContext.GetDbConnection().State != ConnectionState.Open) return;
            logger?.LogDebug("DbFeedbackHandler.PostprocessDbFeedback: Closing the database connection.");
            dbContext.GetDbConnection().Close();
        }
    }
}