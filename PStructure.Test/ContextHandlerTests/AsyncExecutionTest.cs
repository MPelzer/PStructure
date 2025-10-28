using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.Tests.Integration.AsyncExecution
{
    [TestFixture]
    public class AsyncExecutionTests
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
        public async Task ExecuteAsync_ShouldInsertOrder()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (10, 1, 'ORD-010')"
                }
            };

            var affected = await _handler.ExecuteAsync(ctx);
            Assert.AreEqual(1, affected);
        }

        [Test]
        public void ExecuteAsync_ShouldCancelOperation()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO Orders.OrderHeader(OrderID, CustomerID, OrderNumber) VALUES (11, 1, 'ORD-011')"
                }
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await _handler.ExecuteAsync(ctx, cts.Token));
        }
    }
}
