using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Unsafe;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.root;
using PStructure.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure
{
    
    public class ItemManager<T> : ClassCore, IItemManager<T> where T : new()
    {
        private readonly ICrud<T> _crud;
        private readonly IItemFactory<T> _itemFactory;
        private readonly ILogger _logger;
        
        /// <summary>
        /// Constructor for DefaultItemManager with BaseTableLocation and optional ILogger
        /// </summary>
        public ItemManager(ICrud<T> crud, ILogger<T> logger = null)
        {
            _itemFactory = new ItemFactory<T>();
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
            _logger?.LogDebug("{location} Start Executing {sqlType} for {count} items des Typs {type}",PrintLocation(), sqlType, itemCount, typeof(T));
        }
        
        /// <summary>
        /// Checks if the primary key is valid for an item.
        /// </summary>
        public bool IsPrimaryKeyValid()
        {
            throw new NotImplementedException("IsPrimaryKeyValid needs to be implemented.");
        }

        /// <summary>
        /// Outputs a test-friendly string representation of the manager.
        /// </summary>
        public string ToStringForTest()
        {
            throw new NotImplementedException("ToStringForTest needs to be implemented.");
        }

        /// <summary>
        /// Validates the PDO (PHP Data Object).
        /// </summary>
        public bool ValidatePdo()
        {
            throw new NotImplementedException("ValidatePdo needs to be implemented.");
        }
    }
}