using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Logging;
using PStructure.FunctionFeedback;

namespace PStructure.CRUDs
{
    /// <summary>
    /// Basisinterface für grundlegende Datenbanktransaktionen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrud<T>
    {
        int Execute(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc);

        IEnumerable<T> Query(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            ILogger logger,
            Func<ILogger, string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc);

        /// <summary>
        /// Fügt eine Instanz dem Datensatz hinzu.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        int Create(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger);

        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüssel passen.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger);

        /// <summary>
        /// Liest alle Datensätze der Tabelle aus.
        /// </summary>
        /// <param name="dbFeedback"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        IEnumerable<T> ReadAll(ref DbFeedback dbFeedback, ILogger logger);

        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        int Update(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger);

        /// <summary>
        /// Löscht einen Datensatz anhand eines Primärschlüssels.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback, ILogger logger);

        void ApplyTypeHandlersForObject();
    }
}
