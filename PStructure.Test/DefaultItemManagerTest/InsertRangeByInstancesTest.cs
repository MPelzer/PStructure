using System;
using System.Collections.Generic;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.TableLocation;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest
{
    [TestFixture]
    public class InsertRangeTests
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
        public void InsertRangeByInstances_Should_InsertDataCorrectly()
        {
            // Create a list of entries to insert
            var testEntries = new List<TestEntry>
            {
                new TestEntry
                {
                    GuidValue = Guid.NewGuid(),
                    IntegerValue = 101,
                    LongValue = 1010101010L,
                    ShortValue = 10101,
                    ByteValue = 255,
                    FloatValue = 101.01f,
                    DoubleValue = 10101.1010,
                    DecimalValue = 10101.01m,
                    BooleanValue = true,
                    CharValue = 'A',
                    StringValue = "Test String 1",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 1, 2, 3 }
                },
                new TestEntry
                {
                    GuidValue = Guid.NewGuid(),
                    IntegerValue = 202,
                    LongValue = 2020202020L,
                    ShortValue = 20202,
                    ByteValue = 200,
                    FloatValue = 202.02f,
                    DoubleValue = 20202.2020,
                    DecimalValue = 20202.02m,
                    BooleanValue = false,
                    CharValue = 'B',
                    StringValue = "Test String 2",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 4, 5, 6 }
                }
            };

            // Set up the item manager
            var logger = _testEntryFactory.GetTestLogger();
            var tableLocation = new BaseTableLocation("", "TestEntry");
            var itemManager = new DefaultItemManager<TestEntry>(tableLocation, logger);
            
            var dbCom = new DbCom
            {
                requestAnswer = false,
                _dbConnection = _dbConnection,
                _transaction = null,
                injectedSql = " ",
                requestException = null
            };

            // Insert the range of test entries
            itemManager.InsertRangeByInstances(testEntries, ref dbCom);

            // Validate the inserted entries
            var insertedEntries = new List<TestEntry>();
            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM TestEntry";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        insertedEntries.Add(new TestEntry
                        {
                            GuidValue = (Guid)reader["GuidValue"],
                            IntegerValue = Convert.ToInt32(reader["IntegerValue"]),
                            LongValue = Convert.ToInt64(reader["LongValue"]),
                            ShortValue = Convert.ToInt16(reader["ShortValue"]),
                            ByteValue = Convert.ToByte(reader["ByteValue"]),
                            FloatValue = Convert.ToSingle(reader["FloatValue"]),
                            DoubleValue = Convert.ToDouble(reader["DoubleValue"]),
                            DecimalValue = Convert.ToDecimal(reader["DecimalValue"]),
                            BooleanValue = Convert.ToBoolean(reader["BooleanValue"]),
                            CharValue = Convert.ToChar(reader["CharValue"]),
                            StringValue = reader["StringValue"].ToString(),
                            DateTimeValue = (DateTime)reader["DateTimeValue"],
                            ByteArrayValue = (byte[])reader["ByteArrayValue"]
                        });
                    }
                }
            }

            // Assert that both entries were inserted
            Assert.That(insertedEntries.Count, Is.EqualTo(2));

            // Validate each inserted entry
            foreach (var insertedEntry in insertedEntries)
            {
                var originalEntry = testEntries.Find(e => e.GuidValue == insertedEntry.GuidValue);

                Assert.That(originalEntry, Is.Not.Null);
                Assert.That(insertedEntry.IntegerValue, Is.EqualTo(originalEntry.IntegerValue));
                Assert.That(insertedEntry.LongValue, Is.EqualTo(originalEntry.LongValue));
                Assert.That(insertedEntry.ShortValue, Is.EqualTo(originalEntry.ShortValue));
                Assert.That(insertedEntry.ByteValue, Is.EqualTo(originalEntry.ByteValue));
                Assert.That(insertedEntry.FloatValue, Is.EqualTo(originalEntry.FloatValue));
                Assert.That(insertedEntry.DoubleValue, Is.EqualTo(originalEntry.DoubleValue));
                Assert.That(insertedEntry.DecimalValue, Is.EqualTo(originalEntry.DecimalValue));
                Assert.That(insertedEntry.BooleanValue, Is.EqualTo(originalEntry.BooleanValue));
                Assert.That(insertedEntry.CharValue, Is.EqualTo(originalEntry.CharValue));
                Assert.That(insertedEntry.StringValue, Is.EqualTo(originalEntry.StringValue));
                Assert.That(insertedEntry.DateTimeValue, Is.EqualTo(originalEntry.DateTimeValue).Within(TimeSpan.FromSeconds(1)));
                Assert.That(insertedEntry.ByteArrayValue, Is.EqualTo(originalEntry.ByteArrayValue));
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _testEntryFactory.TearDown(_dbConnection);
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
