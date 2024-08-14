using System.Data.Common;
using PStructure.Interfaces;

namespace PStructure
{
    public interface IItemFactory <T>
    {
        /// <summary>
        /// Erstellt ein Item mit Standardwerten.               
        /// </summary>
        /// <returns></returns>
        T CreateDefaultEntry();
        
        /// <summary>
        /// Erstellt ein Objekt mit 
        /// </summary>
        /// <param name="compoundPrimaryKey"></param>
        /// <returns></returns>
        T CreateEntryByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey);

        /// <summary>
        /// Erstellt aus den Eigenschaften, die den Priärschlüssel binden, eine Objektinstanz
        /// </summary>
        /// <param name="originalItem"></param>
        /// <returns></returns>
        ICompoundPrimaryKey CreatePrimaryKeyByItem(T originalItem);
        
        /// <summary>
        /// Erstellt einen leeren Primärschlüssel zu dem PDO
        /// </summary>
        /// <returns></returns>
        ICompoundPrimaryKey CreatePrimaryKey();
        
        /// <summary>
        /// Klont das PDO
        /// </summary>
        /// <param name="originalItem"></param>
        /// <returns></returns>
        T CloneEntry(T originalItem);
    }
}