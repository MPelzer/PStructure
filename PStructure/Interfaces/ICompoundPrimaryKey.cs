namespace PStructure.Interfaces
{
    /// <summary>
    /// Kapselt eine Menge an Eigenschaften, die den Primärschlüssel bilden.
    /// </summary>
    public interface ICompoundPrimaryKey
    {
        
        bool IsFullySet();
        string ToStringForTest();
    }
}