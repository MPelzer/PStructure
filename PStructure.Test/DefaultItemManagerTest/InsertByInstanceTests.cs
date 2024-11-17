using System;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.Pdo;
using PStructure.TableLocation;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest
{
    [TestFixture]
    public class InsertByInstanceTests : BasicTest
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
        public void InsertByInstance_Should_InsertDataCorrectly()
        {
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
            var itemManager = new SimpleItemManager<TestEntry>(tableLocation, logger);
            
            var dbCom = new DbFeedback(_dbConnection)
            {
                InjectedSql = string.Empty,
                DbTransaction = null
            };

            itemManager.CreateByInstances(testEntry, ref dbCom);

            TestEntry tableValue = null;
            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM TestEntry WHERE GuidValue = @GuidValue";
                command.Parameters.Add(new MySqlParameter("@GuidValue", testEntry.GuidValue));
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableValue = new TestEntry
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
                        };
                    }
                }
            }
            Assert.That(tableValue, Is.Not.Null);
            Assert.That(tableValue.GuidValue, Is.EqualTo(testEntry.GuidValue));
            Assert.That(tableValue.IntegerValue, Is.EqualTo(testEntry.IntegerValue));
            Assert.That(tableValue.LongValue, Is.EqualTo(testEntry.LongValue));
            Assert.That(tableValue.ShortValue, Is.EqualTo(testEntry.ShortValue));
            Assert.That(tableValue.ByteValue, Is.EqualTo(testEntry.ByteValue));
            Assert.That(tableValue.FloatValue, Is.EqualTo(testEntry.FloatValue));
            Assert.That(tableValue.DoubleValue, Is.EqualTo(testEntry.DoubleValue));
            Assert.That(tableValue.DecimalValue, Is.EqualTo(testEntry.DecimalValue));
            Assert.That(tableValue.BooleanValue, Is.EqualTo(testEntry.BooleanValue));
            Assert.That(tableValue.CharValue, Is.EqualTo(testEntry.CharValue));
            Assert.That(tableValue.StringValue, Is.EqualTo(testEntry.StringValue));
            Assert.That(tableValue.DateTimeValue, Is.EqualTo(testEntry.DateTimeValue).Within(TimeSpan.FromSeconds(1)));
            Assert.That(tableValue.ByteArrayValue, Is.EqualTo(testEntry.ByteArrayValue));
        }
    }
}
