using System.Collections.Generic;
using Dapper;

namespace PStructure.PersistenceLayer.DatabaseStuff.Handler
{
    /// <summary>
    /// Handler for direct SQL operations.
    /// </summary>
    public class SqlExecutionHandler<T> : IExecutionHandler<SqlRequestContext, T>
    {
        public int Execute(ExecutionContext<SqlRequestContext> context)
        {
            context.Validate();

            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;
            var sql = context.RequestContext.Sql;
            var parameters = context.RequestContext.Parameters;

            return conn.Execute(sql, parameters, tran);
        }

        public IEnumerable<T> Query(ExecutionContext<SqlRequestContext> context)
        {
            context.Validate();

            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;
            var sql = context.RequestContext.Sql;
            var parameters = context.RequestContext.Parameters;

            return conn.Query<T>(sql, parameters, tran);
        }
    }
}