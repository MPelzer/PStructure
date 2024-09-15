using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Unsafe;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Interfaces;
using PStructure.Mapper;
using PStructure.Models;
using PStructure.root;
using PStructure.SqlGenerator;
using PStructure.TableLocation;

namespace PStructure
{
    public class DefaultItemManager<T> : IItemManager<T> where T : new()
    {
        private readonly ExtendedCrud<T> _extendedCrud;
        private readonly IItemFactory<T> _itemFactory;

        /// <summary>
        /// Constructor for DefaultItemManager with BaseTableLocation and optional ILogger
        /// </summary>
        public DefaultItemManager(BaseTableLocation tableLocation, ILogger<T> logger = null)
        {
            _itemFactory = new ItemFactory<T>();
            var mapper = new MapperPdoQuery<T>();
            var sqlGenerator = new BaseSqlGenerator<T>();
            _extendedCrud = new ExtendedCrud<T>(sqlGenerator, mapper, tableLocation, logger);
        }

        /// <summary>
        /// Constructor for DefaultItemManager with an existing ExtendedCrud and optional IItemFactory.
        /// </summary>
        public DefaultItemManager(ExtendedCrud<T> extendedCrud, Option<IItemFactory<T>> itemFactory)
        {
            _extendedCrud = extendedCrud;
            _itemFactory = itemFactory.ValueOrDefault();
        }

        /// <summary>
        /// Inserts an item by instance, handling database feedback by reference.
        /// </summary>
        public T InsertByInstance(T item, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _extendedCrud.InsertByInstance(item, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Optional additional error handling
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
            );
            return item;
        }

        /// <summary>
        /// Reads an item by its primary key, updating the database feedback.
        /// </summary>
        public T ReadByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => item = _extendedCrud.ReadByPrimaryKey(item, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
            );
            return item;
        }

        /// <summary>
        /// Inserts a range of items by instance, updating the database feedback.
        /// </summary>
        public IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _extendedCrud.InsertByInstances(items, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
            );
            return items;
        }

        /// <summary>
        /// Updates an item by instance, updating the database feedback.
        /// </summary>
        public T UpdateByInstance(T item, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _extendedCrud.UpdateByInstance(item, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
            );
            return item;
        }

        /// <summary>
        /// Updates a range of items by instance, updating the database feedback.
        /// </summary>
        public IEnumerable<T> UpdateRangeByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _extendedCrud.UpdateByInstances(items, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
            );
            return items;
        }

        /// <summary>
        /// Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        public void DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: (ref DbFeedback db) => _extendedCrud.DeleteByPrimaryKey(item, ref db),
                onException: (ref DbFeedback db, Exception ex) =>
                {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbFeedback db) => db.RequestAnswer
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
