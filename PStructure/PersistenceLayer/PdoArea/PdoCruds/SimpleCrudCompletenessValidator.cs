using System;
using System.Linq;
using PStructure.PersistenceLayer.Pdo.PdoData;

namespace PStructure.PersistenceLayer.PdoArea.PdoCruds.SimpleCrud
{
    public class PdoValidator<T>
    {
        /// <summary>
        ///     Validiert die gegebenen <see cref="PdoDataCache{T}" /> auf Vollständigkeit.
        ///     Notiz: Absichtlich nur Vollständigkeit, da Einschränkungen
        /// </summary>
        /// <typeparam name="T">Der Typ des Modells, das validiert wird.</typeparam>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn die Validierung fehlschlägt.</exception>
        public void Validate()
        {
            ValidatePrimaryKeyProperties();
            ValidateProperties();
        }

        /// <summary>
        ///     Validiert, dass die Entität mindestens einen Primärschlüssel hat.
        /// </summary>
        private static void ValidatePrimaryKeyProperties()
        {
            if (!PdoDataCache<T>.PrimaryKeyProperties.Any())
                throw new InvalidOperationException(
                    $"Der Typ {typeof(T).Name} muss mindestens einen Primärschlüssel haben.");
        }

        /// <summary>
        ///     Validiert, dass die Entität mindestens eine Eigenschaft hat.
        /// </summary>
        private static void ValidateProperties()
        {
            if (!PdoDataCache<T>.Properties.Any())
                throw new InvalidOperationException(
                    $"Der Typ {typeof(T).Name} muss mindestens eine Eigenschaft haben.");
        }
    }
}
