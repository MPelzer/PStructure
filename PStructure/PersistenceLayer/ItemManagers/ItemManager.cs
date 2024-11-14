using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.root;

namespace PStructure.PersistenceLayer
{
    
    public class ItemManager<T> : ClassCore 
    {
        private readonly ICrud<T> _crud;
        private readonly ILogger _logger;
        
        /// <summary>
        /// Constructor for DefaultItemManager with BaseTableLocation and optional ILogger
        /// </summary>
        public ItemManager(ICrud<T> crud, ILogger logger = null)
        {
            _logger = logger;
            _crud = crud;
        }
        
        /// <summary>
        /// Inserts a range of items by instance, updating the database feedback.
        /// </summary>
        public void CreateByInstance(T item, ref DbFeedback dbFeedback)
        {
            CreateByInstances(new List<T> { item }, ref dbFeedback);
        }
        
        /// <summary>
        /// Inserts an item by instance, handling database feedback by reference.
        /// </summary>
        public void CreateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            LogFunctionStart(ref items,"Create");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _logger,
                action: (ILogger logger, ref DbFeedback db) => _crud.Create(items, ref db, _logger),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Optional additional error handling
                }
            );
        }

        /// <summary>
        /// Reads an item by its primary key, updating the database feedback.
        /// </summary>
        public IEnumerable<T> ReadByInstance(T item, ref DbFeedback dbFeedback)
        {
            return ReadByInstances(new List<T> { item }, ref dbFeedback);
        }
        
        /// <summary>
        /// Reads an item by its primary key, updating the database feedback.
        /// </summary>
        public IEnumerable<T> ReadByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            LogFunctionStart(ref items,"Read");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _logger,
                action: (ILogger logger, ref DbFeedback db) => items = _crud.Read(items, ref db, _logger),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
            return items;
        }
        
        /// <summary>
        /// Updates an item by instance, updating the database feedback.
        /// </summary>
        public  void UpdateByInstance(T item, ref DbFeedback dbFeedback)
        {
             UpdateByInstances(new List<T> { item }, ref dbFeedback);
        }

        /// <summary>
        /// Updates a range of items by instance, updating the database feedback.
        /// </summary>
        public void UpdateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            LogFunctionStart(ref items,"Update");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _logger,
                action: (ILogger logger, ref DbFeedback db) => _crud.Update(items, ref db, _logger),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
        }

        /// <summary>
        /// Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        public void DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            DeleteByPrimaryKeys(new List<T> { item }, ref dbFeedback);
        }
        
        /// <summary>
        /// Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        public void DeleteByPrimaryKeys(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            LogFunctionStart(ref items,"Delete");
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _logger,
                action: (ILogger logger, ref DbFeedback db) => _crud.Delete(items, ref db, _logger),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
        }
        
        /// <summary>
        /// Logging of given 
        /// </summary>
        /// <param name="sqlType">Describes the desired outcome of the action</param>
        /// <param name="items">Set of data</param>
        /// <param name="dbFeedback"></param>
        private void LogFunctionStart(ref IEnumerable<T> items, string sqlType)
        {
            var itemCount = items is ICollection<T> collection ? collection.Count : items.Count();
            _logger?.LogDebug("{location} Start Executing {sqlType} for {count} items des Typs {type}",GetLoggingClassName(), sqlType, itemCount, typeof(T));
        }

        /// <summary>
        /// Outputs a test-friendly string representation of the manager.
        /// </summary>
        public string ToStringForTest()
        {
            throw new NotImplementedException("ToStringForTest needs to be implemented.");
        }
    }
}