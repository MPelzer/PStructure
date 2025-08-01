
using System;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.Pdo;
using PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud;
using PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest;

[TestFixture]
public class DeleteTests : BasicTest
{
    private TestEntryFactory _testEntryFactory;
    [SetUp]
    public void SetUp()
    {
        _testEntryFactory = new TestEntryFactory();
        SetUpDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        TearDownDatabase();
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
        var tableLocation = new TableLocation.TableLocation("", "TestEntry");
        
        
        var simpleCrud = new SimpleCrud<TestEntry>()
        
        var itemManager = new SimplePdoManager<TestEntry>(tableLocation, logger);
        var dbCom = new DbContext(_dbConnection)
        {
            InjectedSql = string.Empty,
            DbTransaction = null
        };

        itemManager.CreateByInstances(testEntry, ref dbCom);

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
}