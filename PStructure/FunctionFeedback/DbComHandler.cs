using System;
using System.Data;
using Optional;
using Optional.Unsafe;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    public delegate void DbAction(ref DbCom dbCom);
    public delegate void DbExceptionAction(ref DbCom dbCom, Exception ex);
    public delegate bool DbCondition(ref DbCom dbCom);
    /// <summary>
    /// Kapselt eine Menge an Funktionen datenbankspezifischen FunctionFeedback Daten.
    /// Grundregeln:
    ///             - Transaktionen:
    ///                             # Keine Transaktion von außen mitgegeben:
    ///                                                                     * Erstelle eine.
    ///                                                                     * Rollback ist Aufgabe des ItemManagers (Nach außen hin atomar wirken)
    ///                                                                     * Commit ist Aufgabe des ItemManagers
    ///                             # Transaktion wurde von außen mitgegeben:
    ///                                                                     * Rollback übernimmt der Konsument
    ///                                                                     * Commit ist Aufgabe des Konsumenten
    ///             - DbConnection:
    ///                             # Connection wurde nicht offen mitgegeben:
    ///                                                                     * öffnen
    ///                                                                     * offen lassen
    ///                             # Connection wurde offen mitgegeben:
    ///                                                                     * nichts tun
    /// </summary>
    public static class DbComHandler
    {
        public static void ExecuteWithTransaction(
            ref DbCom dbCom,
            DbAction action,
            DbExceptionAction onException,
            DbCondition commitCondition,
            Action onFinally = null) 
        {
            var transactionStartedHere = true;
            try
            {
                OpenConnectionIfNotAlready(dbCom);
                
                transactionStartedHere = StartTransactionIfNotAlready(dbCom);
                action(ref dbCom);

                if (transactionStartedHere && commitCondition(ref dbCom))
                {
                    dbCom._transaction.ValueOrDefault()?.Commit();
                }
                SetAnswerValid(dbCom);
            }
            catch (Exception exception)
            {
                SetException(dbCom, exception);
                if (transactionStartedHere)
                {
                    RollbackTransaction(dbCom);
                }
                onException?.Invoke(ref dbCom, exception);
            }
            finally
            {
                onFinally?.Invoke();
            }
        }

        private static void SetAnswerValid(DbCom dbCom)
        {
            dbCom.requestAnswer = true;
        }

        private static bool OpenConnectionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._dbConnection.State == ConnectionState.Open) return false;
            dbCom._dbConnection.Open();
            return true;
        }
        private static bool StartTransactionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._transaction.HasValue) return false;
            dbCom._transaction = Option.Some(dbCom._dbConnection.BeginTransaction());
            return true;
        }

        public static void SetException(DbCom dbCom, Exception exception)
        {
            dbCom.requestAnswer = false;
            dbCom.requestException = Option.Some(exception);
        }
        
        public static void RollbackTransaction(DbCom dbCom)
        {
            dbCom._transaction.Match(
                some: transaction => transaction.Rollback(),
                none: () => { }
            );
        }
    }
}