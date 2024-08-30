﻿using PStructure.Models;

namespace PStructure.TableLocation
{
    /// <summary>
    /// Kapselt die Informationen, wo sich der zu instantiierende Tabelleneintrag innerhalb der Datenbank befindet.
    /// </summary>
    public class BaseTableLocation : ITableLocation
    {
        private readonly string _schema;
        private readonly string _tableName;

        private BaseTableLocation(string schema, string tableName)
        {
            _schema = schema;
            _tableName = tableName;
        }
        
        /// <summary>
        /// Gibt den Pfad zum Tabelleneintrag aus.
        /// </summary>
        /// <returns></returns>
        public string PrintTableLocation()
        {
            return $"{_schema}.{_tableName}";
        }
    }
}