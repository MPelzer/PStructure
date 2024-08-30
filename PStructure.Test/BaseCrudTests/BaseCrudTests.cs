using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using PStructure.CRUDs;
using PStructure.Interfaces;
using PStructure.Models;
using PStructure.SqlGenerator;
using NUnit.Framework;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.TableLocation;

namespace PStructure.Test.BaseCrudTests
{
    [TestFixture]
    public class BaseCrudTests
    {
        private Mock<ISqlGenerator> _mockSqlGenerator;
        private Mock<IMapperPdoQuery<TestEntity>> _mockMapperPdoQuery;
        private Mock<ITableLocation> _mockTableLocation;
        private Mock<ILogger<BaseCrud<TestEntity>>> _mockLogger;
        private Mock<IDbConnection> _mockDbConnection;
        private Mock<IDbTransaction> _mockDbTransaction;
        private DbCom _dbCom;
        private BaseCrud<TestEntity> _baseCrud;

        [SetUp]
        public void SetUp()
        {
            _mockSqlGenerator = new Mock<ISqlGenerator>();
            _mockMapperPdoQuery = new Mock<IMapperPdoQuery<TestEntity>>();
            _mockTableLocation = new Mock<ITableLocation>();
            _mockLogger = new Mock<ILogger<BaseCrud<TestEntity>>>();
            _mockDbConnection = new Mock<IDbConnection>();
            _mockDbTransaction = new Mock<IDbTransaction>();

            _dbCom = new DbCom
            {
                _dbConnection = _mockDbConnection.Object,
                _transaction = _mockDbTransaction.Object
            };

            _baseCrud = new BaseCrud<TestEntity>(
                _mockSqlGenerator.Object,
                _mockMapperPdoQuery.Object,
                _mockTableLocation.Object,
                _mockLogger.Object);
        }

        [Test]
        public void InsertByInstance_ShouldExecuteSqlCommand()
        {
            var item = new TestEntity();
            var sql = "INSERT INTO TestTable (Columns) VALUES (@Value)";
            _mockSqlGenerator.Setup(s => s.GetInsertSql(typeof(TestEntity), It.IsAny<string>())).Returns(sql);

            _baseCrud.InsertByInstance(item, ref _dbCom);

            _mockDbConnection.Verify(conn => conn.Execute(sql, It.IsAny<DynamicParameters>(), _dbCom._transaction), Times.Once);
        }

        [Test]
        public void ReadByPrimaryKey_ShouldExecuteSqlCommand()
        {
            var item = new TestEntity();
            var sql = "SELECT * FROM TestTable WHERE PrimaryKey = @PrimaryKey";
            _mockSqlGenerator.Setup(s => s.GetReadSqlByPrimaryKey(typeof(TestEntity), It.IsAny<string>())).Returns(sql);
            _mockDbConnection.Setup(conn => conn.ExecuteReader(sql, It.IsAny<DynamicParameters>(), _dbCom._transaction))
                             .Returns(Mock.Of<IDataReader>());

            var result = _baseCrud.ReadByPrimaryKey(item, ref _dbCom);

            _mockDbConnection.Verify(conn => conn.ExecuteReader(sql, It.IsAny<DynamicParameters>(), _dbCom._transaction), Times.Once);
        }

        [Test]
        public void UpdateByInstance_ShouldExecuteSqlCommand()
        {
            var item = new TestEntity();
            var sql = "UPDATE TestTable SET Column = @Value WHERE PrimaryKey = @PrimaryKey";
            _mockSqlGenerator.Setup(s => s.GetUpdateSqlByPrimaryKey(typeof(TestEntity), It.IsAny<string>())).Returns(sql);

            _baseCrud.UpdateByInstance(item, ref _dbCom);

            _mockDbConnection.Verify(conn => conn.Execute(sql, It.IsAny<DynamicParameters>(), _dbCom._transaction), Times.Once);
        }

        [Test]
        public void DeleteByPrimaryKey_ShouldExecuteSqlCommand()
        {
            var item = new TestEntity();
            var sql = "DELETE FROM TestTable WHERE PrimaryKey = @PrimaryKey";
            _mockSqlGenerator.Setup(s => s.GetDeleteSqlByPrimaryKey(typeof(TestEntity), It.IsAny<string>())).Returns(sql);

            _baseCrud.DeleteByPrimaryKey(item, ref _dbCom);

            _mockDbConnection.Verify(conn => conn.Execute(sql, It.IsAny<DynamicParameters>(), _dbCom._transaction), Times.Once);
        }

        [Test]
        public void ReadAll_ShouldExecuteSqlCommand()
        {
            var sql = "SELECT * FROM TestTable";
            _mockSqlGenerator.Setup(s => s.GetSelectAll(It.IsAny<string>())).Returns(sql);
            _mockDbConnection.Setup(conn => conn.Query<TestEntity>(sql, It.IsAny<object>(), _dbCom._transaction))
                             .Returns(new List<TestEntity>());

            var result = _baseCrud.ReadAll(ref _dbCom);

            _mockDbConnection.Verify(conn => conn.Query<TestEntity>(sql, It.IsAny<object>(), _dbCom._transaction), Times.Once);
        }
    }

    public class TestEntity
    {
        [PrimaryKey]
        [Column("PrimaryKey")]
        public int Id { get; set; }
        
        [Column("Column")]
        public string Value { get; set; }
    }
}
