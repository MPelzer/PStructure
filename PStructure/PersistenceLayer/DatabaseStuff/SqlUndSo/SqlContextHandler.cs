using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke.PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;
using PStructure.PersistenceLayer.Pdo.PdoData;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    public static class SqlContextHandler
    {
        private static readonly Dictionary<string, List<DynamicParameters>> _batchCache = new();
        private static readonly Dictionary<string, DynamicParameters> _singleCache = new();

        public static object Generate(object input, SqlContext context)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return context.ExecutionMode switch
            {
                SqlExecutionMode.AutoFromItem =>
                    UseGenericTypedHelper(input, context),

                SqlExecutionMode.ManualFromSqlAndObjectArray =>
                    GenerateIndexedParametersFromValues((object[])input),

                SqlExecutionMode.ManualFromSql =>
                    GenerateFromDictionary((IDictionary<string, object>)input),

                _ => throw new NotSupportedException($"Unknown execution mode: {context.ExecutionMode}")
            };
        }

        public static IEnumerable<object> GenerateBatch(IEnumerable<object> inputs, SqlContext context)
        {
            if (inputs == null)
                throw new ArgumentNullException(nameof(inputs));

            if (context.ExecutionMode == SqlExecutionMode.AutoFromItem)
            {
                if (_batchCache.TryGetValue(context.Sql, out var cached))
                    return cached;

                var list = new List<DynamicParameters>();
                foreach (var input in inputs)
                {
                    list.Add((DynamicParameters)UseGenericTypedHelper(input, context));
                }

                _batchCache[context.Sql] = list;
                return list;
            }

            throw new NotSupportedException("Batch generation is only supported for AutoFromItem mode.");
        }

        private static object UseGenericTypedHelper(object input, SqlContext context)
        {
            var method = typeof(SqlContextHandler)
                .GetMethod(nameof(GenerateForTyped), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(input.GetType());

            if (method == null)
                throw new InvalidOperationException("Generic method resolution failed.");

            return method.Invoke(null, new[] { input, context });
        }

        private static DynamicParameters GenerateForTyped<T>(T item, SqlContext context)
        {
            return context.ParameterizingType switch
            {
                SqlParameterizingType.Named => GenerateNamedParameters(item),
                SqlParameterizingType.Indexed => GenerateIndexedParametersFromObject(item, context.Sql),
                _ => throw new NotSupportedException($"Unsupported parameterizing type: {context.ParameterizingType}")
            };
        }

        private static DynamicParameters GenerateNamedParameters<T>(T item)
        {
            var key = typeof(T).FullName;

            if (_singleCache.TryGetValue(key, out var cached))
                return cached;

            var dyn = new DynamicParameters();
            AddNamedParameters(item, dyn);
            _singleCache[key] = dyn;

            return dyn;
        }

        private static void AddNamedParameters<T>(T item, DynamicParameters parameters)
        {
            var accessors = PdoDataCache<T>.ColumnAccessors;

            foreach (var kvp in accessors)
            {
                var columnName = kvp.Key;
                var accessor = kvp.Value;
                var value = accessor(item);
                parameters.Add("@" + columnName, value);
            }
        }

        private static DynamicParameters GenerateIndexedParametersFromObject<T>(T item, string sql)
        {
            var properties = PdoDataCache<T>.Properties;
            var placeholderCount = Regex.Matches(sql ?? "", @"\?").Count;

            if (placeholderCount != properties.Length)
                throw new InvalidOperationException("Mismatch between SQL placeholders and object properties.");

            var parameters = new DynamicParameters();
            for (int i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item);
                parameters.Add($"@p{i}", value);
            }

            return parameters;
        }

        public static DynamicParameters GenerateIndexedParametersFromValues(object[] values)
        {
            var parameters = new DynamicParameters();
            for (int i = 0; i < values.Length; i++)
            {
                parameters.Add($"@p{i}", values[i]);
            }
            return parameters;
        }

        public static DynamicParameters GenerateFromDictionary(IDictionary<string, object> values, SqlContext context = null)
        {
            var parameters = new DynamicParameters();
            foreach (var kvp in values)
            {
                parameters.Add("@" + kvp.Key, kvp.Value);
            }
            return parameters;
        }

        public static void ClearCaches()
        {
            _batchCache.Clear();
            _singleCache.Clear();
        }
    }
}
