using System;
using System.Data;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    public class DbRequest : IDbRequest
    {
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public string InjectedSql { get; set; }

        public DbRequest(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
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