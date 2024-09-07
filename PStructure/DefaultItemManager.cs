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

        public DefaultItemManager(BaseTableLocation tableLocation, ILogger<T> logger = null)
        {
            _itemFactory = new ItemFactory<T>();
            var mapper = new MapperPdoQuery<T>();
            var sqlGenerator = new BaseSqlGenerator<T>();
            _extendedCrud = new ExtendedCrud<T>(sqlGenerator,mapper,tableLocation,logger);
        }
        
        /// <summary>
        /// Ist für die komplette Verwaltung einer Datenbankrepräsentation (PDO) zuständig
        /// </summary>
        /// <param name="extendedCrud"></param>
        /// <param name="itemFactory"></param>
        public DefaultItemManager(ExtendedCrud<T> extendedCrud, Option<IItemFactory<T>> itemFactory)
        {
            _extendedCrud = extendedCrud;
            _itemFactory = itemFactory.ValueOrDefault();
        }
        public T InsertByInstance(T item, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => _extendedCrud.InsertByInstance(item, ref db),
                onException: (ref DbCom db, Exception ex) => {
                    //Optionales weiteres Fehlerhandling neben dem Crud-Standardverhalten
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return item;
        }

        public T ReadByPrimaryKey(T item, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => item = _extendedCrud.ReadByPrimaryKey(item, ref db),
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return item;
        }

        public IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) => {
                    _extendedCrud.InsertByInstances(items, ref db);
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
                action: (ref DbCom db) => _extendedCrud.UpdateByInstance(item, ref db),
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
                    _extendedCrud.UpdateByInstances(items, ref db);
                },
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );

            return items;
        }
        public void DeleteByPrimaryKey(T item, ref DbCom dbCom)
        {
            DbComHandler.ExecuteWithTransaction(
                ref dbCom,
                action: (ref DbCom db) =>
                {
                    _extendedCrud.DeleteByPrimaryKey(item, ref db);
                },
                onException: (ref DbCom db, Exception ex) => {
                    // Handle exception if necessary
                },
                commitCondition: (ref DbCom db) => db.requestAnswer
            );
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
