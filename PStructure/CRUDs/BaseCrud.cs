using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Optional.Unsafe;
using PStructure.FunctionFeedback;
using PStructure.Interfaces;
using PStructure.Mapper;
using PStructure.Models;
using PStructure.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure.CRUDs
{
    public class BaseCrud<T> : IBaseCrud<T>
    {
        protected readonly ISqlGenerator _sqlGenerator;
        protected readonly IMapperPdoQuery<T> _mapperPdoQuery;
        protected readonly ITableLocation _tableLocation;
        private readonly ILogger<T> _logger;

        public BaseCrud(
            ISqlGenerator sqlGenerator,
            IMapperPdoQuery<T> mapperPdoQuery,
            ITableLocation tableLocation,
            ILogger<T> logger)
        {
            _sqlGenerator = sqlGenerator;
            _mapperPdoQuery = mapperPdoQuery;
            _tableLocation = tableLocation;
            _logger = logger;
        }
        
        /// <summary>
        /// Definiert den Grundablauf für Datenbankanfragen. Dazu gehört u.A das Abbilden von Tabellenspalten-Pdo Eigenschaften
        /// Verbindungen, Logging und 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="mapParametersFunc"></param>
        /// <param name="item"></param>
        /// <param name="dbFeedback"></param>
        /// <param name="executeFunc"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        private TResult ExecuteOperation<TResult>(
            string sql,
            Action<T, DynamicParameters> mapParametersFunc,
            T item,
            ref DbFeedback dbFeedback,
            Func<IDbConnection, string, DynamicParameters, IDbTransaction, TResult> executeFunc)
        {
            var parameters = new DynamicParameters();
            mapParametersFunc(item, parameters);

            _logger.LogInformation("Executing SQL: {Sql} for item: {Item}", sql, item);

            try
            {
                _logger.LogDebug("Parameters: {Parameters}", parameters);
                var result = executeFunc(dbFeedback.DbConnection, sql, parameters, dbFeedback.DbTransaction);
                _logger.LogInformation("Operation successful for item: {Item}", item);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SQL for item: {Item}", item);
                throw;
            }
        }

        public T InsertByInstance(T item, ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetInsertSql(typeof(T), _tableLocation.PrintTableLocation());
            return ExecuteOperation(
                sql,
                (i, parameters) => _mapperPdoQuery.MapPdoToTable(i, parameters),
                item,
                ref dbFeedback,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        public T ReadByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetReadSqlByPrimaryKey(typeof(T), _tableLocation.PrintTableLocation());

            return ExecuteOperation(
                sql,
                _mapperPdoQuery.MapPrimaryKeysToParameters,
                item,
                ref dbFeedback,
                (conn, sqlQuery, parameters, txn) =>
                {
                    T entity = Activator.CreateInstance<T>();
                    using (var reader = conn.ExecuteReader(sqlQuery, parameters, txn))
                    {
                        while (reader.Read())
                        {
                            _mapperPdoQuery.MapTableColumnsToPdo(entity, reader);
                            
                        }
                    }

                    return entity;
                });
        }

        public T UpdateByInstance(T item, ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetUpdateSqlByPrimaryKey(typeof(T), _tableLocation.PrintTableLocation());
            return ExecuteOperation(
                sql,
                (i, parameters) =>
                {
                    _mapperPdoQuery.MapPdoToTable(i, parameters);
                    _mapperPdoQuery.MapPrimaryKeysToParameters(i, parameters);
                },
                item,
                ref dbFeedback,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        public T DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetDeleteSqlByPrimaryKey(typeof(T), _tableLocation.PrintTableLocation());
            return ExecuteOperation(
                sql,
                _mapperPdoQuery.MapPrimaryKeysToParameters,
                item,
                ref dbFeedback,
                (conn, sqlQuery, parameters, txn) => { conn.Execute(sqlQuery, parameters, txn); return item; });
        }

        public IEnumerable<T> ReadAll(ref DbFeedback dbFeedback)
        {
            var sql = _sqlGenerator.GetSelectAll(_tableLocation.PrintTableLocation());

            _logger.LogInformation("Reading all items with SQL: {Sql}", sql);

            try
            {
                var result = dbFeedback.DbConnection.Query<T>(sql, transaction: dbFeedback.DbTransaction);
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
