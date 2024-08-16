using System;
using System.Data;
using System.Transactions;
using Optional;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback{
    
    /// <summary>
    /// Kapselt eine Menge an Funktionen datenbankspezifischen FunctionFeedback Daten.
    /// </summary>
    public static class DbComHandler
    {
        public static void SetAnswerValid(DbCom dbCom)
        {
            dbCom.requestAnswer = true;
        }
        
        public static bool OpenConnectionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._dbConnection.State == ConnectionState.Open) return false;
            dbCom._dbConnection.Open();
            return true;
        }

        public static bool StartTransactionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._transaction.HasValue) return false;
            dbCom._transaction = Option.Some(new TransactionScope());
            return true;
        }

        public static void SetException(DbCom dbCom, Exception exception)
        {
            dbCom.requestAnswer = false;
            dbCom.requestException = Option.Some(exception);
        }

        public static bool CommitIfAnswerValid(DbCom dbCom)
        {
            if (!dbCom.requestAnswer) return false;
            dbCom._transaction.Match(
                some: transactionScope => transactionScope.Complete(),
                none: () => { } 
            );
            return true;
        }

        public static void CloseConnectionIfNotAlready(DbCom dbCom)
        {
            if (dbCom._dbConnection.State == ConnectionState.Open)
            {
                dbCom._dbConnection.Close();
            }
        }

        public static void RollbackTransaction(DbCom dbCom)
        {
            dbCom._transaction.Match(
                some: transactionScope => transactionScope.Dispose(),
                none: () => { }
            );
        }
    }
}