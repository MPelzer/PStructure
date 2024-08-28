using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Optional.Unsafe;
using PStructure.Interfaces;
using PStructure.Models;
using PStructure.SqlGenerator;

namespace PStructure.CRUDs
{
    public class BaseCrud<T> : IBaseCrud<T>
    {
        protected readonly ISqlGenerator _sqlGenerator;
        protected readonly IMapperPdoQuery<T> _mapperPdoQuery;
        protected readonly ITableLocation _tableLocation;
        private readonly ILogger<BaseCrud<T>> _logger;

        internal BaseCrud(
            ISqlGenerator sqlGenerator,
            IMapperPdoQuery<T> mapperPdoQuery,
            ITableLocation tableLocation,
            ILogger<BaseCrud<T>> logger)
        {
            _sqlGenerator = sqlGenerator;
            _mapperPdoQuery = mapperPdoQuery;
            _tableLocation = tableLocation;
            _logger = logger;
        }

        private TResult ExecuteOperation<TResult>(
            string sql,
            Action<T, DynamicParameters> mapParametersFunc,
            T item,
            ref DbCom dbCom,
            Func<IDbConnection, string, DynamicParameters, IDbTransaction, TResult> executeFunc)
        {
            var parameters = new DynamicParameters();
            mapParametersFunc(item, parameters);

            _logger.LogInformation("Executing SQL: {Sql} for item: {Item}", sql, item);

            try
            {
                _logger.LogDebug("Parameters: {Parameters}", parameters);
                var result = executeFunc(dbCom._dbConnection, sql, parameters, dbCom._transaction.ValueOrDefault());
                _logger.LogInformation("Operation successful for item: {Item}", item);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SQL for item: {Item}", item);
                throw;
            }
        }

        public T InsertByInstance(T item, ref DbCom dbCom)
        {
            var sql = _sqlGenerator.GetInsertSql(typeof(T), _tableLocation.printTableLocation());
            return ExecuteOperation(
                sql,
                (i, parameters) => _mapperPdoQuery.MapPdoToTable(i, parameters),
                item,
                ref dbCom,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        public IEnumerable<T> ReadByPrimaryKey(T item, ref DbCom dbCom)
        {
            var sql = _sqlGenerator.GetReadSqlByPrimaryKey(typeof(T), _tableLocation.printTableLocation());

            _logger.LogInformation("Reading items by primary key with SQL: {Sql}", sql);

            return ExecuteOperation(
                sql,
                _mapperPdoQuery.MapPrimaryKeysToParameters,
                item,
                ref dbCom,
                (conn, sqlQuery, parameters, txn) =>
                {
                    var results = new List<T>();
                    using (var reader = conn.ExecuteReader(sqlQuery, parameters, txn))
                    {
                        while (reader.Read())
                        {
                            T entity = Activator.CreateInstance<T>();
                            _mapperPdoQuery.MapTableColumnsToPdo(entity, reader);
                            results.Add(entity);
                        }
                    }
                    return results;
                });
        }

        public T UpdateByInstance(T item, ref DbCom dbCom)
        {
            var sql = _sqlGenerator.GetUpdateSqlByPrimaryKey(typeof(T), _tableLocation.printTableLocation());
            return ExecuteOperation(
                sql,
                (i, parameters) =>
                {
                    _mapperPdoQuery.MapPdoToTable(i, parameters);
                    _mapperPdoQuery.MapPrimaryKeysToParameters(i, parameters);
                },
                item,
                ref dbCom,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        T IBaseCrud<T>.DeleteByPrimaryKey(T item, ref DbCom dbCom)
        {
            var sql = _sqlGenerator.GetDeleteSqlByPrimaryKey(typeof(T), _tableLocation.printTableLocation());
            return ExecuteOperation(
                sql,
                _mapperPdoQuery.MapPrimaryKeysToParameters,
                item,
                ref dbCom,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        IEnumerable<T> IBaseCrud<T>.ReadAll(ref DbCom dbCom)
        {
            var sql = _sqlGenerator.GetSelectAll(_tableLocation.printTableLocation());

            _logger.LogInformation("Reading all items with SQL: {Sql}", sql);

            try
            {
                var result = dbCom._dbConnection.Query<T>(sql, transaction: dbCom._transaction.ValueOrDefault());
                _logger.LogInformation("Read all items successful.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading all items.");
                throw;
            }
        }
    }
}
