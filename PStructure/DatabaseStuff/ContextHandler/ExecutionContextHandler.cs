using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace PStructure.PersistenceLayer.Execution
{
    /// <summary>
    /// Zentraler Orchestrator für Ausführungslogik von Dapper-Operationen:
    /// - Synchron / Asynchron
    /// - Parallele Ausführung
    /// - Multi-Mapping-Unterstützung
    /// - Logging & Fehlerbehandlung
    /// </summary>
    public static class ExecutionContextHandler
    {
        private const int MaxErrors = 4;

        #region Validation

        private static void ValidateSqlContext(ExecutionContext<SqlRequestContext> context, string[] errors, ref int errorCount)
        {
            if (context.DbContext?.DbConnection == null)
                errors[errorCount++] = "DbConnection darf nicht null sein";
            if (context.RequestContext?.Sql == null)
                errors[errorCount++] = "Sql darf nicht null sein";
            if (context.RequestContext?.Parameters == null)
                errors[errorCount++] = "Parameters darf nicht null sein";
        }

        private static void ValidateItemContext<T>(ExecutionContext<ItemRequestContext<T>> context, string[] errors, ref int errorCount)
        {
            if (context.DbContext?.DbConnection == null)
                errors[errorCount++] = "DbConnection darf nicht null sein";
            if (context.RequestContext?.StatementBuilder == null)
                errors[errorCount++] = "StatementBuilder darf nicht null sein";
            if (context.RequestContext?.Items == null)
                errors[errorCount++] = "Items darf nicht null sein";
        }

        #endregion

        #region Parallel Execution Helpers

        private static IEnumerable<IEnumerable<T>> ChunkItems<T>(IEnumerable<T> items, int chunks)
        {
            var list = items.ToList();
            var size = (int)Math.Ceiling(list.Count / (double)chunks);
            for (var i = 0; i < list.Count; i += size)
                yield return list.Skip(i).Take(size);
        }

        private static void ThrowIfErrors(string[] errors, int errorCount)
        {
            if (errorCount > 0)
                throw new AggregateException(errors.Take(errorCount).Select(e => new NullReferenceException(e)));
        }

        #endregion

        #region Standard Query & Execute

        public static int Execute<T>(ExecutionContext<ItemRequestContext<T>> context, ILogger? logger = null)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction;

            logger?.LogInformation("Executing SQL: {Sql}", sql);
            return conn.Execute(sql, parameters, tran);
        }

        public static async Task<int> ExecuteAsync<T>(
            ExecutionContext<ItemRequestContext<T>> context,
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            var conn = context.DbContext!.DbConnection!;
            var tran = context.DbContext!.DbTransaction;

            logger?.LogDebug("Executing async SQL: {Sql}", sql);
            return await conn.ExecuteAsync(sql, parameters, tran);
        }

        public static IEnumerable<T> Query<T>(ExecutionContext<ItemRequestContext<T>> context, ILogger? logger = null)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            logger?.LogInformation("Query SQL: {Sql}", sql);
            return context.DbContext!.DbConnection!.Query<T>(sql, parameters, context.DbContext!.DbTransaction);
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(
            ExecutionContext<ItemRequestContext<T>> context,
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var (sql, parameters) = context.RequestContext!.StatementBuilder!(context.RequestContext.Items);
            logger?.LogInformation("Async Query SQL: {Sql}", sql);
            return await context.DbContext!.DbConnection!.QueryAsync<T>(sql, parameters, context.DbContext!.DbTransaction);
        }

        #endregion

        #region Parallel Processing

        public static int ExecuteParallel<T>(
            ExecutionContext<ItemRequestContext<T>> context,
            ILogger? logger = null)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var items = context.RequestContext!.Items!.ToList();
            var parallelism = Math.Max(1, context.RequestContext.NumberOfParallelExecutions);
            var chunks = ChunkItems(items, parallelism).ToList();

            logger?.LogInformation("Executing {Chunks} parallel batches", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var (sql, parameters) = context.RequestContext!.StatementBuilder!(chunk);
                return await context.DbContext!.DbConnection!.ExecuteAsync(sql, parameters, context.DbContext!.DbTransaction);
            });

            Task.WhenAll(tasks).Wait();
            return tasks.Sum(t => t.Result);
        }

        public static IEnumerable<T> QueryParallel<T>(
            ExecutionContext<ItemRequestContext<T>> context,
            ILogger? logger = null)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateItemContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            var items = context.RequestContext!.Items!.ToList();
            var parallelism = Math.Max(1, context.RequestContext.NumberOfParallelExecutions);
            var chunks = ChunkItems(items, parallelism).ToList();

            logger?.LogInformation("Querying {Chunks} parallel batches", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var (sql, parameters) = context.RequestContext!.StatementBuilder!(chunk);
                return await context.DbContext!.DbConnection!.QueryAsync<T>(sql, parameters, context.DbContext!.DbTransaction);
            });

            Task.WhenAll(tasks).Wait();
            return tasks.SelectMany(t => t.Result).ToList();
        }

        #endregion

        #region Multi-Mapping

        public static IEnumerable<TReturn> QueryMulti<T1, T2, TReturn>(
            ExecutionContext<SqlRequestContext> context,
            Func<T1, T2, TReturn> map,
            string splitOn = "Id",
            ILogger? logger = null)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateSqlContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            logger?.LogDebug("Multi-Mapping Query (T1,T2) - SQL: {Sql}", context.RequestContext!.Sql);
            return context.DbContext!.DbConnection!.Query(
                context.RequestContext!.Sql!,
                map,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction!,
                splitOn: splitOn
            );
        }

        public static async Task<IEnumerable<TReturn>> QueryMultiAsync<T1, T2, T3, TReturn>(
            ExecutionContext<SqlRequestContext> context,
            Func<T1, T2, T3, TReturn> map,
            string splitOn = "Id",
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            var errors = new string[MaxErrors];
            var errorCount = 0;
            ValidateSqlContext(context, errors, ref errorCount);
            ThrowIfErrors(errors, errorCount);

            logger?.LogDebug("Multi-Mapping Async Query (T1,T2,T3) - SQL: {Sql}", context.RequestContext!.Sql);
            return await context.DbContext!.DbConnection!.QueryAsync(
                context.RequestContext!.Sql!,
                map,
                context.RequestContext!.Parameters!,
                context.DbContext!.DbTransaction!,
                splitOn: splitOn
            );
        }

        #endregion
    }
}
