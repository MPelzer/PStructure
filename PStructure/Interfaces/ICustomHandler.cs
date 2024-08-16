namespace PStructure.Interfaces
{
    public interface ICustomHandler
    {
        object Format(object value); // Formatting before inserting into DB
        object Parse(object value);  // Parsing after reading from DB
    }
}