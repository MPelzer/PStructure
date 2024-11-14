using System;
using System.Linq;
using System.Text.Json;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.root
{
    /// <summary>
    /// Provides utility methods for creating and handling items of a specified type.
    /// </summary>
    public static class ItemFactory<T>
    {
        /// <summary>
        /// Creates an item with default values by calling the parameterless constructor of T.
        /// </summary>
        /// <typeparam name="T">The type of item to create.</typeparam>
        /// <returns>A new instance of T with default values.</returns>
        public static T CreateDefaultEntry<T>() where T : new()
        {
            return new T();
        }

        /// <summary>
        /// Clones the specified item by creating a new instance and copying all properties.
        /// </summary>
        /// <typeparam name="T">The type of item to clone.</typeparam>
        /// <param name="itemToClone">The item to clone.</param>
        /// <returns>A new instance of T with copied properties from the original item.</returns>
        public static T CloneEntry<T>(T itemToClone) where T : new()
        {
            if (itemToClone == null) throw new ArgumentNullException(nameof(itemToClone));

            var clonedItem = new T();
            foreach (var prop in PdoMetadata<T>.Properties)
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(clonedItem, prop.GetValue(itemToClone));
                }
            }

            return clonedItem;
        }

        /// <summary>
        /// Converts the item to a JSON string representation.
        /// </summary>
        /// <typeparam name="T">The type of item to convert to JSON.</typeparam>
        /// <param name="item">The item to serialize to JSON.</param>
        /// <returns>A JSON string representation of the item.</returns>
        public static string ToJson<T>(T item)
        {
            return JsonSerializer.Serialize(item);
        }

        /// <summary>
        /// Creates an item from a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of item to create from JSON.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An instance of T populated with data from the JSON string.</returns>
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Provides a string representation of the default object of type T, listing each property and its value.
        /// </summary>
        /// <typeparam name="T">The type of item to represent as a string.</typeparam>
        /// <returns>A string representation of the object and its properties.</returns>
        public static string ToStringForTest<T>() where T : new()
        {
            var item = CreateDefaultEntry<T>();
            var typeName = typeof(T).Name;
            var propertyValues = PdoMetadata<T>.Properties
                .Where(prop => prop.CanRead)
                .Select(prop => $"{prop.Name}: {prop.GetValue(item)}");

            return $"{typeName}: {string.Join(", ", propertyValues)}";
        }
    }
}
