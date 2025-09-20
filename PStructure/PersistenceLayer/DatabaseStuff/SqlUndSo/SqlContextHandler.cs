using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.Pdo.PdoData;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    /// <summary>
    /// Handles parameter creation for SQL execution based on SqlContext.
    /// No caching of DynamicParameters – relies on column accessors for fast reuse.
    /// </summary>
    public static class SqlContextHandler<T>
    {
        /// <summary>
        /// Builds the parameters for the given items and SqlContext.
        /// </summary>
        public static object BuildParameters(SqlContext context, IEnumerable<T> items)
        {
            switch (context.ParameterizingType)
            {
                case SqlParameterizingType.Named:
                    return BuildNamedParameters(items);

                case SqlParameterizingType.Indexed:
                    return BuildIndexedParameters(items);

                default:
                    throw new NotSupportedException($"Unsupported parameterizing type {context.ParameterizingType}");
            }
        }

        /// <summary>
        /// Generates parameters where each column is bound by name.
        /// Uses column accessors from PdoDataCache for speed.
        /// </summary>
        private static IEnumerable<DynamicParameters> BuildNamedParameters(IEnumerable<T> items)
        {
            var accessors = PdoDataCache<T>.ColumnAccessors;

            foreach (var item in items)
            {
                var dp = new DynamicParameters();

                foreach (var kvp in accessors)
                {
                    dp.Add(kvp.Key, kvp.Value(item));
                }

                yield return dp;
            }
        }

        /// <summary>
        /// Generates parameters where binding is positional (e.g. @p0, @p1).
        /// </summary>
        private static IEnumerable<DynamicParameters> BuildIndexedParameters(IEnumerable<T> items)
        {
            var accessors = PdoDataCache<T>.ColumnAccessors;
            var columns = accessors.Keys.ToList();

            foreach (var item in items)
            {
                var dp = new DynamicParameters();

                for (int i = 0; i < columns.Count; i++)
                {
                    var columnName = columns[i];
                    var accessor = accessors[columnName];
                    dp.Add($"@p{i}", accessor(item));
                }

                yield return dp;
            }
        }
    }
}
