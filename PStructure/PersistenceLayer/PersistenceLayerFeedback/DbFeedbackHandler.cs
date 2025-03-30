using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace PStructure.FunctionFeedback
{
    public delegate void DbAction(ILogger logger, ref DbFeedback dbCom);

    public delegate void DbExceptionAction(ref DbFeedback dbCom, Exception ex);

    public static class DbFeedbackHandler
    {
        public static void ExecuteWithTransaction(
            ref DbFeedback dbFeedback,
            ILogger logger,
            DbAction action,
            DbExceptionAction onException,
            Action onFinally = null)
        {
            var transactionStartedHere = true;
            try
            {
                transactionStartedHere = PrepareDbFeedback(ref dbFeedback, logger);
                action(logger, ref dbFeedback);
                ApplyActionResult(transactionStartedHere, ref dbFeedback, logger);
            }
            catch (Exception exception)
            {
                ApplyException(transactionStartedHere, exception, ref dbFeedback, logger);
                onException?.Invoke(ref dbFeedback, exception);
            }
            finally
            {
                PostprocessDbFeedback(ref dbFeedback, logger);
                onFinally?.Invoke();
            }
        }

        private static void ApplyException(bool transactionStartedHere, Exception exception, ref DbFeedback dbFeedback,
            ILogger logger)
        {
            dbFeedback.SetRequestAnswer(false);
            dbFeedback.SetRequestException(exception);
            logger?.LogError(exception,
                "DbFeedbackHandler.ApplyException: An error occurred during database transaction.");
            if (!transactionStartedHere) return;
            dbFeedback.GetDbTransaction()?.Rollback();
            logger?.LogDebug("DbFeedbackHandler.ApplyException: Transaction rolled back.");
            dbFeedback.SetDbTransaction(null);
            logger?.LogDebug("DbFeedbackHandler.ApplyException: Transaction removed.");
        }

        private static void ApplyActionResult(bool transactionStartedHere, ref DbFeedback dbFeedback, ILogger logger)
        {
            dbFeedback.SetRequestAnswer(true);
            if (!transactionStartedHere) return;
            dbFeedback.GetDbTransaction()?.Commit();
            logger?.LogDebug("DbFeedbackHandler.ApplyActionResult: Committed the transaction.");
            dbFeedback.SetDbTransaction(null);
            logger?.LogDebug("DbFeedbackHandler.ApplyActionResult: Set transaction back to null.");
        }

        private static bool PrepareDbFeedback(ref DbFeedback dbFeedback, ILogger logger)
        {
            if (dbFeedback.GetDbConnection().State != ConnectionState.Open)
            {
                logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Opening the database connection.");
                dbFeedback.GetDbConnection().Open();
            }

            dbFeedback.SetRequestAnswer(false);

            if (dbFeedback.GetDbTransaction() != null)
            {
                logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Using the given transaction.");
                return false;
            }

            logger?.LogDebug("DbFeedbackHandler.PrepareDbFeedback: Starting a new transaction.");
            dbFeedback.SetDbTransaction(dbFeedback.GetDbConnection().BeginTransaction());
            return true;
        }

        private static void PostprocessDbFeedback(ref DbFeedback dbFeedback, ILogger logger)
        {
            if (dbFeedback.GetDbConnection().State != ConnectionState.Open) return;
            logger?.LogDebug("DbFeedbackHandler.PostprocessDbFeedback: Closing the database connection.");
            dbFeedback.GetDbConnection().Close();
        }
    }
}