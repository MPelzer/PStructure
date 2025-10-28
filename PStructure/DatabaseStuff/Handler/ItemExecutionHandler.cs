using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.DatabaseStuff.Handler;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    /// <summary>
    /// Generic Dapper handler for item-based operations.
    /// Supports:
    /// - Execute / Query (sync & async)
    /// - Multi-Mapping (2 or 3 entities)
    /// - Parallel batch execution (sync & async)
    /// </summary>
    public class ItemExecutionHandler<T> : IExecutionHandler<ItemRequestContext<T>, T>
    {
        private IExecutionHandler<ItemRequestContext<T>, T> _executionHandlerImplementation;
        private const int MaxErrors = 3;

        #region Validation

        private static void ValidateContext(ExecutionContext<ItemRequestContext<T>> context, string[] errors, ref int errorCount)
        {
            if (context.DbContext?.DbConnection == null)
                errors[errorCount++] = "DbConnection darf nicht null sein";
            if (context.DbContext?.DbTransaction == null)
                errors[errorCount++] = "DbTransaction darf nicht null sein";
            if (context.RequestContext?.StatementBuilder == null)
                errors[errorCount++] = "StatementBuilder darf nicht null sein";
            if (context.RequestContext?.Items == null)
                errors[errorCount++] = "Items darf nicht null sein";
        }

        private static void ThrowIfErrors(string[] errors, int errorCount)
        {
            if (errorCount > 0)
                throw new AggregateException(errors.Take(errorCount).Select(e => new NullReferenceException(e)));
        }

        #endregion

        #region 🧱 Base Execute / Query

        public int Execute(ExecutionContext<ItemRequestContext<T>> context)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction!;
            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);

            return conn.Execute(sql, parameters, tran);
        }

        public IEnumerable<T> Query(ExecutionContext<ItemRequestContext<T>> context)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction!;
            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);

            return conn.Query<T>(sql, parameters, tran);
        }

        #endregion

        #region 🧩 Multi-Mapping

        public IEnumerable<TReturn> QueryMulti<T1, T2, TReturn>(
            ExecutionContext<ItemRequestContext<T>> context,
            Func<T1, T2, TReturn> map,
            string splitOn = "Id")
        {
            ValidateAndThrow(context);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            return context.DbContext!.DbConnection!.Query(sql, map, parameters, context.DbContext!.DbTransaction!, splitOn: splitOn);
        }

        public IEnumerable<TReturn> QueryMulti<T1, T2, T3, TReturn>(
            ExecutionContext<ItemRequestContext<T>> context,
            Func<T1, T2, T3, TReturn> map,
            string splitOn = "Id")
        {
            ValidateAndThrow(context);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            return context.DbContext!.DbConnection!.Query(sql, map, parameters, context.DbContext!.DbTransaction!, splitOn: splitOn);
        }

        private void ValidateAndThrow(ExecutionContext<ItemRequestContext<T>> context)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);
        }

        #endregion

        #region ⚙️ Async

        public async Task<int> ExecuteAsync(ExecutionContext<ItemRequestContext<T>> context, CancellationToken cancellationToken = default)
        {
            ValidateAndThrow(context);

            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction!;
            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);

            return await conn.ExecuteAsync(sql, parameters, tran);
        }

        public async Task<IEnumerable<T>> QueryAsync(ExecutionContext<ItemRequestContext<T>> context, CancellationToken cancellationToken = default)
        {
            ValidateAndThrow(context);

            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction!;
            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);

            return await conn.QueryAsync<T>(sql, parameters, tran);
        }

        #endregion

        #region ⚡ Parallel Execution

        private static IEnumerable<IEnumerable<T>> ChunkItems(IEnumerable<T> items, int chunkCount)
        {
            var list = items.ToList();
            var size = (int)Math.Ceiling(list.Count / (double)chunkCount);
            for (var i = 0; i < list.Count; i += size)
                yield return list.Skip(i).Take(size);
        }

        public async Task<int> ExecuteParallelAsync(ExecutionContext<ItemRequestContext<T>> context, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            ValidateAndThrow(context);

            var items = context.RequestContext!.Items!.ToList();
            var parallelism = Math.Max(1, context.RequestContext.NumberOfParallelExecutions);
            var chunks = ChunkItems(items, parallelism).ToList();

            logger?.LogInformation("Executing {Count} parallel item batches.", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var (sql, parameters) = context.RequestContext!.StatementBuilder!(chunk);
                return await context.DbContext!.DbConnection!.ExecuteAsync(sql, parameters, context.DbContext!.DbTransaction);
            });

            var results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        public async Task<IEnumerable<T>> QueryParallelAsync(ExecutionContext<ItemRequestContext<T>> context, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            ValidateAndThrow(context);

            var items = context.RequestContext!.Items!.ToList();
            var parallelism = Math.Max(1, context.RequestContext.NumberOfParallelExecutions);
            var chunks = ChunkItems(items, parallelism).ToList();

            logger?.LogInformation("Querying {Count} parallel item batches.", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var (sql, parameters) = context.RequestContext!.StatementBuilder!(chunk);
                return await context.DbContext!.DbConnection!.QueryAsync<T>(sql, parameters, context.DbContext!.DbTransaction);
            });

            await Task.WhenAll(tasks);
            return tasks.SelectMany(t => t.Result).ToList();
        }

        #endregion

        #region ⚡ Parallel Multi-Mapping

        public async Task<IEnumerable<TReturn>> QueryMultiParallelAsync<T1, T2, TReturn>(
            ExecutionContext<ItemRequestContext<T>> context,
            Func<T1, T2, TReturn> map,
            string splitOn = "Id",
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            ValidateAndThrow(context);

            var items = context.RequestContext!.Items!.ToList();
            var parallelism = Math.Max(1, context.RequestContext.NumberOfParallelExecutions);
            var chunks = ChunkItems(items, parallelism).ToList();

            logger?.LogInformation("Executing {Count} parallel multi-mapping batches (T1,T2).", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var (sql, parameters) = context.RequestContext!.StatementBuilder!(chunk);
                return await context.DbContext!.DbConnection!.QueryAsync(sql, map, parameters, context.DbContext!.DbTransaction, splitOn: splitOn);
            });

            await Task.WhenAll(tasks);
            return tasks.SelectMany(t => t.Result).ToList();
        }

        #endregion
    }
}
