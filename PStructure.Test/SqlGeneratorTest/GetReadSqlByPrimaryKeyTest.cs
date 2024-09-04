using NUnit.Framework;
using System;
using PStructure.SqlGenerator;
using PStructure.Models;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetReadSqlByPrimaryKeyTests
    {
        private BaseSqlGenerator<TestEntity> _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new BaseSqlGenerator<TestEntity>();
        }

        [Test]
        public void GetReadSqlByPrimaryKey_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetReadSqlByPrimaryKey(typeof(TestEntity), tableName);

            var expectedSql = "SELECT * FROM TestTable WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_NoPrimaryKey_ShouldThrowException()
        {
            var typeWithoutPrimaryKey = typeof(EntityWithoutPrimaryKey);
            var tableName = "TestTable";

            Assert.That(() => _generator.GetReadSqlByPrimaryKey(typeWithoutPrimaryKey, tableName),
                        Throws.InvalidOperationException.With.Message.Contains("does not have any properties with [PrimaryKey] attribute"));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_MultiplePrimaryKeys_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetReadSqlByPrimaryKey(typeof(MultiplePrimaryKeyEntity), tableName);

            var expectedSql = "SELECT * FROM TestTable WHERE PrimaryKey1 = @PrimaryKey1 AND PrimaryKey2 = @PrimaryKey2";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_ColumnNamesWithSpecialCharacters_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetReadSqlByPrimaryKey(typeof(SpecialCharColumnEntity), tableName);

            var expectedSql = "SELECT * FROM TestTable WHERE [Special Key] = @Special_Key";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_EmptyTableName_ShouldReturnCorrectSql()
        {
            var tableName = string.Empty;
            var sql = _generator.GetReadSqlByPrimaryKey(typeof(TestEntity), tableName);

            var expectedSql = "SELECT * FROM  WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetReadSqlByPrimaryKey_Caching_ShouldUseCachedValue()
        {
            var tableName = "TestTable";
            var sql1 = _generator.GetReadSqlByPrimaryKey(typeof(TestEntity), tableName);
            var sql2 = _generator.GetReadSqlByPrimaryKey(typeof(TestEntity), tableName);

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
            
            [Column("Column")]
            public string Value { get; set; }
        }

        public class EntityWithoutPrimaryKey
        {
            [Column("Column")]
            public string Value { get; set; }
        }

        public class MultiplePrimaryKeyEntity
        {
            [PrimaryKey]
            [Column("PrimaryKey1")]
            public int Key1 { get; set; }

            [PrimaryKey]
            [Column("PrimaryKey2")]
            public int Key2 { get; set; }

            [Column("Column")]
            public string Value { get; set; }
        }

        public class SpecialCharColumnEntity
        {
            [PrimaryKey]
            [Column("Special Key")]
            public int Key { get; set; }

            [Column("Special Column")]
            public string Value { get; set; }
        }

        public class EntityWithNoColumns
        {
            [PrimaryKey]
            [Column("PrimaryKey")]
            public int Id { get; set; }
        }
    }
}
