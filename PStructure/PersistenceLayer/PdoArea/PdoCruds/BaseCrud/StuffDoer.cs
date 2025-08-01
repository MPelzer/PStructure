using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud
{
    /// <summary>
    /// Implementierung einer CRUD, welche die grundlegenden Abläufe enthält, um Daten abzurufen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StuffDoer<T> : ClassCore
    {

        
        public StuffDoer()
        {
            ApplyTypeHandlersForObject();
        }

        /// <summary>
        /// Verarbeitet einen Ausführbefehl, oder Routine auf der Datenbank aus. 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="sqlGeneratorFunc"></param>
        /// <param name="mapParametersFunc"></param>
        /// <returns></returns>
        public int Execute(
            IEnumerable<T> items,
            ref DbContext dbContext,
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
                    result += dbContext.GetDbConnection().Execute(sql, parameters, dbContext.GetDbTransaction());
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

        /// <summary>
        /// Verarbeitet eine Anfrage an die Datenbank uns gibt dessen Antwort wieder
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="sqlGeneratorFunc"></param>
        /// <param name="mapParametersFunc"></param>
        /// <returns></returns>
        public IEnumerable<T> Query(
            IEnumerable<T> items,
            ref DbContext dbContext,
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
                    result.AddRange(dbContext.GetDbConnection()
                        .Query<T>(sql, parameters, dbContext.GetDbTransaction()));
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