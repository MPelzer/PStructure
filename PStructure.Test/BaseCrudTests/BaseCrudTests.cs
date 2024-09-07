using NUnit.Framework;
using MySqlConnector;
using System;
using System.Data;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.root;
using PStructure.SqlGenerator;
using PStructure.TableLocation;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test
{
    [TestFixture]
    public class DefaultCrudTests
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
        private MySqlConnection _dbConnection;

        [SetUp]
        public void SetUp()
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
                Assert.Fail($"Failed to initialize connection or table: {ex.Message}");
            }
        }
        

        [TearDown]
        public void TearDown()
        {
            try
            {
                var entryFactory = new TestEntryFactory();
                entryFactory.TearDown(_dbConnection);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to drop table: {ex.Message}");
            }
            finally
            {
                _dbConnection?.Close();
                _dbConnection?.Dispose();
            }
        }
    }
}
