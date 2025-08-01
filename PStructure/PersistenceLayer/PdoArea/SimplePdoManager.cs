using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PStructure.FunctionFeedback;
using PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface;
using PStructure.PersistenceLayer.PersistenceLayerFeedback;

namespace PStructure.PersistenceLayer.Pdo
{
    public class SimplePdoManager<T> : PdoManager<T>
    {
        public SimplePdoManager(ICrud<T> crud, ILogger logger = null) : base(crud, logger)
        {
        }

        /// <summary>
        ///     Inserts a range of items by instance, updating the database feedback.
        /// </summary>
        public override void CreateByInstance(T item, ref DbContext dbContext)
        {
            CreateByInstances(new List<T> { item }, ref dbContext);
        }

        /// <summary>
        ///     Inserts an item by instance, handling database feedback by reference.
        /// </summary>
        public override void CreateByInstances(IEnumerable<T> items, ref DbContext dbContext)
        {
            LogFunctionStart(ref items, "Create");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbContext,
                _logger,
                (ILogger logger, ref DbContext db) => _crud.Create(items, ref db, _logger),
                (ref DbContext db, Exception ex) =>
                {
                    // Optional additional error handling
                }
            );
        }

        /// <summary>
        ///     Reads an item by its primary key, updating the database feedback.
        /// </summary>
        public override IEnumerable<T> ReadByInstance(T item, ref DbContext dbContext)
        {
            return ReadByInstances(new List<T> { item }, ref dbContext);
        }

        /// <summary>
        ///     Reads an item by its primary key, updating the database feedback.
        /// </summary>
        public override IEnumerable<T> ReadByInstances(IEnumerable<T> items, ref DbContext dbContext)
        {
            LogFunctionStart(ref items, "Read");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbContext,
                _logger,
                (ILogger logger, ref DbContext db) => items = _crud.Read(items, ref db, _logger),
                (ref DbContext db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
            return items;
        }

        /// <summary>
        ///     Updates an item by instance, updating the database feedback.
        /// </summary>
        public override void UpdateByInstance(T item, ref DbContext dbContext)
        {
            UpdateByInstances(new List<T> { item }, ref dbContext);
        }

        /// <summary>
        ///     Updates a range of items by instance, updating the database feedback.
        /// </summary>
        public override void UpdateByInstances(IEnumerable<T> items, ref DbContext dbContext)
        {
            LogFunctionStart(ref items, "Update");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbContext,
                _logger,
                (ILogger logger, ref DbContext db) => _crud.Update(items, ref db, _logger),
                (ref DbContext db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
        }

        /// <summary>
        ///     Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        public override void DeleteByPrimaryKey(T item, ref DbContext dbContext)
        {
            DeleteByPrimaryKeys(new List<T> { item }, ref dbContext);
        }

        /// <summary>
        ///     Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        public override void DeleteByPrimaryKeys(IEnumerable<T> items, ref DbContext dbContext)
        {
            LogFunctionStart(ref items, "Delete");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbContext,
                _logger,
                (ILogger logger, ref DbContext db) => _crud.Delete(items, ref db, _logger),
                (ref DbContext db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
        }

        /// <summary>
        ///     Logging of given
        /// </summary>
        /// <param name="sqlType">Describes the desired outcome of the action</param>
        /// <param name="items">Set of data</param>
        /// <param name="dbFeedback"></param>
        private void LogFunctionStart(ref IEnumerable<T> items, string sqlType)
        {
            var itemCount = items is ICollection<T> collection ? collection.Count : items.Count();
            _logger?.LogDebug("{location} Start Executing {sqlType} for {count} items des Typs {type}",
                GetLoggingClassName(), sqlType, itemCount, typeof(T));
        }

        /// <summary>
        ///     Outputs a test-friendly string representation of the manager.
        /// </summary>
        public string ToStringForTest()
        {
            throw new NotImplementedException("ToStringForTest needs to be implemented.");
        }
    }
}