using System;
using System.Collections.Generic;
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
    public class DefaultItemManager<T> : IItemManager<T> where T : new()
    {
        private readonly ICrud<T> _crud;
        private readonly IItemFactory<T> _itemFactory;

        /// <summary>
        /// Constructor for DefaultItemManager with BaseTableLocation and optional ILogger
        /// </summary>
        public DefaultItemManager(BaseTableLocation tableLocation, ILogger<T> logger = null)
        {
            _itemFactory = new ItemFactory<T>();
            var mapper = new MapperPdoQuery<T>();
            var sqlGenerator = new BaseSqlGenerator<T>();
            _crud = new SimpleCrud<T>(sqlGenerator, mapper, tableLocation, logger);
        }

        /// <summary>
        /// Constructor for DefaultItemManager with an existing ExtendedCrud and optional IItemFactory.
        /// </summary>
        public DefaultItemManager(ICrud<T> crud, Option<IItemFactory<T>> itemFactory)
        {
            _crud = crud;
            _itemFactory = itemFactory.ValueOrDefault();
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
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _crud.Create(items, ref db),
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
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => items = _crud.Read(items, ref db),
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
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _crud.Update(items, ref db),
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
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _crud.Delete(items, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                }
            );
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
