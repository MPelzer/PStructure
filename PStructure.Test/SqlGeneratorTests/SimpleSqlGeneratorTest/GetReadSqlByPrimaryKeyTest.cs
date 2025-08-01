using NUnit.Framework;
using System;
using PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.Test.DBTestEnvironment;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetReadSqlByPrimaryKeyTests
    {
        private SimpleSqlGenerator<TestEntry> _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new SimpleSqlGenerator<TestEntry>(null);
        }

        [Test]
        public void GetReadSqlByPrimaryKey_ShouldReturnCorrectSql()
        {
            // Arrange
            var tableName = "TestTable";

            // Act
            var sql = _generator.GetReadSqlByPrimaryKey(null);
            var expectedSql = "SELECT * FROM TestTable WHERE GuidValue = @GuidValue";

            // Assert
            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_NoPrimaryKey_ShouldThrowException()
        {
            // Arrange
            var generator = new SimpleSqlGenerator<TestEntryWithoutPrimaryKey>(null);
            var tableName = "TestTable";

            // Act & Assert
            Assert.That(() => generator.GetReadSqlByPrimaryKey(null),
                        Throws.InvalidOperationException.With.Message.Contains("does not have any properties with [PrimaryKey] attribute"));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_Caching_ShouldUseCachedValue()
        {
            // Arrange
            var tableName = "TestTable";

            // Act
            var sql1 = _generator.GetReadSqlByPrimaryKey(null);
            var sql2 = _generator.GetReadSqlByPrimaryKey(null);

            // Assert
            Assert.That(NormalizeSql(sql1), Is.EqualTo(NormalizeSql(sql2)), "The cached SQL should be used.");
        }

        private string NormalizeSql(string sql)
        {
            return string.Join(" ", sql.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        // Supporting class for the no-primary-key scenario
        public class TestEntryWithoutPrimaryKey
        {
            [PdoPropertyAttributes.Column("Column")]
            public string Value { get; set; }
        }
    }
}
