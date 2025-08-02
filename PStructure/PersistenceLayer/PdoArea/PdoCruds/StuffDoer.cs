using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds
{
    /// <summary>
    /// Universelle Ausführungsklasse für SQL-Operationen über Items.
    /// Unterstützt automatische Parameter-Generierung und flexible Kontextausführung.
    /// </summary>
    /// <typeparam name="T">Typ des Datenobjekts</typeparam>
    public class StuffDoer<T> : ClassCore
    {
        private readonly IExecutionContext _context;

        public StuffDoer(IExecutionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.Validate();
        }

        /// <summary>
        /// Führt einen SQL-Befehl für ein einzelnes Objekt oder ein Batch aus, basierend auf dem Kontext.
        /// </summary>
        /// <param name="itemOrItems">Einzelobjekt oder Liste von Objekten</param>
        /// <returns>Anzahl betroffener Zeilen</returns>
        public int Execute(object itemOrItems)
        {
            switch (_context.DbContext.ProcessingType)
            {
                case ProcessingType.Single:
                    return ExecuteSingle(itemOrItems);

                case ProcessingType.Bulk:
                    return ExecuteBatch((IEnumerable<object>)itemOrItems);

                default:
                    throw new NotSupportedException($"Unsupported processing type: {_context.DbContext.ProcessingType}");
            }
        }

        private int ExecuteSingle(object item)
        {
            var db = _context.DbContext.DbConnection;
            var sql = _context.SqlContext.Sql;
            var transaction = _context.DbContext.DbTransaction;

            var parameters = SqlContextHandler.Generate(item, _context.SqlContext);

            _context.Logger?.LogDebug("Executing SQL: {Sql} with parameters {Params}", sql, parameters);
            return db.Execute(sql, parameters, transaction);
        }

        private int ExecuteBatch(IEnumerable<object> items)
        {
            var db = _context.DbContext.DbConnection;
            var sql = _context.SqlContext.Sql;
            var transaction = _context.DbContext.DbTransaction;

            var parametersList = SqlContextHandler.GenerateBatch(items, _context.SqlContext);

            _context.Logger?.LogDebug("Executing SQL batch for {Count} items", parametersList.Count());

            int affected = 0;
            foreach (var parameters in parametersList)
            {
                affected += db.Execute(sql, parameters, transaction);
            }

            return affected;
        }

        /// <summary>
        /// Optionaler Shortcut für direkte Ausführung mit Typed-T
        /// </summary>
        public int ExecuteTyped(T item)
        {
            return Execute(item);
        }

        /// <summary>
        /// Optionaler Shortcut für Bulk-Ausführung mit Typed-T
        /// </summary>
        public int ExecuteTypedBatch(IEnumerable<T> items)
        {
            return Execute(items.Cast<object>());
        }
    }
}
