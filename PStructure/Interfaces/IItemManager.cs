namespace PStructure
{
    /// <summary>
    /// Kümmert sich um die Koordination aller Module, die sich um die Instanzen oder den Datensätzen zu tun haben.
    /// </summary>
    public interface IItemManager
    {
        /// <summary>
        /// Prüft, ob der Primärschlüssel valide ist. 
        /// </summary>
        /// <returns></returns>
        bool IsPrimaryKeyValid();

        string ToStringForTest();
        
    }
}