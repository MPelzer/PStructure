using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds.BaseCrud;

public class Crud<T> : StuffDoer<T>
{
    public int Create(IEnumerable<T> items, ref DbContext dbContext, ILogger logger)
        {
            return Execute(
                items,
                ref dbContext,
                logger,
                _sqlGenerator.GetInsertSql,
                (item, parameters) => _mapper.MapPropertiesToParameters(item, parameters));
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ref DbContext dbContext, ILogger logger)
        {
            return Query(
                items,
                ref dbContext,
                logger,
                _sqlGenerator.GetReadSqlByPrimaryKey,
                (item, parameters) => _mapper.MapPrimaryKeysToParameters(item, parameters));
        }

        public IEnumerable<T> ReadAll(ref DbContext dbContext, ILogger logger)
        {
            var sql = _sqlGenerator.GetReadAll(logger);
            logger?.LogInformation("{location} Reading all items with SQL: {Sql}", GetLoggingClassName(), sql);

            try
            {
                var result = dbContext.GetDbConnection().Query<T>(sql, transaction: dbContext.GetDbTransaction());
                logger?.LogInformation("{location} Read all items successful.", GetLoggingClassName());
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{location} Error reading all items.", GetLoggingClassName());
                throw;
            }
        }

        public int Update(IEnumerable<T> items, ref DbContext dbContext, ILogger logger)
        {
            return Execute(
                items,
                ref dbContext,
                logger,
                _sqlGenerator.GetUpdateSqlByPrimaryKey,
                (item, parameters) =>
                {
                    _mapper.MapPropertiesToParameters(item, parameters);
                    _mapper.MapPrimaryKeysToParameters(item, parameters);
                });
        }

        public int Delete(IEnumerable<T> items, ref DbContext dbContext, ILogger logger)
        {
            return Execute(
                items,
                ref dbContext,
                logger,
                _sqlGenerator.GetDeleteSqlByPrimaryKey,
                _mapper.MapPrimaryKeysToParameters);
        }
}