using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds
{
    public class SqlExecutionHandler<T> : ExecutionContextHandler<T>
    {
        private readonly ISqlGenerator<T> _sqlGenerator;
        private readonly ILogger _logger;

        public SqlExecutionHandler(ILogger logger = null)
        {
            _sqlGenerator = new DefaultSqlGenerator<T>();
            _logger = logger;
        }

        public int Create(IEnumerable<T> items, ExecutionContext context)
        {
            var (sql, factory) = _sqlGenerator.GetInsert();
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = factory;

            return Execute(items, context);
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ExecutionContext context)
        {
            var (sql, factory) = _sqlGenerator.GetReadByPk();
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = factory;

            return Query(items, context);
        }

        public IEnumerable<T> ReadAll(ExecutionContext context)
        {
            context.RequestContext.Sql = _sqlGenerator.GetReadAll();
            return context.DbContext.DbConnection.Query<T>(
                context.RequestContext.Sql,
                transaction: context.DbContext.DbTransaction);
        }

        public int Update(IEnumerable<T> items, ExecutionContext context)
        {
            var (sql, factory) = _sqlGenerator.GetUpdate();
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = factory;

            return Execute(items, context);
        }

        public int Delete(IEnumerable<T> items, ExecutionContext context)
        {
            var (sql, factory) = _sqlGenerator.GetDelete();
            context.RequestContext.Sql = sql;
            context.RequestContext.ParameterFactory = factory;

            return Execute(items, context);
        }

        /// <summary>
        /// Run custom raw SQL (single or multiple commands).
        /// </summary>
        public int RunBatch(ExecutionContext context, string sql)
        {
            context.RequestContext.Sql = sql;
            return ExecuteBatchSql(context);
        }
        
      
        
        
        
        private string GetLoggingClassName() => $"[Crud<{typeof(T).Name}>]";
    }
}
