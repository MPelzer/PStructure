using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace PStructure.DatabaseStuff.Handler
{
    /// <summary>
    /// Executes raw SQL commands using Dapper.
    /// Supports:
    /// - Execute / Query (sync & async)
    /// - Parallel batch execution
    /// </summary>
    public class SqlExecutionHandler<T>
    {
        private const int MaxErrors = 4;

        #region Validation

        private static void ValidateContext(ExecutionContext<SqlRequestContext> context, string[] errors, ref int errorCount)
        {
            if (context.DbContext?.DbConnection == null)
                errors[errorCount++] = "DbConnection darf nicht null sein";
            if (string.IsNullOrWhiteSpace(context.RequestContext?.Sql))
                errors[errorCount++] = "SQL darf nicht null oder leer sein";
            if (context.RequestContext?.Parameters == null)
                errors[errorCount++] = "Parameters darf nicht null sein";
        }

        private static void ThrowIfErrors(string[] errors, int errorCount)
        {
            if (errorCount > 0)
                throw new AggregateException(errors.Take(errorCount).Select(e => new NullReferenceException(e)));
        }

        #endregion

        #region 🔧 Base Execute / Query

        public int Execute(ExecutionContext<SqlRequestContext> context)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            return context.DbContext!.DbConnection!.Execute(
                context.RequestContext!.Sql!,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction
            );
        }

        public IEnumerable<T> Query(ExecutionContext<SqlRequestContext> context)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            return context.DbContext!.DbConnection!.Query<T>(
                context.RequestContext!.Sql!,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction
            );
        }

        #endregion

        #region ⚡ Async

        public async Task<int> ExecuteAsync(ExecutionContext<SqlRequestContext> context, CancellationToken cancellationToken = default)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            return await context.DbContext!.DbConnection!.ExecuteAsync(
                context.RequestContext!.Sql!,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction
            );
        }

        public async Task<IEnumerable<T>> QueryAsync(ExecutionContext<SqlRequestContext> context, CancellationToken cancellationToken = default)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            return await context.DbContext!.DbConnection!.QueryAsync<T>(
                context.RequestContext!.Sql!,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction
            );
        }

        #endregion

        #region ⚡ Parallel Execution

        private static IEnumerable<IEnumerable<SqlRequestContext>> ChunkSqlRequests(IEnumerable<SqlRequestContext> requests, int chunks)
        {
            var list = requests.ToList();
            var size = (int)Math.Ceiling(list.Count / (double)chunks);
            for (var i = 0; i < list.Count; i += size)
                yield return list.Skip(i).Take(size);
        }

        /// <summary>
        /// Execute multiple SQL requests in parallel batches.
        /// </summary>
        public async Task<int> ExecuteParallelAsync(IEnumerable<SqlRequestContext> sqlRequests, ExecutionContext<SqlRequestContext> context, int parallelism, ILogger? logger = null)
        {
            if (!sqlRequests.Any()) return 0;
            parallelism = Math.Max(1, parallelism);

            var chunks = ChunkSqlRequests(sqlRequests, parallelism).ToList();
            logger?.LogInformation("Executing {Count} parallel SQL batches.", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var sum = 0;
                foreach (var request in chunk)
                {
                    var batchContext = new ExecutionContext<SqlRequestContext>
                    {
                        DbContext = context.DbContext,
                        Logger = context.Logger,
                        RequestContext = request
                    };
                    sum += await ExecuteAsync(batchContext);
                }
                return sum;
            });

            var results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        /// <summary>
        /// Query multiple SQL requests in parallel batches.
        /// </summary>
        public async Task<IEnumerable<T>> QueryParallelAsync(IEnumerable<SqlRequestContext> sqlRequests, ExecutionContext<SqlRequestContext> context, int parallelism, ILogger? logger = null)
        {
            if (!sqlRequests.Any()) return Enumerable.Empty<T>();
            parallelism = Math.Max(1, parallelism);

            var chunks = ChunkSqlRequests(sqlRequests, parallelism).ToList();
            logger?.LogInformation("Querying {Count} parallel SQL batches.", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var resultList = new List<T>();
                foreach (var request in chunk)
                {
                    var batchContext = new ExecutionContext<SqlRequestContext>
                    {
                        DbContext = context.DbContext,
                        Logger = context.Logger,
                        RequestContext = request
                    };
                    var batchResult = await QueryAsync(batchContext);
                    resultList.AddRange(batchResult);
                }
                return resultList;
            });

            await Task.WhenAll(tasks);
            return tasks.SelectMany(t => t.Result).ToList();
        }

        #endregion
    }
}
