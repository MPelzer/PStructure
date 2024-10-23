using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.Logging;
using Optional.Unsafe;
using PStructure.DapperSqlDateTimeMappers;
using PStructure.FunctionFeedback;
using PStructure.Interfaces;
using PStructure.Interfaces.DapperSqlDateTimeMappers;
using PStructure.Mapper;
using PStructure.Models;
using PStructure.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure.CRUDs
{
    public class SimpleCrud<T> : ICrud<T>
    {
        private readonly ISqlGenerator _sqlGenerator;
        private readonly IMapperPdoQuery<T> _mapperPdoQuery;
        private readonly ITableLocation _tableLocation;
        private readonly ILogger<T> _logger;

        public SimpleCrud(
            ISqlGenerator sqlGenerator,
            IMapperPdoQuery<T> mapperPdoQuery,
            ITableLocation tableLocation,
            ILogger<T> logger)
        {
            _sqlGenerator = sqlGenerator;
            _mapperPdoQuery = mapperPdoQuery;
            _tableLocation = tableLocation;
            _logger = logger;
            ApplyTypeHandlersForObject<T>();
        }
        public int Execute(
            IEnumerable<T> items, 
            ref DbFeedback dbFeedback, 
            Func<Type, string, string> sqlGeneratorFunc, 
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = 0;
            var tableLocation = _tableLocation.PrintTableLocation();
            var sql = sqlGeneratorFunc(typeof(T), tableLocation);
            
            _logger?.LogInformation("Executing for {EntityType} with SQL: {Sql}", typeof(T).Name, sql);
            
            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    _logger?.LogDebug("Executing SQL for item: {Item}", item);
                    result += dbFeedback.GetDbConnection().Execute(sql, parameters, dbFeedback.GetDbTransaction());
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error executing SQL for item: {Item}", item);
                    throw;
                }
            }

            return result;
        }
        public IEnumerable<T> Query(
            IEnumerable<T> items, 
            ref DbFeedback dbFeedback, 
            Func<Type, string, string> sqlGeneratorFunc, 
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = new List<T>();
            var tableLocation = _tableLocation.PrintTableLocation();
            var sql = sqlGeneratorFunc(typeof(T), tableLocation);
            
            _logger?.LogInformation("Executing for {EntityType} with SQL: {Sql}", typeof(T).Name, sql);
            
            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);
                try
                {
                    _logger?.LogDebug("Executing SQL for item: {Item}", item);
                    result.AddRange(dbFeedback.GetDbConnection().Query<T>(sql, parameters, dbFeedback.GetDbTransaction()));
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error executing SQL for item: {Item}", item);
                    throw;
                }
            }

            return result;
        }
        
        public int Create(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            return Execute(
                items, 
                ref dbFeedback, 
                _sqlGenerator.GetInsertSql, 
                (item, parameters) => _mapperPdoQuery.MapPropertiesToParameters(item, parameters));
        }
        public IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            return Query(
                items, 
                ref dbFeedback, 
                _sqlGenerator.GetReadSqlByPrimaryKey, 
                (item, parameters) => _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters));
        }
        
        public IEnumerable<T> ReadAll(ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetSelectAll(_tableLocation.PrintTableLocation());

            _logger.LogInformation("Reading all items with SQL: {Sql}", sql);

            try
            {
                var result = dbFeedback.GetDbConnection().Query<T>(sql, transaction: dbFeedback.GetDbTransaction());
                _logger.LogInformation("Read all items successful.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading all items.");
                throw;
            }
        }
        public int Update(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            return Execute(
                items, 
                ref dbFeedback, 
                _sqlGenerator.GetUpdateSqlByPrimaryKey, 
                (item, parameters) =>
                {
                    _mapperPdoQuery.MapPropertiesToParameters(item, parameters);
                    _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters);
                });
        }
        
        public int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            return Execute(
                items, 
                ref dbFeedback, 
                _sqlGenerator.GetDeleteSqlByPrimaryKey, 
                _mapperPdoQuery.MapPrimaryKeysToParameters);
        }
        
        public void ApplyTypeHandlersForObject<T>()
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var handlerAttribute = property.GetCustomAttribute<TypeHandlerAttribute>();
                if (handlerAttribute == null) continue;
                var handlerInstance = (SqlMapper.ITypeHandler)Activator.CreateInstance(handlerAttribute.HandlerType);
                SqlMapper.AddTypeHandler(property.PropertyType, handlerInstance);
            }
        }

    }
    
}
