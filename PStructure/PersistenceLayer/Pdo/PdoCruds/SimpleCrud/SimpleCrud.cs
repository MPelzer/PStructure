using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;

namespace PStructure.PersistenceLayer.Pdo.PdoToTableMapping.SimpleCrud
{
    public class SimpleCrud<T> : ClassCore, ICrud<T>
    {
        private readonly IMapper<T> _mapper;
        private readonly ISqlGenerator<T> _sqlGenerator;

        public SimpleCrud(ISqlGenerator<T> sqlGenerator, IMapper<T> mapper)
        {
            _sqlGenerator = sqlGenerator;
            _mapper = mapper;
            ApplyTypeHandlersForObject();
        }

        public int Execute(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = 0;
            var sql = sqlGeneratorFunc(logger);

            logger?.LogInformation("{location} Executing for {EntityType} with SQL: {Sql}", GetLoggingClassName(),
                typeof(T).Name, sql);

            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    logger?.LogDebug("{location} Executing SQL for item: {Item}", GetLoggingClassName(), item);
                    result += dbFeedback.GetDbConnection().Execute(sql, parameters, dbFeedback.GetDbTransaction());
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "{location} Error executing SQL for item: {Item}", GetLoggingClassName(),
                        item);
                    throw;
                }
            }

            return result;
        }

        public IEnumerable<T> Query(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = new List<T>();
            var sql = sqlGeneratorFunc(logger);

            logger?.LogInformation("{location} Executing fetching operation for {EntityType} with SQL: {Sql}",
                GetLoggingClassName(), typeof(T).Name, sql);

            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    logger?.LogDebug("{location} Executing SQL for item: {Item}", GetLoggingClassName(), item);
                    result.AddRange(dbFeedback.GetDbConnection()
                        .Query<T>(sql, parameters, dbFeedback.GetDbTransaction()));
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "{location} Error executing SQL for item: {Item}", GetLoggingClassName(),
                        item);
                    throw;
                }
            }

            return result;
        }

        public int Create(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Execute(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetInsertSql,
                (item, parameters) => _mapper.MapPropertiesToParameters(item, parameters));
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Query(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetReadSqlByPrimaryKey,
                (item, parameters) => _mapper.MapPrimaryKeysToParameters(item, parameters));
        }

        public IEnumerable<T> ReadAll(ref DbFeedback dbFeedback, ILogger logger)
        {
            var sql = _sqlGenerator.GetReadAll(logger);
            logger?.LogInformation("{location} Reading all items with SQL: {Sql}", GetLoggingClassName(), sql);

            try
            {
                var result = dbFeedback.GetDbConnection().Query<T>(sql, transaction: dbFeedback.GetDbTransaction());
                logger?.LogInformation("{location} Read all items successful.", GetLoggingClassName());
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{location} Error reading all items.", GetLoggingClassName());
                throw;
            }
        }

        public int Update(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Execute(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetUpdateSqlByPrimaryKey,
                (item, parameters) =>
                {
                    _mapper.MapPropertiesToParameters(item, parameters);
                    _mapper.MapPrimaryKeysToParameters(item, parameters);
                });
        }

        public int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Execute(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetDeleteSqlByPrimaryKey,
                _mapper.MapPrimaryKeysToParameters);
        }

        public void ApplyTypeHandlersForObject()
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var handlerAttribute = property.GetCustomAttribute<PdoPropertyAttributes.TypeHandler>();
                if (handlerAttribute == null) continue;
                var handlerInstance = (SqlMapper.ITypeHandler)Activator.CreateInstance(handlerAttribute.HandlerType);
                SqlMapper.AddTypeHandler(property.PropertyType, handlerInstance);
            }
        }
    }
}