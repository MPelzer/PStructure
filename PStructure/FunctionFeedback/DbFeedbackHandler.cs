using System;
using System.Data;
using System.Data.Common;
using Optional;
using Optional.Unsafe;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    public delegate void DbAction(ref DbFeedback dbCom);
    public delegate void DbExceptionAction(ref DbFeedback dbCom, Exception ex);
    public delegate bool DbCondition(ref DbFeedback dbCom);
    /// <summary>
    /// Kapselt eine Menge an Funktionen auf datenbankspezifischen FunctionFeedback Daten.
    /// Grundregeln:
    ///             - Transaktionen:
    ///                             # Keine Transaktion von außen mitgegeben:
    ///                                                                     * Erstelle eine.
    ///                                                                     * Rollback ist Aufgabe des ItemManagers (Nach außen hin atomar wirken)
    ///                                                                     * Commit ist Aufgabe des ItemManagers
    ///                                                                     * Schließen der Connection am Ende
    ///                             # Transaktion wurde von außen mitgegeben:
    ///                                                                     * Rollback übernimmt der Konsument
    ///                                                                     * Commit ist Aufgabe des Konsumenten
    ///                                                                     * Schließen der Connection am Ende
    ///             - DbConnection:
    ///                             # Connection wurde nicht offen mitgegeben:
    ///                                                                     * öffnen
    ///                                                                     * schließen
    ///                             # Connection wurde offen mitgegeben:
    ///                                                                     * Schließen der Connection am Ende
    /// </summary>
    public static class DbFeedbackHandler
    {
        /// <summary>
        /// Stellt den Standardmechanismus/Standardverhalten dar, welcher rund um Datenbankzugriffe notwendig ist.
        /// Dazu gehören u.A das Öffnen temporärer <see cref="IDbTransaction"/> und das Füllen von <see cref="DbCom"/>
        /// mit den richtigen Rückmeldungen für den Konsumenten.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        /// <param name="commitCondition"></param>
        /// <param name="onFinally"></param>
        public static void ExecuteWithTransaction(
            ref DbFeedback dbFeedback, // Pass DbFeedback as reference
            DbAction action,
            DbExceptionAction onException,
            DbCondition commitCondition,
            Action onFinally = null)
        {
            var transactionStartedHere = true;
            try
            {
                dbFeedback.OpenConnection();
                dbFeedback.RequestAnswer = false;
                transactionStartedHere = StartTransactionIfNotAlready(ref dbFeedback);
                action(ref dbFeedback);
                dbFeedback.RequestAnswer = true;
                if (!transactionStartedHere || !commitCondition(ref dbFeedback)) return;
                dbFeedback.CommitTransaction();
            }
            catch (Exception exception)
            {
                dbFeedback.RequestAnswer = false;
                dbFeedback.RequestException = exception;
                if (transactionStartedHere)
                {
                    dbFeedback.RollbackTransaction();
                }
                onException?.Invoke(ref dbFeedback, exception);
            }
            finally
            {
                dbFeedback.CloseConnection();
                onFinally?.Invoke();
            }
        }

        private static bool StartTransactionIfNotAlready(ref DbFeedback dbFeedback)
        {
            if (dbFeedback.DbTransaction != null) return false;
            dbFeedback.BeginTransaction();
            return true;
        }
    }
}