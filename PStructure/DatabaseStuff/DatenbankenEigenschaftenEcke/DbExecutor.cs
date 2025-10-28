using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    public static class DbExecutor
    {
        // --------------------------------------------
        // 🧱 Common Helpers
        // --------------------------------------------

        private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> items, int chunkCount)
        {
            var list = items.ToList();
            var size = (int)Math.Ceiling(list.Count / (double)chunkCount);
            for (var i = 0; i < list.Count; i += size)
                yield return list.Skip(i).Take(size);
        }

        // --------------------------------------------
        // 🔹 Parallel Multi-Mapping (Sync)
        // --------------------------------------------

        public static IEnumerable<TReturn> QueryMultiParallel<TRequest, TItem, T1, T2, TReturn>(
            ExecutionContext<TRequest> context,
            Func<ExecutionContext<TRequest>, IEnumerable<TItem>, Func<T1, T2, TReturn>, string, IEnumerable<TReturn>> queryMultiFunc,
            Func<T1, T2, TReturn> map,
            IEnumerable<TItem> items,
            int degreeOfParallelism = 4,
            string splitOn = "Id",
            ILogger? logger = null)
            where TRequest : RequestContext
        {
            var results = new List<TReturn>();
            var chunks = Chunk(items, degreeOfParallelism).ToList();

            logger?.LogInformation("Starting {Count} parallel multi-mapping batches", chunks.Count);

            Parallel.ForEach(chunks, chunk =>
            {
                var localContext = context;
                var chunkResults = queryMultiFunc(localContext, chunk, map, splitOn);
                lock (results)
                {
                    results.AddRange(chunkResults);
                }
            });

            return results;
        }

        // --------------------------------------------
        // 🔹 Parallel Multi-Mapping (Async)
        // --------------------------------------------

        public static async Task<IEnumerable<TReturn>> QueryMultiParallelAsync<TRequest, TItem, T1, T2, TReturn>(
            ExecutionContext<TRequest> context,
            Func<ExecutionContext<TRequest>, IEnumerable<TItem>, Func<T1, T2, TReturn>, string, CancellationToken, Task<IEnumerable<TReturn>>> queryMultiAsyncFunc,
            Func<T1, T2, TReturn> map,
            IEnumerable<TItem> items,
            int degreeOfParallelism = 4,
            string splitOn = "Id",
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
            where TRequest : RequestContext
        {
            var results = new List<TReturn>();
            var chunks = Chunk(items, degreeOfParallelism).ToList();

            logger?.LogInformation("Starting {Count} async parallel multi-mapping batches", chunks.Count);

            var tasks = chunks.Select(async chunk =>
            {
                var localContext = context;
                var chunkResult = await queryMultiAsyncFunc(localContext, chunk, map, splitOn, cancellationToken);
                lock (results)
                {
                    results.AddRange(chunkResult);
                }
            });

            await Task.WhenAll(tasks);
            return results;
        }
    }
}
