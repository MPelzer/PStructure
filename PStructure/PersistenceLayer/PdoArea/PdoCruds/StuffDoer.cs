using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds
{
    /// <summary>
    /// Base class providing core execution logic for CRUD operations.
    /// Relies on SqlContextHandler for SQL/parameter preparation.
    /// </summary>
    /// <typeparam name="T">PDO type</typeparam>
    public abstract class StuffDoer<T>
    {
        /// <summary>
        /// Executes an INSERT/UPDATE/DELETE command against the database.
        /// </summary>
        protected int Execute(IEnumerable<T> items, ExecutionContext context)
        {
            var sql = context.SqlContext.Sql;
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            context.Logger?.LogDebug("{location} Executing SQL: {Sql}", GetLoggingClassName(), sql);

            try
            {
                if (items == null || !items.Any())
                {
                    // No items → just run the SQL without parameters (manual or custom SQL)
                    return conn.Execute(sql, transaction: tran);
                }

                // Build parameters fresh (no caching, but accessors reused via PdoDataCache)
                var parameters = SqlContextHandler<T>.BuildParameters(context.SqlContext, items);

                return conn.Execute(sql, parameters, transaction: tran);
            }
            catch (Exception ex)
            {
                context.Logger?.LogError(ex, "{location} Error executing SQL.", GetLoggingClassName());
                throw;
            }
        }

        /// <summary>
        /// Executes a SELECT query that returns items.
        /// </summary>
        protected IEnumerable<T> Query(IEnumerable<T> items, ExecutionContext context)
        {
            var sql = context.SqlContext.Sql;
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            context.Logger?.LogDebug("{location} Querying SQL: {Sql}", GetLoggingClassName(), sql);

            try
            {
                if (items == null || !items.Any())
                {
                    // No items → just run raw SQL (custom/manual query)
                    return conn.Query<T>(sql, transaction: tran);
                }

                var parameters = SqlContextHandler<T>.BuildParameters(context.SqlContext, items);

                return conn.Query<T>(sql, parameters, transaction: tran);
            }
            catch (Exception ex)
            {
                context.Logger?.LogError(ex, "{location} Error querying SQL.", GetLoggingClassName());
                throw;
            }
        }

        /// <summary>
        /// Helper for consistent logging.
        /// </summary>
        protected string GetLoggingClassName()
        {
            return $"[StuffDoer<{typeof(T).Name}>]";
        }
    }
}
