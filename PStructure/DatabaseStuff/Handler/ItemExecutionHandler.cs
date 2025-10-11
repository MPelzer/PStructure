using System;
using System.Collections.Generic;
using Dapper;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    /// <summary>
    /// Handler for item-based Dapper operations, including optional multi-mapping.
    /// </summary>
    public class ItemExecutionHandler<T> : IExecutionHandler<ItemRequestContext<T>, T>
    {
        public int Execute(ExecutionContext<ItemRequestContext<T>> context)
        {
            context.Validate();

            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            var (sql, parameters) = context.RequestContext.StatementBuilder(context.RequestContext.Items);
            return conn.Execute(sql, parameters, tran);
        }

        public IEnumerable<T> Query(ExecutionContext<ItemRequestContext<T>> context)
        {
            context.Validate();

            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;

            var (sql, parameters) = context.RequestContext.StatementBuilder(context.RequestContext.Items);
            return conn.Query<T>(sql, parameters, tran);
        }

        #region Multi-Mapping

        public IEnumerable<TReturn> QueryMulti<T1, T2, TReturn>(
            ExecutionContext<ItemRequestContext<T>> context,
            Func<T1, T2, TReturn> map,
            string splitOn = "Id")
        {
            context.Validate();

            var (sql, parameters) = context.RequestContext.StatementBuilder(context.RequestContext.Items);
            return context.DbContext.DbConnection.Query(sql, map, parameters, context.DbContext.DbTransaction, splitOn: splitOn);
        }

        public IEnumerable<TReturn> QueryMulti<T1, T2, T3, TReturn>(
            ExecutionContext<ItemRequestContext<T>> context,
            Func<T1, T2, T3, TReturn> map,
            string splitOn = "Id")
        {
            context.Validate();

            var (sql, parameters) = context.RequestContext.StatementBuilder(context.RequestContext.Items);
            return context.DbContext.DbConnection.Query(sql, map, parameters, context.DbContext.DbTransaction, splitOn: splitOn);
        }

        // Add more overloads if necessary, but these two usually cover most Dapper use-cases.

        #endregion
    }
}
