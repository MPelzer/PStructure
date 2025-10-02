using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    /// <summary>
    /// Execution handler for entity-based operations.
    /// Items are translated into SQL + parameters by a statement builder.
    /// </summary>
    public class ItemExecutionHandler<T> : IExecutionHandler<T>
    {
        private readonly IEnumerable<T> _items;
        private readonly Func<IEnumerable<T>, (string Sql, object Parameters)> _statementBuilder;

        public ItemExecutionHandler(
            IEnumerable<T> items,
            Func<IEnumerable<T>, (string Sql, object Parameters)> statementBuilder)
        {
            _items = items;
            _statementBuilder = statementBuilder;
        }

        public void PrepareStatement(ExecutionContext context)
        {
            var (sql, _) = _statementBuilder(_items);
            context.RequestContext.Sql = sql;
        }

        public void PrepareParameters(ExecutionContext context)
        {
            var (_, parameters) = _statementBuilder(_items);
            context.RequestContext.ParameterFactory = _ => parameters;
        }

        public void Validate(ExecutionContext context)
        {
            if (_items == null || !_items.Any())
                context.AddValidationError("No items provided for ItemExecutionHandler.");

            context.Validate();
        }

        public int Execute(ExecutionContext context)
        {
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;
            return conn.Execute(context.RequestContext.Sql,
                                context.RequestContext.ParameterFactory(null), tran);
        }

        public IEnumerable<T> Query(ExecutionContext context)
        {
            var conn = context.DbContext.DbConnection;
            var tran = context.DbContext.DbTransaction;
            return conn.Query<T>(context.RequestContext.Sql,
                                 context.RequestContext.ParameterFactory(null), tran);
        }
    }
}
