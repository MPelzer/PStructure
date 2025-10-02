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
    /// Base class providing common Dapper execution methods.
    /// </summary>
    public abstract class ExecutionContextHandler<T>
    {
        protected int Execute(IEnumerable<T> items, ExecutionContext context)
        {
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            var parameters = context.RequestContext.ParameterFactory?.Invoke(items.Cast<object>())
                             ?? items?.Cast<object>();

            return conn.Execute(context.RequestContext.Sql, parameters, tran);
        }

        protected IEnumerable<T> Query(IEnumerable<T> items, ExecutionContext context)
        {
            
            
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            var parameters = context.RequestContext.ParameterFactory?.Invoke(items.Cast<object>())
                             ?? items?.Cast<object>();

            return conn.Query<T>(context.RequestContext.Sql, parameters, tran);
        }

        /// <summary>
        /// Executes raw SQL (can contain multiple statements).
        /// </summary>
        protected int ExecuteBatchSql(ExecutionContext context)
        {
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            return conn.Execute(context.RequestContext.Sql, transaction: tran);
        }

        /// <summary>
        /// Multi-mapping support (two objects combined into one).
        /// </summary>
        protected IEnumerable<TReturn> QueryMulti<T1, T2, TReturn>(
            ExecutionContext context,
            Func<T1, T2, TReturn> map,
            string splitOn = "Id")
        {
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            return conn.Query(context.RequestContext.Sql, map, transaction: tran, splitOn: splitOn);
        }
        
        /// <summary>
        /// Run a custom SELECT query with user-supplied SQL and parameters.
        /// Useful for filtering or custom WHERE clauses.
        /// </summary>
        public IEnumerable<T> QueryCustom(ExecutionContext context, string sql, object parameters = null)
        {
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = _ => parameters; 

            return Query(null, context); // items aren't needed, only parameters
        }

        /// <summary>
        /// Run a custom SQL command (INSERT/UPDATE/DELETE).
        /// </summary>
        public int ExecuteCustom(ExecutionContext context, string sql, object parameters = null)
        {
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = _ => parameters;

            return Execute(null, context);
        }
        protected string GetLoggingClassName() => $"[StuffDoer<{typeof(T).Name}>]";
    }
}
