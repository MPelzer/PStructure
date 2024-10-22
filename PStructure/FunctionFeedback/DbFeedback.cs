using System;
using System.Data;
using System.Data.Common;
using Dapper;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    /// <summary>
    /// 
    /// </summary>
    public class DbFeedback : FunctionFeedback
    {
        // DbRequest methods
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        public DbFeedback(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        
        public void SetDbTransaction(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public IDbTransaction GetDbTransaction()
        {
            return _dbTransaction;
        }

        public void SetDbConnection(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IDbConnection GetDbConnection()
        {
            return _dbConnection;
        }
    }
}