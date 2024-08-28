using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using Optional.Unsafe;
using PStructure.Interfaces;
using PStructure.Models;
using PStructure.SqlGenerator;

namespace PStructure.CRUDs
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
    
    public class ExtendedCrud<T> : BaseCrud<T>, IExtendedCrud<T>
    {
        private readonly ILogger<ExtendedCrud<T>> _logger;
        public ExtendedCrud(
            ISqlGenerator sqlGenerator, 
            IMapperPdoQuery<T> mapperPdoQuery, 
            ITableLocation tableLocation, 
            ILogger<ExtendedCrud<T>> logger) 
            : base(sqlGenerator, mapperPdoQuery, tableLocation, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private T ExecuteBatchOperation(
            IEnumerable<T> items, 
            ref DbCom dbCom, 
            Func<Type, string, string> sqlGeneratorFunc, 
            Action<T, DynamicParameters> mapParametersFunc)
        {
            var result = default(T);
            var tableLocation = _tableLocation.printTableLocation();
            var sql = sqlGeneratorFunc(typeof(T), tableLocation);

            _logger.LogInformation("Executing batch operation for {EntityType} with SQL: {Sql}", typeof(T).Name, sql);

            foreach (var item in items)
            {
                var parameters = new DynamicParameters();
                mapParametersFunc(item, parameters);

                try
                {
                    _logger.LogDebug("Executing SQL for item: {Item}", item);
                    dbCom._dbConnection.Execute(sql, parameters, dbCom._transaction.ValueOrDefault());
                    result = item;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing SQL for item: {Item}", item);
                    throw;
                }
            }

            return result;
        }

        public IEnumerable<T> ReadRangeByPrimaryKeys(IEnumerable<T> items, ref DbCom dbCom)
        {
            return ExecuteBatchOperation(
                items, 
                ref dbCom, 
                _sqlGenerator.GetReadSqlByPrimaryKey, 
                (item, parameters) => _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters))
                .Yield();
        }

        public T UpdateByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {
            return ExecuteBatchOperation(
                items, 
                ref dbCom, 
                _sqlGenerator.GetUpdateSqlByPrimaryKey, 
                (item, parameters) =>
                {
                    _mapperPdoQuery.MapPdoToTable(item, parameters);
                    _mapperPdoQuery.MapPrimaryKeysToParameters(item, parameters);
                });
        }

        public T InsertByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {
            return ExecuteBatchOperation(
                items, 
                ref dbCom, 
                _sqlGenerator.GetInsertSql, 
                (item, parameters) => _mapperPdoQuery.MapPdoToTable(item, parameters));
        }

        public T DeleteByPrimaryKeys(IEnumerable<T> items, ref DbCom dbCom)
        {
            return ExecuteBatchOperation(
                items, 
                ref dbCom, 
                _sqlGenerator.GetDeleteSqlByPrimaryKey, 
                _mapperPdoQuery.MapPrimaryKeysToParameters);
        }
    }
}
