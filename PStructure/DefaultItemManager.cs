using System;
using System.Collections.Generic;
using Optional;
using Optional.Unsafe;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Interfaces;
using PStructure.Models;

namespace PStructure
{
    public class DefaultItemManager<T> : IItemManager<T>
    {
        private readonly ExtendedCrud<T> _defaultCrud;

        private readonly IItemFactory<T> _itemFactory;

        /// <summary>
        /// Ist für die komplette Verwaltung einer Datenbankrepräsentation (PDO) zuständig
        /// </summary>
        /// <param name="defaultCrud"></param>
        /// <param name="itemFactory"></param>
        public DefaultItemManager(ExtendedCrud<T> defaultCrud, Option<IItemFactory<T>> itemFactory)
        {
            _defaultCrud = defaultCrud;
            _itemFactory = itemFactory.ValueOrDefault();
        }
        public T InsertByInstance(T item, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => _defaultCrud.InsertByInstance(item, ref db),
                onException: (ref DbCom db, Exception ex) => {
                    //Optionales weiteres Fehlerhandling neben dem Crud-Standardverhalten
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return item;
        }

        public IEnumerable<T> ReadRangeByPrimaryKey(T item, ref DbCom dbCom)
        {
            IEnumerable<T> items = null;
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => items = _defaultCrud.ReadByPrimaryKey(item, ref db),
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return items;
        }

        public IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => {
                    _defaultCrud.InsertByInstances(items, ref db);
                },
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return items;
        }

        public T UpdateByInstance(T item, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => _defaultCrud.UpdateByInstance(item, ref db),
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return item;
        }

        public IEnumerable<T> UpdateRangeByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) =>
                {
                    _defaultCrud.UpdateByInstances(items, ref db);
                },
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return items;
        }
        public bool IsPrimaryKeyValid()
        {
            throw new NotImplementedException("IsPrimaryKeyValid needs to be implemented.");
        }

        public string ToStringForTest()
        {
            throw new NotImplementedException("ToStringForTest needs to be implemented.");
        }

        public bool ValidatePdo()
        {
            throw new NotImplementedException("ValidatePdo needs to be implemented.");
        }
    }
}
