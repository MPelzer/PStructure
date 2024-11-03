using NUnit.Framework;
using System;
using System.Linq;
using PStructure.Models;
using PStructure.SqlGenerator;
using PStructure.root;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetDeleteSqlByPrimaryKeyTests : BasicTest
    {
        private SimpleSqlGenerator<TestEntity> _generator;

        [SetUp]
        public void SetUp()
        {
            SetUpDatabase();
            _generator = new SimpleSqlGenerator<TestEntity>(null);
        }

        [TearDown]
        public void TearDown()
        {
            TearDownDatabase();
        }
        
        [Test]
        public void GetDeleteSqlByPrimaryKey_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetDeleteSqlByPrimaryKey(null, tableName);

            var expectedSql = "DELETE FROM TestTable WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetDeleteSqlByPrimaryKey_MultiplePrimaryKeys_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetDeleteSqlByPrimaryKey(null, tableName);

            var expectedSql = "DELETE FROM TestTable WHERE PrimaryKey1 = @PrimaryKey1 AND PrimaryKey2 = @PrimaryKey2";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetDeleteSqlByPrimaryKey_ColumnNamesWithSpecialCharacters_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetDeleteSqlByPrimaryKey(null, tableName);

            var expectedSql = "DELETE FROM TestTable WHERE [Special Key] = @Special_Key";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetDeleteSqlByPrimaryKey_EmptyTableName_ShouldReturnCorrectSql()
        {
            var tableName = string.Empty;
            var sql = _generator.GetDeleteSqlByPrimaryKey(null, tableName);

            var expectedSql = "DELETE FROM  WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetDeleteSqlByPrimaryKey_NoPrimaryKey_ShouldThrowException()
        {
            var typeWithNoPrimaryKey = typeof(EntityWithNoPrimaryKey);
            var tableName = "TestTable";

            Assert.Throws<InvalidOperationException>(() => _generator.GetDeleteSqlByPrimaryKey(null, tableName));
        }

        [Test]
        public void Caching_DeleteSql_ShouldUseCachedValue()
        {
            var tableName = "TestTable";
            var sql1 = _generator.GetDeleteSqlByPrimaryKey(null, tableName);
            var sql2 = _generator.GetDeleteSqlByPrimaryKey(null, tableName);

            Assert.That(NormalizeSql(sql1), Is.EqualTo(NormalizeSql(sql2)), "The cached SQL should be used.");
        }

        private string NormalizeSql(string sql)
        {
            return string.Join(" ", sql.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        // Test classes for different scenarios
        public class TestEntity
        {
            [PrimaryKey]
            [Column("PrimaryKey")]
            public int Id { get; set; }
        }

        public class EntityWithNoPrimaryKey
        {
            [Column("Column")]
            public string Value { get; set; }
        }
    }
}
