using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;

namespace PStructure.PersistenceLayer
{
    public interface IItemManager<T>
    {
        void SetCrud(ICrud<T> crud);
        
        /// <summary>
        /// Inserts an item by instance, updating the database feedback.
        /// </summary>
        void CreateByInstance(T item, ref DbFeedback dbFeedback);

        /// <summary>
        /// Inserts a range of items by instance, updating the database feedback.
        /// </summary>
        void CreateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback);

        /// <summary>
        /// Reads an item by its primary key, updating the database feedback.
        /// </summary>
        IEnumerable<T> ReadByInstance(T item, ref DbFeedback dbFeedback);

        /// <summary>
        /// Reads a range of items by instance, updating the database feedback.
        /// </summary>
        IEnumerable<T> ReadByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback);

        /// <summary>
        /// Updates an item by instance, updating the database feedback.
        /// </summary>
        void UpdateByInstance(T item, ref DbFeedback dbFeedback);

        /// <summary>
        /// Updates a range of items by instance, updating the database feedback.
        /// </summary>
        void UpdateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback);

        /// <summary>
        /// Deletes an item by its primary key, updating the database feedback.
        /// </summary>
        void DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback);

        /// <summary>
        /// Deletes a range of items by their primary keys, updating the database feedback.
        /// </summary>
        void DeleteByPrimaryKeys(IEnumerable<T> items, ref DbFeedback dbFeedback);

        /// <summary>
        /// Outputs a test-friendly string representation of the manager.
        /// </summary>
        string ToStringForTest();
    }
}