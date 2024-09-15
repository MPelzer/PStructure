using System;
using MySqlConnector;
using NUnit.Framework;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.SqlGenerator;
using PStructure.TableLocation;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest;

[TestFixture]
public class DeleteTests
{
    private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
    private MySqlConnection _dbConnection;
    private TestEntryFactory _testEntryFactory;

    [SetUp]
    public void SetUp()
    {
        try
        {
            _dbConnection = new MySqlConnection(ConnectionString);
            _dbConnection.Open();
            _testEntryFactory = new TestEntryFactory();
            _testEntryFactory.intitalizeDatabaseTable(_dbConnection);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to initialize connection or table: {ex.Message}");
        }
    }

    [Test]
    public void DeleteByPrimaryKey_Should_RemoveDataCorrectly()
    {
        // Insert initial data
        var testEntry = new TestEntry
        {
            GuidValue = Guid.NewGuid(),
            IntegerValue = 123,
            LongValue = 1234567890L,
            ShortValue = 12345,
            ByteValue = 255,
            FloatValue = 123.45f,
            DoubleValue = 12345.6789,
            DecimalValue = 123456.78m,
            BooleanValue = true,
            CharValue = 'A',
            StringValue = "Test String",
            DateTimeValue = DateTime.Now,
            ByteArrayValue = new byte[] { 1, 2, 3, 4, 5 }
        };

        var logger = _testEntryFactory.GetTestLogger();
        var tableLocation = new BaseTableLocation("", "TestEntry");
        var itemManager = new DefaultItemManager<TestEntry>(tableLocation, logger);
        var dbCom = new DbFeedback(_dbConnection)
        {
            InjectedSql = string.Empty,
            DbTransaction = null
        };

        itemManager.InsertByInstance(testEntry, ref dbCom);

        // Delete data by primary key
        itemManager.DeleteByPrimaryKey(testEntry, ref dbCom);

        // Verify deletion
        TestEntry deletedEntry = null;
        _dbConnection.Open();
        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM TestEntry WHERE GuidValue = @GuidValue";
            command.Parameters.Add(new MySqlParameter("@GuidValue", testEntry.GuidValue));
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    deletedEntry = new TestEntry
                    {
                        GuidValue = (Guid)reader["GuidValue"],
                        // Other fields...
                    };
                }
            }
        }

        Assert.That(deletedEntry, Is.Null);
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