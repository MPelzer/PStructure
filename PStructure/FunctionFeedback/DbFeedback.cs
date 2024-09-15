using System;
using System.Data;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    /// <summary>
    /// Combines database request handling and feedback mechanisms into a single class.
    /// </summary>
    public class DbFeedback : IFunctionFeedback, IDbRequest
    {
        public bool RequestAnswer { get; set; }
        public Exception RequestException { get; set; }
        public bool SilentThrow { get; set; }
        
        // DbRequest methods
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public string InjectedSql { get; set; }

        public DbFeedback(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
            RequestAnswer = false;
            RequestException = null;
            SilentThrow = false;
        }
        
        
        public void OpenConnection()
        {
            if (DbConnection.State != ConnectionState.Open)
            {
                DbConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (DbConnection.State == ConnectionState.Open)
            {
                DbConnection.Close();
            }
        }

        public void BeginTransaction()
        {
            if (DbTransaction == null)
            {
                DbTransaction = DbConnection.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            DbTransaction?.Commit();
            DbTransaction = null;
        }

        public void RollbackTransaction()
        {
            DbTransaction?.Rollback();
            DbTransaction = null;
        }

        
    }
}