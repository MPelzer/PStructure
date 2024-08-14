using System.Collections.Generic;

namespace PStructure.Interfaces
{
    /// <summary>
    /// Basisinterface 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrud <T> 
    {
        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüssel passen.
        /// </summary>
        /// <param name="compoundPrimaryKey"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadRangeByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, out DbCom dbCom);
        
        /// <summary>
        /// Aktualisiert eine Menge an Datensätzen anhand ihrer Instanzen
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> UpdateRangeByInstances(IEnumerable<T> items, out DbCom dbCom);

        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T UpdateByInstance(T item, out DbCom dbCom);
        
        /// <summary>
        /// Fügt eine Menge an Instanzen dem Datensatz hinzu
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, out DbCom dbCom);
        
        /// <summary>
        /// Fügt eine Instanz dem Datensatz hinzu. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T InsertByInstance(T item, out DbCom dbCom);

        /// <summary>
        /// Löscht einen Datensatz anhand eines Primätschlüssels.
        /// </summary>
        /// <param name="compoundPrimaryKey"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T DeleteByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, out DbCom dbCom);

        /// <summary>
        /// Löscht eine Menge an Datensätzen anhand eines (Teil)-Primärschlüssels
        /// Trivia: Löschen sollte aus Sicherheitsgründen ein bewusster Prozess sein und deswegen
        /// nicht über Instanzen direkt geschehen dürfen.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> DeleteRangeByPrimaryKeys(IEnumerable<T> items, out DbCom dbCom);

        /// <summary>
        /// Liest alle Datensätze der Tabelle aus.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadAll(IEnumerable<T> items, out DbCom dbCom);
    }
}