using System.Linq;
using Dapper;
using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.Tests.Integration.MultiMapping
{
    [TestFixture]
    public class MultiMappingTests
    {
        private iDB2Connection _connection;
        private DbContext _dbContext;
        private SqlExecutionHandler<dynamic> _handler;

        [SetUp]
        public void Setup()
        {
            _connection = new iDB2Connection("Server=localhost:3306;Database=testdb;UID=testuser;PWD=testpassword;");
            _connection.Open();
            _dbContext = new DbContext(_connection);
            _handler = new SqlExecutionHandler<dynamic>();
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        [Test]
        public void MultiMapping_ShouldJoinOrderHeaderWithPositions()
        {
            _connection.Execute("INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (300, 1, 'ORD-300')");
            for (int i = 1; i <= 2; i++)
                _connection.Execute($"INSERT INTO Orders.OrderPosition(OrderID, OrderSubNumber, PositionNumber, ProductName, Quantity, UnitPrice) VALUES (300, 'A', {i}, 'Prod{i}', {i}, 10)");

            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = @"SELECT h.OrderID, h.OrderNumber, p.PositionNumber, p.ProductName
                            FROM Orders.OrderHeader h
                            JOIN Orders.OrderPosition p ON h.OrderID = p.OrderID"
                }
            };

            var result = _handler.Query<dynamic>(ctx).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Prod1", (string)result[0].PRODUCTNAME);
        }
    }
}