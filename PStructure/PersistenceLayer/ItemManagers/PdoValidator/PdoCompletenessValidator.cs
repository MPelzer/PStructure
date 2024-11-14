using System;
using System.Linq;
using PStructure.Models;
namespace PStructure.PersistenceLayer.PdoData
{
    public static class PdoValidator<T> : IValidator<T>
    {
        /// <summary>
        /// Validiert die gegebenen <see cref="PdoMetadata{T}"/> auf Vollständigkeit.
        /// Notiz: Absichtlich nur Vollständigkeit, da Einschränkungen 
        /// </summary>
        /// <typeparam name="T">Der Typ des Modells, das validiert wird.</typeparam>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn die Validierung fehlschlägt.</exception>
        public static void Validate()
        {
            ValidatePrimaryKeyProperties();
            ValidateProperties();
            ValidateTableLocationMode();
            ValidateDatabase();
            ValidateSchema();
            ValidateTableName();
        }

        /// <summary>
        /// Validiert, dass die Entität mindestens einen Primärschlüssel hat.
        /// </summary>
        private static void ValidatePrimaryKeyProperties()
        {
            if (!PdoMetadata<T>.PrimaryKeyProperties.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen Primärschlüssel haben.");
            }
        }

        /// <summary>
        /// Validiert, dass die Entität mindestens eine Eigenschaft hat.
        /// </summary>
        private static void ValidateProperties()
        {
            if (!PdoMetadata<T>.Properties.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens eine Eigenschaft haben.");
            }
        }

        /// <summary>
        /// Validiert den TableLocation Mode, dass alle WorkModes vorhanden sind.
        /// </summary>
        private static void ValidateTableLocationMode()
        {
            var workModes = Enum.GetValues(typeof(WorkMode)).Cast<WorkMode>().ToArray();
            
            if (!PdoMetadata<T>.TableLocationData.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation haben.");
            }

            foreach (var mode in workModes)
            {
                if (PdoMetadata<T>.TableLocationData.All(tl => tl.Mode != mode))
                {
                    throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation für den WorkMode '{mode}' haben.");
                }
            }
        }

        /// <summary>
        /// Validiert, dass die Datenbank nicht null oder leer ist und dass alle Attribute vorhanden sind.
        /// </summary>
        private static void ValidateDatabase()
        {
            var tableLocations = PdoMetadata<T>.TableLocationData.ToArray();

            if (!tableLocations.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation haben.");
            }

            foreach (var tableLocation in tableLocations)
            {
                if (string.IsNullOrEmpty(tableLocation.Database.ToString()))
                {
                    throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss eine gültige, nicht-Undefined Datenbank haben.");
                }
            }
        }

        /// <summary>
        /// Validiert, dass das Schema nicht null oder leer ist.
        /// </summary>
        private static void ValidateSchema()
        {
            var tableLocations = PdoMetadata<T>.TableLocationData.ToArray();

            if (!tableLocations.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation haben.");
            }

            foreach (var tableLocation in tableLocations)
            {
                if (string.IsNullOrEmpty(tableLocation.Schema))
                {
                    throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss ein nicht-null, nicht-leeres Schema haben.");
                }
            }
        }

        /// <summary>
        /// Validiert, dass der Tabellenname nicht null oder leer ist.
        /// </summary>
        private static void ValidateTableName()
        {
            var tableLocations = PdoMetadata<T>.TableLocationData.ToArray();

            if (!tableLocations.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation haben.");
            }

            foreach (var tableLocation in tableLocations)
            {
                if (string.IsNullOrEmpty(tableLocation.TableName))
                {
                    throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss einen nicht-null, nicht-leeren Tabellenname haben.");
                }
            }
        }
    }
}
