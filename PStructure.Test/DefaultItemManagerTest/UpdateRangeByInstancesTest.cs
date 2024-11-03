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
    public class UpdateRangeTests : BasicTest
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
        public void UpdateRangeByInstances_Should_UpdateDataCorrectly()
        {
            // Step 1: Insert initial data
            var initialEntries = new List<TestEntry>
            {
                new TestEntry
                {
                    GuidValue = Guid.NewGuid(),
                    IntegerValue = 100,
                    LongValue = 1000000000L,
                    ShortValue = 1000,
                    ByteValue = 100,
                    FloatValue = 100.1f,
                    DoubleValue = 100.10,
                    DecimalValue = 100.10m,
                    BooleanValue = true,
                    CharValue = 'X',
                    StringValue = "Initial String 1",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 10, 20, 30 }
                },
                new TestEntry
                {
                    GuidValue = Guid.NewGuid(),
                    IntegerValue = 200,
                    LongValue = 2000000000L,
                    ShortValue = 2000,
                    ByteValue = 200,
                    FloatValue = 200.2f,
                    DoubleValue = 200.20,
                    DecimalValue = 200.20m,
                    BooleanValue = false,
                    CharValue = 'Y',
                    StringValue = "Initial String 2",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 40, 50, 60 }
                }
            };

            // Set up the item manager
            var logger = _testEntryFactory.GetTestLogger();
            var tableLocation = new BaseTableLocation("", "TestEntry");
            var itemManager = new ItemManager<TestEntry>(tableLocation, logger);
            
            var dbCom = new DbFeedback(_dbConnection)
            {
                InjectedSql = string.Empty,
                DbTransaction = null
            };

            // Insert the initial entries into the database
            itemManager.InsertRangeByInstances(initialEntries, ref dbCom);

            // Step 2: Modify data and update the entries
            var updatedEntries = new List<TestEntry>
            {
                new TestEntry
                {
                    GuidValue = initialEntries[0].GuidValue,
                    IntegerValue = 300,
                    LongValue = 3000000000L,
                    ShortValue = 3000,
                    ByteValue = 150,
                    FloatValue = 300.3f,
                    DoubleValue = 300.30,
                    DecimalValue = 300.30m,
                    BooleanValue = false,
                    CharValue = 'Z',
                    StringValue = "Updated String 1",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 70, 80, 90 }
                },
                new TestEntry
                {
                    GuidValue = initialEntries[1].GuidValue,
                    IntegerValue = 400,
                    LongValue = 4000000000L,
                    ShortValue = 4000,
                    ByteValue = 250,
                    FloatValue = 400.4f,
                    DoubleValue = 400.40,
                    DecimalValue = 400.40m,
                    BooleanValue = true,
                    CharValue = 'W',
                    StringValue = "Updated String 2",
                    DateTimeValue = DateTime.Now,
                    ByteArrayValue = new byte[] { 100, 110, 120 }
                }
            };

            // Update the entries in the database
            itemManager.UpdateRangeByInstances(updatedEntries, ref dbCom);

            // Step 3: Validate the updated entries
            var updatedDbEntries = new List<TestEntry>();
            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM TestEntry";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        updatedDbEntries.Add(new TestEntry
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

            // Assert that both entries were updated
            Assert.That(updatedDbEntries.Count, Is.EqualTo(2));

            // Validate each updated entry
            foreach (var updatedDbEntry in updatedDbEntries)
            {
                var originalEntry = updatedEntries.Find(e => e.GuidValue == updatedDbEntry.GuidValue);

                Assert.That(originalEntry, Is.Not.Null);
                Assert.That(updatedDbEntry.IntegerValue, Is.EqualTo(originalEntry.IntegerValue));
                Assert.That(updatedDbEntry.LongValue, Is.EqualTo(originalEntry.LongValue));
                Assert.That(updatedDbEntry.ShortValue, Is.EqualTo(originalEntry.ShortValue));
                Assert.That(updatedDbEntry.ByteValue, Is.EqualTo(originalEntry.ByteValue));
                Assert.That(updatedDbEntry.FloatValue, Is.EqualTo(originalEntry.FloatValue));
                Assert.That(updatedDbEntry.DoubleValue, Is.EqualTo(originalEntry.DoubleValue));
                Assert.That(updatedDbEntry.DecimalValue, Is.EqualTo(originalEntry.DecimalValue));
                Assert.That(updatedDbEntry.BooleanValue, Is.EqualTo(originalEntry.BooleanValue));
                Assert.That(updatedDbEntry.CharValue, Is.EqualTo(originalEntry.CharValue));
                Assert.That(updatedDbEntry.StringValue, Is.EqualTo(originalEntry.StringValue));
                Assert.That(updatedDbEntry.DateTimeValue, Is.EqualTo(originalEntry.DateTimeValue).Within(TimeSpan.FromSeconds(1)));
                Assert.That(updatedDbEntry.ByteArrayValue, Is.EqualTo(originalEntry.ByteArrayValue));
            }
        }

       
    }
}
