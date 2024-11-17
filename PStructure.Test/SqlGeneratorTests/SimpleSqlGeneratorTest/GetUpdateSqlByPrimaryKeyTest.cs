using NUnit.Framework;
using System;
using PStructure.Models;
using PStructure.PersistenceLayer.Pdo.PdoToTableMapping.SimpleCrud;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.Test.DBTestEnvironment;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetUpdateSqlByPrimaryKeyTests
    {
        private SimpleSqlGenerator<TestEntry> _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new SimpleSqlGenerator<TestEntry>(null);
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_ShouldReturnCorrectSql()
        {
            // Arrange
            var tableName = "TestTable";

            // Act
            var sql = _generator.GetUpdateSqlByPrimaryKey(null, tableName);
            var expectedSql = "UPDATE TestTable SET IntegerValue = @IntegerValue, LongValue = @LongValue, ShortValue = @ShortValue, ByteValue = @ByteValue, FloatValue = @FloatValue, DoubleValue = @DoubleValue, DecimalValue = @DecimalValue, BooleanValue = @BooleanValue, CharValue = @CharValue, StringValue = @StringValue, DateTimeValue = @DateTimeValue, ByteArrayValue = @ByteArrayValue WHERE GuidValue = @GuidValue";

            // Assert
            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_EmptyTableName_ShouldReturnCorrectSql()
        {
            // Arrange
            var tableName = string.Empty;

            // Act
            var sql = _generator.GetUpdateSqlByPrimaryKey(null, tableName);
            var expectedSql = "UPDATE  SET IntegerValue = @IntegerValue, LongValue = @LongValue, ShortValue = @ShortValue, ByteValue = @ByteValue, FloatValue = @FloatValue, DoubleValue = @DoubleValue, DecimalValue = @DecimalValue, BooleanValue = @BooleanValue, CharValue = @CharValue, StringValue = @StringValue, DateTimeValue = @DateTimeValue, ByteArrayValue = @ByteArrayValue WHERE GuidValue = @GuidValue";

            // Assert
            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_NoPrimaryKey_ShouldThrowException()
        {
            // Arrange
            var generator = new SimpleSqlGenerator<EntityWithoutPrimaryKey>(null);
            var tableName = "TestTable";

            // Act & Assert
            Assert.That(() => generator.GetUpdateSqlByPrimaryKey(null, tableName),
                        Throws.InvalidOperationException.With.Message.Contain("does not have any properties with [PrimaryKey] attribute"));
        }

        [Test]
        public void Caching_UpdateSql_ShouldUseCachedValue()
        {
            // Arrange
            var tableName = "TestTable";

            // Act
            var sql1 = _generator.GetUpdateSqlByPrimaryKey(null, tableName);
            var sql2 = _generator.GetUpdateSqlByPrimaryKey(null, tableName);

            // Assert
            Assert.That(NormalizeSql(sql1), Is.EqualTo(NormalizeSql(sql2)), "The cached SQL should be used.");
        }

        // Utility method to normalize SQL strings for accurate comparisons
        private static string NormalizeSql(string sql)
        {
            return string.Join(" ", sql.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries));
        }

        #region Test Classes

        public class EntityWithoutPrimaryKey
        {
            [Column("Column")]
            public string Value { get; set; }
        }

        #endregion
    }
}
