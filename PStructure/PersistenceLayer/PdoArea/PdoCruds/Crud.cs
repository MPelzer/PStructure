using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds
{
    public class Crud<T> : StuffDoer<T>
    {
        private readonly ISqlGenerator<T> _sqlGenerator;
        private readonly ILogger _logger;

        public Crud(ILogger logger = null)
        {
            _sqlGenerator = new DefaultSqlGenerator<T>();
            _logger = logger;
        }

        public int Create(IEnumerable<T> items, ExecutionContext context)
        {
            context.SqlContext.Sql = _sqlGenerator.GetInsertSql(_logger);
            context.SqlContext.ParameterizingType = SqlParameterizingType.Named;

            return Execute(items, context);
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ExecutionContext context)
        {
            context.SqlContext.Sql = _sqlGenerator.GetReadSqlByPrimaryKey(_logger);
            context.SqlContext.ParameterizingType = SqlParameterizingType.Named;

            return Query(items, context);
        }

        public IEnumerable<T> ReadAll(ExecutionContext context)
        {
            var sql = _sqlGenerator.GetReadAll(_logger);
            context.SqlContext.Sql = sql;

            _logger?.LogInformation("{location} Reading all items with SQL: {Sql}", GetLoggingClassName(), sql);

            try
            {
                return context.DbContext.GetDbConnection()
                    .Query<T>(sql, transaction: context.DbContext.GetDbTransaction());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{location} Error reading all items.", GetLoggingClassName());
                throw;
            }
        }

        public int Update(IEnumerable<T> items, ExecutionContext context)
        {
            context.SqlContext.Sql = _sqlGenerator.GetUpdateSqlByPrimaryKey(_logger);
            context.SqlContext.ParameterizingType = SqlParameterizingType.Named;

            return Execute(items, context);
        }

        public int Delete(IEnumerable<T> items, ExecutionContext context)
        {
            context.SqlContext.Sql = _sqlGenerator.GetDeleteSqlByPrimaryKey(_logger);
            context.SqlContext.ParameterizingType = SqlParameterizingType.Named;

            return Execute(items, context);
        }

        private string GetLoggingClassName()
        {
            return $"[Crud<{typeof(T).Name}>]";
        }
    }
}
