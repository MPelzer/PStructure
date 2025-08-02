using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PStructure
{
    public class ClassCore
    {
        public string GetLoggingClassName([CallerMemberName] string functionName = null)
        {
            return $"{GetType().Name}.{functionName}:";
        }

        /// <summary>
        /// Gibt alle Eigenschaften der aktuellen Instanz mit ihrem Namen und Wert in die Konsole aus.
        /// Nützlich zum Debuggen und Testen von Datenobjekten.
        /// </summary>
        public void PrintAllProperties()
        {
            Console.WriteLine($"== {GetType().Name} Property Dump ==");

            foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(this);
                Console.WriteLine($"{prop.Name} = {value}");
            }

            Console.WriteLine($"== End of {GetType().Name} ==");
        }
    }
}