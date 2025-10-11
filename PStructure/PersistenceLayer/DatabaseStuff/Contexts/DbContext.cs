using System.Data;

public class DbContext : DbResult
{
    public IDbConnection DbConnection { get; set; }
    public IDbTransaction DbTransaction { get; set; }

    public DbContext(IDbConnection dbConnection)
    {
        DbConnection = dbConnection;
    }
}