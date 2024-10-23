using System;
using System.Collections.Generic;
using Dapper;
using PStructure.FunctionFeedback;

namespace PStructure.CRUDs
{
    /// <summary>
    /// Basisinterface für grundlegende Datenbnaktransaktionen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrud <T> 
    {
        int Execute(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            Func<string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc);

        IEnumerable<T> Query(
            IEnumerable<T> items,
            ref DbFeedback dbFeedback,
            Func<string, string> sqlGeneratorFunc,
            Action<T, DynamicParameters> mapParametersFunc);
        
        /// <summary>
        /// Fügt eine Instanz dem Datensatz hinzu. 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <returns></returns>
        int Create(IEnumerable<T> items, ref DbFeedback dbFeedback);
        
        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüssel passen.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <returns></returns>
        IEnumerable<T> Read(IEnumerable<T> items, ref DbFeedback dbFeedback);
        
        /// <summary>
        /// Liest alle Datensätze der Tabelle aus.
        /// </summary>
        /// <param name="dbFeedback"></param>
        /// <returns></returns>
        IEnumerable<T> ReadAll(ref DbFeedback dbFeedback);
        
        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <returns></returns>
        int Update(IEnumerable<T> items, ref DbFeedback dbFeedback);
        
        /// <summary>
        /// Löscht einen Datensatz anhand eines Primätschlüssels.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbFeedback"></param>
        /// <returns></returns>
        int Delete(IEnumerable<T> items, ref DbFeedback dbFeedback);

        void ApplyTypeHandlersForObject();

    }
}