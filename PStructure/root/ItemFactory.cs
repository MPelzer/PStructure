using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using PStructure.Interfaces;
// Verwenden von System.Text.Json für JSON-Serialisierung/Deserialisierung

namespace PStructure.root
{
    public class ItemFactory<T> : IItemFactory<T> where T : new()
    {
        /// <summary>
        /// Erstellt ein Item mit Standardwerten, indem der parameterlose Konstruktor von T aufgerufen wird.
        /// </summary>
        /// <returns>Eine neue Instanz von T mit Standardwerten.</returns>
        public T CreateDefaultEntry()
        {
            return new T(); // Nimmt an, dass T einen parameterlosen Konstruktor hat
            //Todo: Muss noch hier definert werden.
        }

        /// <summary>
        /// Klont das angegebene Item, indem eine neue Instanz erstellt und alle Eigenschaften kopiert werden.
        /// </summary>
        /// <param name="itemToClone">Das zu klonende Item.</param>
        /// <returns>Eine neue Instanz von T mit kopierten Eigenschaften des Original-Items.</returns>
        public T CloneEntry(T itemToClone)
        {
            if (itemToClone == null)
            {
                throw new ArgumentNullException(nameof(itemToClone));
            }

            var clonedItem = new T();
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(clonedItem, prop.GetValue(itemToClone));
                }
            }

            return clonedItem;
        }

        /// <summary>
        /// Konvertiert das Item in eine JSON-String-Darstellung.
        /// </summary>
        /// <returns>Die JSON-String-Darstellung des Items.</returns>
        public string ToJson(T item)
        {
            return JsonSerializer.Serialize(item);
        }

        /// <summary>
        /// Erstellt ein Item aus einem JSON-String.
        /// </summary>
        /// <param name="json">Der JSON-String, der deserialisiert werden soll.</param>
        /// <returns>Eine Instanz von T, die mit Daten aus dem JSON-String gefüllt ist.</returns>
        public T FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("JSON-String darf nicht null oder leer sein", nameof(json));
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Gibt eine Zeichenfolgen-Darstellung des Objekts im Format "KlassenName: Wert, Eigenschaft1: Wert, Eigenschaft2: Wert, ..." zurück.
        /// </summary>
        /// <returns>Eine Zeichenfolge, die das Objekt und seine Eigenschaften darstellt.</returns>
        public string ToStringForTest()
        {
            var item = CreateDefaultEntry(); // Annahme: Wir wollen die String-Darstellung eines Standard-Entries
            var typeName = typeof(T).Name;
            var propertyValues = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanRead)
                .Select(prop => $"{prop.Name}: {prop.GetValue(item)}");

            return $"{typeName}: {string.Join(", ", propertyValues)}";
        }
    }
}
