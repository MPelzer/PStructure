using System;
using System.Data;
using Dapper;
using PStructure.Interfaces;

namespace PStructure.DapperSqlDateTimeMappers
{
    /// <summary>
    /// Implementiert einen <see cref="ICustomHandler"/>, welcher das idiotische Format hat, bei dem das Jahrtausend
    /// durch eine Stelle zu Beginn repräsentiert wird. Mal im Ernst, wie will man Zahlen kleiner 1000 darstellen?
    /// </summary>
    public class CustomDateHandler_cyyMMdd : SqlMapper.TypeHandler<DateTime>, ICustomHandler
    {
        private const string DateFormat = "cyyMMdd";

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            var century = value.Year >= 2000 ? 2 : 1; 
            var formattedDate = $"{century}{value:yyMMdd}";
            parameter.Value = formattedDate;
        }

        public override DateTime Parse(object value)
        {
            var dateStr = value.ToString();
            var century = dateStr[0] == '2' ? 2000 : 1900;
            var yearPart = dateStr.Substring(1, 2);
            var monthDayPart = dateStr.Substring(3);

            var fullDate = $"{century}{yearPart}{monthDayPart}";
            return DateTime.ParseExact(fullDate, "yyyyMMdd", null);
        }

        object ICustomHandler.Format(object value)
        {
            var date = (DateTime)value;
            var century = date.Year >= 2000 ? 2 : 1;
            return $"{century}{date:yyMMdd}";
        }

        object ICustomHandler.Parse(object value)
        {
            return Parse(value);
        }
    }
}