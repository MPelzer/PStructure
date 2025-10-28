using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.Test.ContextHandlerTests
{
    [TestFixture]
    [NonParallelizable] // DB2 doesn’t handle parallel schema ops well
    public class SqlExecutionHandlerDb2Tests
    {
        private iDB2Connection _connection;
        private DbContext _dbContext;
        private SqlExecutionHandler<dynamic> _handler;

        private const string ConnectionString =
            "Server=localhost:50000;Database=testdb;UID=db2inst1;PWD=db2pass;";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Wait for DB2 to start (especially in CI)
            Console.WriteLine("Waiting for Db2 container to initialize...");
            Task.Delay(10000).Wait();
        }

        [SetUp]
        public void Setup()
        {
            _connection = new iDB2Connection(ConnectionString);
            _connection.Open();
            _dbContext = new DbContext(_connection);
            _handler = new SqlExecutionHandler<dynamic>();

            try
            {
                _connection.Execute("DROP TABLE USERS");
            }
            catch
            {
                // ignore if doesn't exist
            }

            _connection.Execute("CREATE TABLE USERS (ID INTEGER GENERATED ALWAYS AS IDENTITY, NAME VARCHAR(100))");
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        #region 🧱 Basic Execution

        [Test]
        public void Execute_ShouldInsertRow_AndReturnAffectedCount()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO USERS (NAME) VALUES ('Alice')",
                    Parameters = null
                }
            };

            var result = _handler.Execute(ctx);
            Assert.AreEqual(1, result);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM USERS");
            NUnit.Framework.Assert.AreEqual(1, count);
        }

        [Test]
        public void Query_ShouldReturnInsertedRows()
        {
            _connection.Execute("INSERT INTO USERS (NAME) VALUES ('Bob')");
            _connection.Execute("INSERT INTO USERS (NAME) VALUES ('Charlie')");

            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "SELECT NAME FROM USERS",
                    Parameters = null
                }
            };

            var result = _handler.Query(ctx).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Bob", (string)result[0].NAME);
        }

        [Test]
        public void Execute_WithParameters_ShouldInsertCorrectly()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO USERS (NAME) VALUES (@Name)",
                    Parameters = new { Name = "Dora" }
                }
            };

            var result = _handler.Execute(ctx);
            Assert.AreEqual(1, result);

            var name = _connection.QuerySingle<string>("SELECT NAME FROM USERS FETCH FIRST 1 ROWS ONLY");
            Assert.AreEqual("Dora", name);
        }

        [Test]
        public void Execute_WithNullSql_ShouldThrow()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = null,
                    Parameters = new { Name = "Fail" }
                }
            };

            Assert.Throws<AggregateException>(() => _handler.Execute(ctx));
        }

        #endregion

        #region ⚙️ Async Execution

        [Test]
        public async Task ExecuteAsync_ShouldWork()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO USERS (NAME) VALUES (@Name)",
                    Parameters = new { Name = "Eve" }
                }
            };

            var result = await _handler.ExecuteAsync(ctx);
            Assert.AreEqual(1, result);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM USERS");
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task QueryAsync_ShouldReturnEnumerable()
        {
            _connection.Execute("INSERT INTO USERS (NAME) VALUES ('Fritz')");
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "SELECT NAME FROM USERS",
                    Parameters = null
                }
            };

            var result = await _handler.QueryAsync(ctx);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void ExecuteAsync_ShouldSupportCancellation()
        {
            var ctx = new ExecutionContext<SqlRequestContext>
            {
                DbContext = _dbContext,
                RequestContext = new SqlRequestContext
                {
                    Sql = "INSERT INTO USERS (NAME) VALUES ('CancelTest')",
                    Parameters = null
                }
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _handler.ExecuteAsync(ctx, cts.Token);
            });
        }

        #endregion

        #region ⚡ Parallel Execution

        [Test]
        public async Task ExecuteParallelAsync_ShouldInsertMultiple()
        {
            var requests = new[]
            {
                new SqlRequestContext { Sql = "INSERT INTO USERS (NAME) VALUES ('A')", Parameters = null },
                new SqlRequestContext { Sql = "INSERT INTO USERS (NAME) VALUES ('B')", Parameters = null },
                new SqlRequestContext { Sql = "INSERT INTO USERS (NAME) VALUES ('C')", Parameters = null }
            };

            var total = await _handler.ExecuteParallelAsync(requests, new ExecutionContext<SqlRequestContext> { DbContext = _dbContext }, 3);
            Assert.AreEqual(3, total);

            var count = _connection.QuerySingle<int>("SELECT COUNT(*) FROM USERS");
            Assert.AreEqual(3, count);
        }

        [Test]
        public async Task QueryParallelAsync_ShouldReturnCombinedResults()
        {
            _connection.Execute("INSERT INTO USERS (NAME) VALUES ('A')");
            _connection.Execute("INSERT INTO USERS (NAME) VALUES ('B')");

            var requests = new[]
            {
                new SqlRequestContext { Sql = "SELECT * FROM USERS WHERE NAME='A'", Parameters = null },
                new SqlRequestContext { Sql = "SELECT * FROM USERS WHERE NAME='B'", Parameters = null }
            };

            var results = await _handler.QueryParallelAsync(requests, new ExecutionContext<SqlRequestContext> { DbContext = _dbContext }, 2);
            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public async Task ExecuteParallelAsync_EmptyRequests_ShouldReturnZero()
        {
            var total = await _handler.ExecuteParallelAsync(Array.Empty<SqlRequestContext>(), new ExecutionContext<SqlRequestContext> { DbContext = _dbContext }, 2);
            Assert.AreEqual(0, total);
        }

        #endregion
    }
}
