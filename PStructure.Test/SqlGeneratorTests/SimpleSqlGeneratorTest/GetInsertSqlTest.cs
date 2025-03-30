using NUnit.Framework;
using System;
using System.Linq;
using PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;

namespace PStructure.Test.SqlGeneratorTest
{
    [TestFixture]
    public class GetInsertSqlTests
    {
        private SimpleSqlGenerator<TestEntity> _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new SimpleSqlGenerator<TestEntity>(null);
        }

        [Test]
        public void GetInsertSql_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetInsertSql(null, tableName);

            var expectedSql = "INSERT INTO TestTable (PrimaryKey, Column) VALUES (@PrimaryKey, @Column)";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetInsertSql_ColumnNamesWithSpecialCharacters_ShouldReturnCorrectSql()
        {
            var tableName = "TestTable";
            var sql = _generator.GetInsertSql(null, tableName);

            var expectedSql = "INSERT INTO TestTable ([Special Key], [Special Column]) VALUES (@Special_Key, @Special_Column)";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void GetInsertSql_EmptyTableName_ShouldReturnCorrectSql()
        {
            var tableName = string.Empty;
            var sql = _generator.GetInsertSql(null, tableName);

            var expectedSql = "INSERT INTO  (PrimaryKey, Column) VALUES (@PrimaryKey, @Column)";

            Assert.That(NormalizeSql(expectedSql), Is.EqualTo(NormalizeSql(sql)));
        }

        [Test]
        public void Caching_InsertSql_ShouldUseCachedValue()
        {
            var tableName = "TestTable";
            var sql1 = _generator.GetInsertSql(null, tableName);
            var sql2 = _generator.GetInsertSql(null, tableName);

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

        public class SpecialCharColumnEntity
        {
            [PrimaryKey]
            [Column("Special Key")]
            public int Key { get; set; }

            [Column("Special Column")]
            public string Value { get; set; }
        }
    }
}
