using System.Collections.Generic;

namespace PStructure.Interfaces
{
    public interface IExtendedCrud<T> : IBaseCrud<T>
    {
        /// <summary>
        /// Liest eine Menge an Datensätzen aus, die auf den gegebenen Primärschlüsseln passen.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        IEnumerable<T> ReadRangeByPrimaryKeys(IEnumerable<T> items, ref DbCom dbCom);
        
        /// <summary>
        /// Aktualisiert einen Datensatz anhand seiner Instanz
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T UpdateByInstances(IEnumerable<T> items, ref DbCom dbCom);
        
        /// <summary>
        /// Fügt eine Instanz dem Datensatz hinzu. 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T InsertByInstances(IEnumerable<T> items, ref DbCom dbCom);

        /// <summary>
        /// Löscht einen Datensatz anhand eines Primätschlüssels.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dbCom"></param>
        /// <returns></returns>
        T DeleteByPrimaryKeys(IEnumerable<T> items, ref DbCom dbCom);
    }
}