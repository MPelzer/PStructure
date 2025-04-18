﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.Pdo;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;
using PStructure.Test.DBTestEnvironment;
using PStructure.Test.Models;

namespace PStructure.Test.DefaultItemManagerTest
{
    [TestFixture]
    public class ReadByPrimaryKeyTests : BasicTest
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
        public void ReadByPrimaryKey_Should_ReturnCorrectData()
        {
            // Step 1: Insert an entry
            var insertedEntry = new TestEntry
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
                StringValue = "Test String",
                DateTimeValue = DateTime.Now,
                ByteArrayValue = new byte[] { 10, 20, 30 }
            };

            var logger = _testEntryFactory.GetTestLogger();
            var tableLocation = new TableLocation.TableLocation("", "TestEntry");
            var itemManager = new SimplePdoManager<TestEntry>(tableLocation, logger);

            var dbCom = new DbFeedback(_dbConnection)
            {
                InjectedSql = string.Empty,
                DbTransaction = null
            };

            // Insert the entry into the database
            itemManager.CreateByInstances(insertedEntry, ref dbCom);

            // Step 2: Use ReadByPrimaryKey to retrieve the entry
            var retrievedEntry = itemManager.ReadByPrimaryKey(insertedEntry, ref dbCom);

            // Step 3: Assert that the retrieved entry matches the inserted one
            Assert.That(retrievedEntry.GuidValue, Is.EqualTo(insertedEntry.GuidValue));
            Assert.That(retrievedEntry.IntegerValue, Is.EqualTo(insertedEntry.IntegerValue));
            Assert.That(retrievedEntry.LongValue, Is.EqualTo(insertedEntry.LongValue));
            Assert.That(retrievedEntry.ShortValue, Is.EqualTo(insertedEntry.ShortValue));
            Assert.That(retrievedEntry.ByteValue, Is.EqualTo(insertedEntry.ByteValue));
            Assert.That(retrievedEntry.FloatValue, Is.EqualTo(insertedEntry.FloatValue));
            Assert.That(retrievedEntry.DoubleValue, Is.EqualTo(insertedEntry.DoubleValue));
            Assert.That(retrievedEntry.DecimalValue, Is.EqualTo(insertedEntry.DecimalValue));
            Assert.That(retrievedEntry.BooleanValue, Is.EqualTo(insertedEntry.BooleanValue));
            Assert.That(retrievedEntry.CharValue, Is.EqualTo(insertedEntry.CharValue));
            Assert.That(retrievedEntry.StringValue, Is.EqualTo(insertedEntry.StringValue));
            Assert.That(retrievedEntry.DateTimeValue, Is.EqualTo(insertedEntry.DateTimeValue).Within(TimeSpan.FromSeconds(1)));
            Assert.That(retrievedEntry.ByteArrayValue, Is.EqualTo(insertedEntry.ByteArrayValue));
        }

        
    }
}
