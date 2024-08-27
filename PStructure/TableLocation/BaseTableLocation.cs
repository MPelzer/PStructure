using PStructure.Models;

namespace PStructure
{
    public class BaseTableLocation : ITableLocation
    {
        private readonly string _schema;
        private readonly string _tableName;

        private BaseTableLocation(string schema, string tableName)
        {
            _schema = schema;
            _tableName = tableName;
        }

        public string printTableLocation()
        {
            return $"{_schema}.{_tableName}";
        }
    }
}