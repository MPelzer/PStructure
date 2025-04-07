using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PStructure.PersistenceLayer.Pdo.PdoCruds.BaseCrud;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;
using PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;
using Microsoft.Extensions.Configuration;

namespace PStructure.PersistenceLayer.Pdo.PdoData
{
    /// <summary>
    /// Cache in ROM-Form für Eigenschaften eines PDOs. Einmal geladen, bleibt es bestehen bis zum Neustart der Anwendung.
    /// </summary>
    /// <typeparam name="T">Typ des PDOs</typeparam>
    public static class PdoDataCache<T>
    {
        private static readonly Lazy<PropertyInfo[]> _propertyData = new(() =>
            typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<PdoPropertyAttributes.Column>() != null)
                .ToArray()
        );

        private static readonly Lazy<PropertyInfo[]> _primaryKeyData = new(() =>
            typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<PdoPropertyAttributes.PrimaryKey>() != null)
                .ToArray()
        );
        public static PropertyInfo[] Properties => _propertyData.Value;
        public static PropertyInfo[] PrimaryKeyProperties => _primaryKeyData.Value;

        private static readonly ConcurrentDictionary<WorkMode, TableLocation> _tableLocationCache = new();
        public static TableLocation GetTableLocation(WorkMode mode)
        {
            return _tableLocationCache.GetOrAdd(mode, m =>
            {
                var config = SimplePdoDataGlobal.Configuration;
                var section = config.GetSection("TableMappings");
                var allMappings = section.Get<Dictionary<string, List<TableLocation>>>();

                var typeName = typeof(T).Name;

                if (!allMappings.TryGetValue(typeName, out var mappings))
                    throw new InvalidOperationException($"No config mapping found for type '{typeName}'.");

                var match = mappings.FirstOrDefault(e => e.Mode == mode);
                if (match == null)
                    throw new InvalidOperationException($"No mapping for type '{typeName}' and WorkMode '{mode}'.");

                return match;
            });
        }

        public static IEnumerable<WorkMode> CachedWorkModes => _tableLocationCache.Keys;
    }

    /// <summary>
    /// Plain config DTO used to map JSON configuration entries.
    /// </summary>
    public class TableLocationConfig
    {
        public WorkMode Mode { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }

        public string PrintTableLocation() => $"{Schema}.{TableName}";
    }
}
