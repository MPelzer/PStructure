using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;

namespace PStructure.PersistenceLayer.Pdo
{
    public class PdoManager<T> : ClassCore, IItemManager<T>
    {
        protected readonly ILogger _logger;
        protected ICrud<T> _crud;


        public PdoManager(ICrud<T> crud, ILogger logger = null)
        {
            _logger = logger;
            _crud = crud;
        }

        public void SetCrud(ICrud<T> crud)
        {
            _crud = crud;
        }

        public virtual void CreateByInstance(T item, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual void CreateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> ReadByInstance(T item, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> ReadByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateByInstance(T item, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteByPrimaryKeys(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new NotImplementedException();
        }

        public string ToStringForTest()
        {
            throw new NotImplementedException();
        }
    }
}