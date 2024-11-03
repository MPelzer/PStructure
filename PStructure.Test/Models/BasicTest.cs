using System;
using MySqlConnector;
using PStructure.Test.Models;

namespace PStructure.Test;

public class BasicTest
{
    private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
    private MySqlConnection _dbConnection;

    
    public void SetUpDatabase()
    {
        try
        {
            _dbConnection = new MySqlConnection(ConnectionString);
            _dbConnection.Open();
            var entryFactory = new TestEntryFactory();
            entryFactory.intitalizeDatabaseTable(_dbConnection);
        }
        catch (Exception ex)
        {
            // ignored
        }
        finally
        {
            _dbConnection?.Close();
            _dbConnection?.Dispose();
        }
    }
        

    
    public void TearDownDatabase()
    {
        try
        {
            var entryFactory = new TestEntryFactory();
            entryFactory.TearDown(_dbConnection);
        }
        catch (Exception ex)
        {
            // ignored
        }
        finally
        {
            _dbConnection?.Close();
            _dbConnection?.Dispose();
        }
    }
}