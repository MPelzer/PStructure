using System.Collections.Generic;
using PStructure.FunctionFeedback;

namespace PStructure.CRUDs
{
    /// <summary>
    /// Basisinterface für grundlegende Datenbnaktransaktionen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseCrud <T> 
    {
        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüssel passen.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadByPrimaryKey(T item, ref DbCom dbCom);
        
        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T UpdateByInstance(T item, ref DbCom dbCom);
        
        /// <summary>
        /// Fügt eine Instanz dem Datensatz hinzu. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T InsertByInstance(T item, ref DbCom dbCom);

        /// <summary>
        /// Löscht einen Datensatz anhand eines Primätschlüssels.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T DeleteByPrimaryKey(T item, ref DbCom dbCom);
        
        /// <summary>
        /// Liest alle Datensätze der Tabelle aus.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadAll(ref DbCom dbCom);
    }
}