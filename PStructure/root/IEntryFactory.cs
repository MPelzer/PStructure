namespace PStructure.Interfaces
{
    public interface IItemFactory <T>
    {
        /// <summary>
        /// Erstellt ein Item mit Standardwerten.               
        /// </summary>
        /// <returns></returns>
        T CreateDefaultEntry();
        
        /// <summary>
        /// Klont das PDO
        /// </summary>
        /// <param name="itemToClone"></param>
        /// <returns></returns>
        T CloneEntry(T itemToClone);
        
        /// <summary>
        /// Ein Item zu JSON
        /// </summary>
        /// <returns></returns>
        string ToJson();

        /// <summary>
        /// Ein Item von JSON
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        T FromJson(string json);
        
        /// <summary>
        /// Gibt das Objekt in Schreibweise KlassenName:value,Eigenschlaft1:value,Eigenschlaft2:value,... aus.
        /// </summary>
        /// <returns></returns>
        string ToStringForTest();
        
    }
}