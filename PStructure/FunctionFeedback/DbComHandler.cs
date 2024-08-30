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
    /// Kapselt eine Menge an Funktionen auf datenbankspezifischen FunctionFeedback Daten.
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
    ///                                                                     * 
    /// </summary>
    public static class DbComHandler
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
                    dbCom._dbConnection.Close();
                }
                SetDbComAnswerValid(dbCom);
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
        
        /// <summary>
        /// Markiert die Rückgabe des <see cref="DbCom"/> als positiv  
        /// </summary>
        /// <param name="dbCom"></param>
        private static void SetDbComAnswerValid(DbCom dbCom)
        {
            dbCom.requestAnswer = true;
        }
        
        /// <summary>
        /// Öffnet die Datenbankverbindung im gegebenen <see cref="DbCom"/>, wenn sie noch nicht offen war.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        private static bool OpenConnectionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._dbConnection.State == ConnectionState.Open) return false;
            dbCom._dbConnection.Open();
            return true;
        }
        
        /// <summary>
        /// Startet die Transaktion im <see cref="DbCom"/>, sollte sie noch nicht gestartet worden sein.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        private static bool StartTransactionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._transaction.HasValue) return false;
            dbCom._transaction = Option.Some(dbCom._dbConnection.BeginTransaction());
            return true;
        }
        
        /// <summary>
        /// Fügt dem <see cref="DbCom"/> eine Exception hinzu. Dabei wird automatisch die Rückgabe auf negativ gesetzt.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <param name="exception"></param>
        private static void SetException(DbCom dbCom, Exception exception)
        {
            dbCom.requestAnswer = false;
            dbCom.requestException = Option.Some(exception);
        }
        
        /// <summary>
        /// Invalidiert die Änderungen der Transaktion in der <see cref="DbCom"/>
        /// </summary>
        /// <param name="dbCom"></param>
        private static void RollbackTransaction(DbCom dbCom)
        {
            dbCom._transaction.Match(
                some: transaction => transaction.Rollback(),
                none: () => { }
            );
        }
    }
}