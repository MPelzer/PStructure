using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;

namespace PStructure.PersistenceLayer.Pdo
{
    public class PdoManager<T> : ClassCore, IItemManager<T>
    {
        protected ICrud<T> _crud;
        protected readonly ILogger _logger;
        
        
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
            throw new System.NotImplementedException();
        }

        public virtual void CreateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual IEnumerable<T> ReadByInstance(T item, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual IEnumerable<T> ReadByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual void UpdateByInstance(T item, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual void UpdateByInstances(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual void DeleteByPrimaryKey(T item, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public virtual void DeleteByPrimaryKeys(IEnumerable<T> items, ref DbFeedback dbFeedback)
        {
            throw new System.NotImplementedException();
        }

        public string ToStringForTest()
        {
            throw new System.NotImplementedException();
        }
    }
}