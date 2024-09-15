using System.Data;

namespace PStructure.Interfaces
{
    public interface IDbRequest
    {
        IDbConnection DbConnection { get; set; }
        IDbTransaction DbTransaction { get; set; }
        string InjectedSql { get; set; }

        void OpenConnection();
        void CloseConnection();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}