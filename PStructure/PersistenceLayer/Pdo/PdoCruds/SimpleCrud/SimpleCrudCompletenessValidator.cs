using System;
using System.Linq;
using PStructure.PersistenceLayer.PdoData;

namespace PStructure.PersistenceLayer.Pdo.PdoValidation
{
    public class PdoValidator<T> : IValidator<T>
    {
        /// <summary>
        /// Validiert die gegebenen <see cref="PdoDataCache{T}"/> auf Vollständigkeit.
        /// Notiz: Absichtlich nur Vollständigkeit, da Einschränkungen 
        /// </summary>
        /// <typeparam name="T">Der Typ des Modells, das validiert wird.</typeparam>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn die Validierung fehlschlägt.</exception>
        public void Validate()
        {
            ValidatePrimaryKeyProperties();
            ValidateProperties();
            ValidateTableLocationMode();
            ValidateSchema();
            ValidateTableName();
        }

        /// <summary>
        /// Validiert, dass die Entität mindestens einen Primärschlüssel hat.
        /// </summary>
        private static void ValidatePrimaryKeyProperties()
        {
            if (!PdoDataCache<T>.PrimaryKeyProperties.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen Primärschlüssel haben.");
            }
        }

        /// <summary>
        /// Validiert, dass die Entität mindestens eine Eigenschaft hat.
        /// </summary>
        private static void ValidateProperties()
        {
            if (!PdoDataCache<T>.Properties.Any())
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
            
            if (!PdoDataCache<T>.TableLocationData.Any())
            {
                throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation haben.");
            }

            foreach (var mode in workModes)
            {
                if (PdoDataCache<T>.TableLocationData.All(tl => tl.Mode != mode))
                {
                    throw new InvalidOperationException($"Der Typ {typeof(T).Name} muss mindestens einen TableLocation für den WorkMode '{mode}' haben.");
                }
            }
        }

        /// <summary>
        /// Validiert, dass das Schema nicht null oder leer ist.
        /// </summary>
        private static void ValidateSchema()
        {
            var tableLocations = PdoDataCache<T>.TableLocationData.ToArray();

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
            var tableLocations = PdoDataCache<T>.TableLocationData.ToArray();

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
