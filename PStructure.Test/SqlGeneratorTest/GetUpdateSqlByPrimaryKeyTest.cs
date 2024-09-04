using System;
using NUnit.Framework;
using PStructure.Models;
using PStructure.SqlGenerator;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetUpdateSqlByPrimaryKeyTests
    {
        private BaseSqlGenerator<TestEntity> _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new BaseSqlGenerator<TestEntity>();
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetUpdateSqlByPrimaryKey(typeof(TestEntity), tableName);

            var expectedSql = "UPDATE TestTable SET Column = @Column WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_NoPrimaryKey_ShouldThrowException()
        {
            var typeWithoutPrimaryKey = typeof(EntityWithoutPrimaryKey);
            var tableName = "TestTable";

            Assert.That(() => _generator.GetUpdateSqlByPrimaryKey(typeWithoutPrimaryKey, tableName),
                        Throws.InvalidOperationException);
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_MultiplePrimaryKeys_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetUpdateSqlByPrimaryKey(typeof(MultiplePrimaryKeyEntity), tableName);

            var expectedSql = "UPDATE TestTable SET Column = @Column WHERE PrimaryKey1 = @PrimaryKey1 AND PrimaryKey2 = @PrimaryKey2";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_ColumnNamesWithSpecialCharacters_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetUpdateSqlByPrimaryKey(typeof(SpecialCharColumnEntity), tableName);

            var expectedSql = "UPDATE TestTable SET [Special Column] = @Special_Column WHERE [Special Key] = @Special_Key";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_EmptyTableName_ShouldReturnCorrectSql()
        {
            var tableName = string.Empty;
            var sql = _generator.GetUpdateSqlByPrimaryKey(typeof(TestEntity), tableName);

            var expectedSql = "UPDATE  SET Column = @Column WHERE PrimaryKey = @PrimaryKey";

            Assert.That(NormalizeSql(sql), Is.EqualTo(NormalizeSql(expectedSql)));
        }

        [Test]
        public void GetUpdateSqlByPrimaryKey_NoColumns_ShouldThrowException()
        {
            var typeWithNoColumns = typeof(EntityWithNoColumns);
            var tableName = "TestTable";

            Assert.That(() => _generator.GetUpdateSqlByPrimaryKey(typeWithNoColumns, tableName),
                        Throws.InvalidOperationException);
        }

        [Test]
        public void Caching_UpdateSql_ShouldUseCachedValue()
        {
            var tableName = "TestTable";
            var sql1 = _generator.GetUpdateSqlByPrimaryKey(typeof(TestEntity), tableName);
            var sql2 = _generator.GetUpdateSqlByPrimaryKey(typeof(TestEntity), tableName);

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
