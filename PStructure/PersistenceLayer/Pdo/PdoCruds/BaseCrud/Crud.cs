using System;
using System.Collections.Generic;
using System.Net;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;

namespace PStructure.PersistenceLayer.ItemManagers.PdoToTableMapping.Cruds
{
    public class Crud<T> : ICrud<T>
    {
        public int Execute(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger, Func<ILogger, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Query(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger, Func<ILogger, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc)
        {
            throw new NotImplementedException();
        }

        public int Create(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReadAll(ref DbFeedback dbFeedback, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public int Update(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger)
        {
            throw new NotImplementedException();
        }

        public void ApplyTypeHandlersForObject()
        {
            throw new NotImplementedException();
        }
    }
}