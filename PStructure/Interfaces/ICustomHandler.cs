namespace PStructure.Interfaces
{
    public interface ICustomHandler
    {
        //todo: In den Implementierungen die Nullvalues prüfen bzw durch default abfangen.
        object Format(object value); // Formatting before inserting into DB
        object Parse(object value);  // Parsing after reading from DB
    }
}