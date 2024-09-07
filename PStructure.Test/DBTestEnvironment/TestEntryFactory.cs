using System;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PStructure.Test.DBTestEnvironment;

namespace PStructure.Test.Models
{
    public class TestEntryFactory
    {
        TestEntry GetNewTestEntry()
        {
            var testEntry = new TestEntry()
            {
                IntegerValue = 0,
                LongValue = 0L,
                ShortValue = 0,
                ByteValue = 0,
                FloatValue = 0.0f,
                DoubleValue = 0.0,
                DecimalValue = 0.0m,
                BooleanValue = false,
                CharValue = '\0',
                StringValue = string.Empty,
                DateTimeValue = DateTime.Now,
                GuidValue = Guid.NewGuid(),
                ByteArrayValue = Array.Empty<byte>(),
            };
            return testEntry;
        }

        public void intitalizeDatabaseTable(DbConnection dbConnection)
        {
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS TestEntry (
                    GuidValue CHAR(36) PRIMARY KEY, 
                    IntegerValue INT,
                    LongValue BIGINT,
                    ShortValue SMALLINT,
                    ByteValue TINYINT UNSIGNED, -- Use TINYINT UNSIGNED to allow values 0-255
                    FloatValue FLOAT,
                    DoubleValue DOUBLE,
                    DecimalValue DECIMAL(18,2),
                    BooleanValue BOOLEAN,
                    CharValue CHAR(1),
                    StringValue TEXT,
                    DateTimeValue DATETIME,
                    ByteArrayValue BLOB
                )";
                command.ExecuteNonQuery();
            }
        }

        public void TearDown(DbConnection dbConnection)
        {
            try
            {
                
                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE IF EXISTS TestEntry";
                    command.ExecuteNonQuery();
                }
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to drop table: {ex.Message}");
            }
        }

        public ILogger<TestEntry> GetTestLogger()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug);
            });

            return loggerFactory.CreateLogger<TestEntry>();
        }
    }
}