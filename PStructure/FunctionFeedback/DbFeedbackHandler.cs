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
        /// Dazu gehören u.A das Öffnen temporärer <see cref="IDbTransaction"/> und das Füllen von <see cref="DbFeedback"/>
        /// mit den richtigen Rückmeldungen für den Konsumenten.
        /// </summary>
        /// <param name="dbFeedback"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        /// <param name="onFinally"></param>
        public static void ExecuteWithTransaction(
            ref DbFeedback dbFeedback, 
            DbAction action,
            DbExceptionAction onException,
            Action onFinally = null)
        {
            var transactionStartedHere = true;
            try
            {
                transactionStartedHere = PrepareDbFeedback(ref dbFeedback);
                action(ref dbFeedback);
                ApplyActionResult(transactionStartedHere, ref dbFeedback);
            }
            catch (Exception exception)
            {
                ApplyException(transactionStartedHere, exception, ref dbFeedback);
                onException?.Invoke(ref dbFeedback, exception);
            }
            finally
            {
                PostprocessDbFeedback(ref dbFeedback);
                onFinally?.Invoke();
            }
            
        }

        private static void ApplyException(bool transactionStartedHere, Exception exception, ref DbFeedback dbFeedback)
        {
            dbFeedback.SetRequestAnswer(false);
            dbFeedback.SetRequestException(exception);
            if (!transactionStartedHere) return;
            dbFeedback.GetDbTransaction()?.Rollback();
            dbFeedback.SetDbTransaction(null);
        }

        private static void ApplyActionResult(bool transactionStartedHere, ref DbFeedback dbFeedback)
        {
            dbFeedback.SetRequestAnswer(true);
            if (!transactionStartedHere) return;
            dbFeedback.GetDbTransaction()?.Commit();
            dbFeedback.SetDbTransaction(null);
        }

        private static bool PrepareDbFeedback(ref DbFeedback dbFeedback)
        {
            if (dbFeedback.GetDbConnection().State != ConnectionState.Open)
            {
                dbFeedback.GetDbConnection().Open();
            }
            dbFeedback.SetRequestAnswer(false);
            if (dbFeedback.GetDbTransaction() != null) return false;
            dbFeedback.SetDbTransaction(dbFeedback.GetDbConnection().BeginTransaction());
            return true;
        }

        private static void PostprocessDbFeedback(ref DbFeedback dbFeedback)
        {
            if (dbFeedback.GetDbConnection().State == ConnectionState.Open)
            {
                dbFeedback.GetDbConnection().Close();
            }
        }
    }
}