using System.Linq;
using Dapper;
using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.Tests.Integration.BasicExecution
{
    [TestFixture]
    public class BasicExecutionTests
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

            // Testdaten aufräumen
            _connection.Execute("DELETE FROM Orders.OrderPosition");
            _connection.Execute("DELETE FROM Orders.OrderHeader");
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        [Test]
        public void Execute_ShouldInsertSingleOrder()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (1, 1, 'ORD-001')"
                }
            };

            var affected = _handler.Execute(ctx);
            Assert.AreEqual(1, affected);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders.OrderHeader");
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Execute_ShouldUpdateOrder()
        {
            _connection.Execute("INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (2, 2, 'ORD-002')");

            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "UPDATE Orders.OrderHeader SET OrderNumber='ORD-002-UPDATED' WHERE OrderID=2"
                }
            };

            var affected = _handler.Execute(ctx);
            Assert.AreEqual(1, affected);

            var orderNumber = _connection.QuerySingle<string>("SELECT OrderNumber FROM Orders.OrderHeader WHERE OrderID=2");
            Assert.AreEqual("ORD-002-UPDATED", orderNumber);
        }

        [Test]
        public void Execute_ShouldDeleteOrder()
        {
            _connection.Execute("INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (3, 3, 'ORD-003')");

            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "DELETE FROM Orders.OrderHeader WHERE OrderID=3"
                }
            };

            var affected = _handler.Execute(ctx);
            Assert.AreEqual(1, affected);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders.OrderHeader WHERE OrderID=3");
            Assert.AreEqual(0, count);
        }
    }
}
