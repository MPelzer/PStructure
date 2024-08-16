using System.Collections.Generic;
using Dapper;

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
        IEnumerable<T> ReadRangeByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, ref DbCom dbCom);
        
        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüssel passen.
        /// </summary>
        /// <param name="compoundPrimaryKeys"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadRangeByPrimaryKeys(IEnumerable<ICompoundPrimaryKey> compoundPrimaryKeys, ref DbCom dbCom);
        
        /// <summary>
        /// Aktualisiert eine Menge an Datensätzen anhand ihrer Instanzen
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> UpdateRangeByInstances(IEnumerable<T> items, ref DbCom dbCom);

        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T UpdateByInstance(T item, ref DbCom dbCom);
        
        /// <summary>
        /// Fügt eine Menge an Instanzen dem Datensatz hinzu
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, ref DbCom dbCom);
        
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
        /// <param name="compoundPrimaryKey"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T DeleteByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, ref DbCom dbCom);

        /// <summary>
        /// Löscht eine Menge an Datensätzen anhand eines (Teil)-Primärschlüssels
        /// Trivia: Löschen sollte aus Sicherheitsgründen ein bewusster Prozess sein und deswegen
        /// nicht über Instanzen direkt geschehen dürfen.
        /// </summary>
        /// <param name="compoundPrimaryKeys"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> DeleteRangeByPrimaryKeys(IEnumerable<ICompoundPrimaryKey> compoundPrimaryKeys, ref DbCom dbCom);

        /// <summary>
        /// Liest alle Datensätze der Tabelle aus.
        /// </summary>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadAll(ref DbCom dbCom);

        void MapPdoToTable(T item, DynamicParameters parameters);
    }
}