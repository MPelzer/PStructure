using System.Data;

namespace PStructure.PersistenceLayer.PersistenceLayerFeedback
{
    /// <summary>
    /// </summary>
    public class DbContext : FunctionFeedback.DbResult
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;

        public DbContext(IDbConnection dbConnection)
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