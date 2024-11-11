using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.Models;
using PStructure.PersistenceLayer.PdoToTableMapping.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure.CRUDs
{
    public class SimpleCrud<T> : ClassCore, ICrud<T>
    {
        private readonly ISqlGenerator<T> _sqlGenerator;
        private readonly IMapperPdoQuery<T> _mapperPdoQuery;
        private readonly ITableLocation _tableLocation;

        public SimpleCrud(ISqlGenerator<T> sqlGenerator, IMapperPdoQuery<T> mapperPdoQuery, ITableLocation tableLocation)
        {
            _sqlGenerator = sqlGenerator;
            _mapperPdoQuery = mapperPdoQuery;
            _tableLocation = tableLocation;
            ApplyTypeHandlersForObject();
        }

        public int Execute(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = 0;
            var tableLocation = _tableLocation.PrintTableLocation();
            var sql = sqlGeneratorFunc(logger, tableLocation);

            logger?.LogInformation("{location} Executing for {EntityType} with SQL: {Sql}", PrintLocation(), typeof(T).Name, sql);

            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    logger?.LogDebug("{location} Executing SQL for item: {Item}", PrintLocation(), item);
                    result += dbFeedback.GetDbConnection().Execute(sql, parameters, dbFeedback.GetDbTransaction());
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "{location} Error executing SQL for item: {Item}", PrintLocation(), item);
                    throw;
                }
            }

            return result;
        }

        public IEnumerable<T> Query(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = new List<T>();
            var tableLocation = _tableLocation.PrintTableLocation();
            var sql = sqlGeneratorFunc(logger, tableLocation);

            logger?.LogInformation("{location} Executing fetching operation for {EntityType} with SQL: {Sql}", PrintLocation(), typeof(T).Name, sql);

            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    logger?.LogDebug("{location} Executing SQL for item: {Item}", PrintLocation(), item);
                    result.AddRange(dbFeedback.GetDbConnection().Query<T>(sql, parameters, dbFeedback.GetDbTransaction()));
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "{location} Error executing SQL for item: {Item}", PrintLocation(), item);
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
                (item, parameters) => _mapperPdoQuery.MapPropertiesToParameters(item, parameters));
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Query(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetReadSqlByPrimaryKey,
                (item, parameters) => _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters));
        }

        public IEnumerable<T> ReadAll(ref DbFeedback dbFeedback, ILogger logger)
        {
            var sql = _sqlGenerator.GetReadAll(logger, _tableLocation.PrintTableLocation());
            logger?.LogInformation("{location} Reading all items with SQL: {Sql}", PrintLocation(), sql);

            try
            {
                var result = dbFeedback.GetDbConnection().Query<T>(sql, transaction: dbFeedback.GetDbTransaction());
                logger?.LogInformation("{location} Read all items successful.", PrintLocation());
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{location} Error reading all items.", PrintLocation());
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
                    _mapperPdoQuery.MapPropertiesToParameters(item, parameters);
                    _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters);
                });
        }

        public int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            return Execute(
                items,
                ref dbFeedback,
                logger,
                _sqlGenerator.GetDeleteSqlByPrimaryKey,
                _mapperPdoQuery.MapPrimaryKeysToParameters);
        }

        public void ApplyTypeHandlersForObject()
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var handlerAttribute = property.GetCustomAttribute<TypeHandler>();
                if (handlerAttribute == null) continue;
                var handlerInstance = (SqlMapper.ITypeHandler)Activator.CreateInstance(handlerAttribute.HandlerType);
                SqlMapper.AddTypeHandler(property.PropertyType, handlerInstance);
            }
        }
    }
}
