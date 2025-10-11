using System;
using System.Data;
using Dapper;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.PropertyConverters
{
    public class CustomDateHandler_yyyy_MM_ddTHH_mm_ss : SqlMapper.TypeHandler<DateTime>, ICustomHandler
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";

        object ICustomHandler.Format(object value)
        {
            return ((DateTime)value).ToString(DateFormat);
        }

        object ICustomHandler.Parse(object value)
        {
            return Parse(value);
        }

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value.ToString(DateFormat);
        }

        public override DateTime Parse(object value)
        {
            return DateTime.ParseExact(value.ToString(), DateFormat, null);
        }
    }
}