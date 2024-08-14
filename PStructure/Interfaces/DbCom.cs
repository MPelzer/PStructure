using System;
using System.Data;
using System.Transactions;
using Optional;

namespace PStructure.Interfaces
{
    public class DbCom
    {
        public IDbConnection  _dbConnection;

        public Option<Transaction> _transaction;

        public bool requestAnswer;

        public Option<Exception> requestException;

        public Option<ICompoundPrimaryKey> compoundPrimaryKey;
    }
}