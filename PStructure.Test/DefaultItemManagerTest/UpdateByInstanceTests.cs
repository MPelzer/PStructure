using System;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.TableLocation;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest;

[TestFixture]
public class UpdateByInstanceTests : BasicTest
{
    [SetUp]
    public void SetUp()
    {
        SetUpDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        TearDownDatabase();
    }

    [Test]
    public void UpdateByInstance_Should_UpdateDataCorrectly()
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
        var itemManager = new ItemManager<TestEntry>(tableLocation, logger);
        var dbCom = new DbFeedback(_dbConnection)
        {
            InjectedSql = string.Empty,
            DbTransaction = null
        };

        itemManager.CreateByInstances(testEntry, ref dbCom);

        // Update data
        testEntry.IntegerValue = 456;
        testEntry.StringValue = "Updated String";

        itemManager.UpdateByInstance(testEntry, ref dbCom);

        // Verify update
        TestEntry updatedEntry = null;
        _dbConnection.Open();
        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM TestEntry WHERE GuidValue = @GuidValue";
            command.Parameters.Add(new MySqlParameter("@GuidValue", testEntry.GuidValue));

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    updatedEntry = new TestEntry
                    {
                        GuidValue = (Guid)reader["GuidValue"],
                        IntegerValue = Convert.ToInt32(reader["IntegerValue"]),
                        StringValue = reader["StringValue"].ToString(),
                        // Other fields...
                    };
                }
            }
        }

        Assert.That(updatedEntry, Is.Not.Null);
        Assert.That(updatedEntry.IntegerValue, Is.EqualTo(456));
        Assert.That(updatedEntry.StringValue, Is.EqualTo("Updated String"));
    }

    
}