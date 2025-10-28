using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.Tests.Integration.ParallelExecution
{
    [TestFixture]
    public class ParallelExecutionTests
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
        public async Task ExecuteParallelAsync_ShouldInsertMultiplePositions()
        {
            var requests = Enumerable.Range(1, 5).Select(i => new SqlRequestContext
            {
                Sql = "INSERT INTO Orders.OrderPosition(OrderID, OrderSubNumber, PositionNumber, ProductName, Quantity, UnitPrice) VALUES (100, 'A', @Pos, 'Prod'+@Pos, 2, 12.5)",
                Parameters = new { Pos = i }
            }).ToArray();

            var total = await _handler.ExecuteParallelAsync(requests, new ExecutionContext<SqlRequestContext> { DbContext = _dbContext }, 3);
            Assert.AreEqual(5, total);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM Orders.OrderPosition WHERE OrderID = 100");
            Assert.AreEqual(5, count);
        }

        [Test]
        public async Task QueryParallelAsync_ShouldReturnAllPositions()
        {
            for (int i = 1; i <= 3; i++)
                _connection.Execute($"INSERT INTO Orders.OrderPosition(OrderID, OrderSubNumber, PositionNumber, ProductName, Quantity, UnitPrice) VALUES (200, 'A', {i}, 'Prod{i}', {i}, 10)");

            var requests = Enumerable.Range(1, 3).Select(i => new SqlRequestContext
            {
                Sql = $"SELECT * FROM Orders.OrderPosition WHERE PositionNumber = {i}"
            }).ToArray();

            var result = await _handler.QueryParallelAsync(requests, new ExecutionContext<SqlRequestContext> { DbContext = _dbContext }, 2);
            Assert.AreEqual(3, result.Count());
        }
    }
}
