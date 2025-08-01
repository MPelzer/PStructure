using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;

namespace PStructure.PersistenceLayer.DatabaseStuff
{
    /// <summary>
    /// Encapsulates a database request with SQL and dynamic parameter generation.
    /// </summary>
    /// <typeparam name="T">The PDO model type.</typeparam>
    public class DbRequest<T> : IDbRequest<T>
    {
        private readonly string _sqlTemplate;
        private readonly bool _isPositional;

        /// <summary>
        /// Initializes a new DbRequest with SQL. Automatically detects if it uses positional parameters (?).
        /// </summary>
        public DbRequest(string sqlTemplate)
        {
            _sqlTemplate = sqlTemplate;
            _isPositional = sqlTemplate.Contains("?");
        }

        public string GetSql()
        {
            return _sqlTemplate;
        }

        
        public DynamicParameters GetParameters(T item)
        {
            var parameters = new DynamicParameters();
            var properties = PdoDataCache<T>.Properties;

            if (_isPositional)
            {
                // Match number of "?" placeholders in SQL
                var placeholders = Regex.Matches(_sqlTemplate, @"\?").Count;
                if (placeholders != properties.Length)
                {
                    throw new InvalidOperationException(
                        $"Mismatch between SQL placeholders ({placeholders}) and properties ({properties.Length}) for type {typeof(T).Name}.");
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item);
                    parameters.Add($"@p{i}", value);
                }
            }
            else
            {
                // Named parameters based on [Column] attribute
                foreach (var prop in properties)
                {
                    var columnAttr = prop.GetCustomAttribute<PdoPropertyAttributes.Column>();
                    if (columnAttr == null)
                        continue;

                    var columnName = columnAttr.ColumnName;
                    var value = prop.GetValue(item);
                    parameters.Add($"@{columnName}", value);
                }
            }

            return parameters;
        }
    }
}
