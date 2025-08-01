using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;

namespace PStructure.PersistenceLayer.Pdo.PdoData
{
    /// <summary>
    /// Cache für PDO-Metadaten auf Typ-Ebene.
    /// Speichert reflektierte Eigenschaften eines PDO-Typs, inklusive Spalteninformationen,
    /// Primärschlüsseldefinitionen und vorberechneter Lambda-Accessoren für performanten Zugriff.
    /// </summary>
    /// <typeparam name="T">Der Typ des Datenobjekts (PDO).</typeparam>
    public static class PdoDataCache<T>
    {
        // Reflektierte Properties mit [Column]-Attribut
        private static readonly Lazy<PropertyInfo[]> _propertyData = new(() =>
            typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>() != null)
                .ToArray()
        );

        // Reflektierte Properties mit [PrimaryKey]-Attribut
        private static readonly Lazy<PropertyInfo[]> _primaryKeyData = new(() =>
            typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<PdoPropertyAttributes.PrimaryKey>() != null)
                .ToArray()
        );

        // Spaltennamen der Properties mit [Column]-Attribut
        private static readonly Lazy<string[]> _columnNames = new(() =>
            Properties
                .Select(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>()?.ColumnName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray()
        );

        // Kompilierte Lambda-Accessoren zum Auslesen der Property-Werte eines Objekts
        private static readonly Lazy<Dictionary<string, Func<T, object>>> _columnAccessors = new(BuildColumnAccessors);

        /// <summary>
        /// Gibt alle Properties mit [Column]-Attribut zurück.
        /// </summary>
        public static PropertyInfo[] Properties => _propertyData.Value;

        /// <summary>
        /// Gibt alle Properties mit [PrimaryKey]-Attribut zurück.
        /// </summary>
        public static PropertyInfo[] PrimaryKeyProperties => _primaryKeyData.Value;

        /// <summary>
        /// Gibt alle in [Column] definierten Spaltennamen zurück.
        /// </summary>
        public static string[] ColumnNames => _columnNames.Value;

        /// <summary>
        /// Gibt kompilierte Accessoren zum Auslesen der Property-Werte eines Objekts zurück.
        /// Schlüssel ist jeweils der Spaltenname.
        /// </summary>
        public static Dictionary<string, Func<T, object>> ColumnAccessors => _columnAccessors.Value;

        /// <summary>
        /// Erstellt Zugriffsfunktionen (Lambdas) für alle Spalten, um Werte aus Objekten effizient auszulesen.
        /// </summary>
        private static Dictionary<string, Func<T, object>> BuildColumnAccessors()
        {
            var accessors = new Dictionary<string, Func<T, object>>();

            foreach (var prop in Properties)
            {
                var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                if (columnAttr == null || string.IsNullOrWhiteSpace(columnAttr.ColumnName))
                    continue;

                var columnName = columnAttr.ColumnName;
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, prop);
                var convert = Expression.Convert(propertyAccess, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(convert, parameter).Compile();

                accessors[columnName] = lambda;
            }

            return accessors;
        }
    }
}
