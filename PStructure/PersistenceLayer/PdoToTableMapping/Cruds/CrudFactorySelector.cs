using Microsoft.Extensions.Logging;
using System;
using PStructure.PersistenceLayer;
using PStructure.PersistenceLayer.PdoToTableMapping;

namespace PStructure.CRUDs
{
    public static class CrudFactorySelector<T> where T : class, new()
    {
        public static ICrud<T> CreateCrud(WorkMode mode, CrudType crudType, ILogger logger = null)
        {
            switch (crudType)
            {
                case CrudType.Simple:
                    return SimpleCrudFactory<T>.CreateC(mode, logger);
                case CrudType.Advanced:
                    return AdvancedCrudFactory<T>.Create(mode, logger);
                case CrudType.Complex:
                    return ComplexCrudFactory<T>.Create(mode, logger);
                case CrudType.Report:
                    return ReportCrudFactory<T>.Create(mode, logger);
                case CrudType.Audit:
                    return AuditCrudFactory<T>.Create(mode, logger);
                default:
                    throw new NotSupportedException($"CrudType '{crudType}' is not supported.");
            }
        }
    }
}