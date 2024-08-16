using System;
using System.Data;
using Dapper;
using PStructure.Interfaces;

public class CustomDateHandler_cyyMMdd : SqlMapper.TypeHandler<DateTime>, ICustomHandler
{
    private const string DateFormat = "cyyMMdd";

    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        int century = value.Year >= 2000 ? 2 : 1; // Assuming 2 for 2000s, 1 for 1900s
        string formattedDate = $"{century}{value:yyMMdd}";
        parameter.Value = formattedDate;
    }

    public override DateTime Parse(object value)
    {
        string dateStr = value.ToString();
        int century = dateStr[0] == '2' ? 2000 : 1900;
        string yearPart = dateStr.Substring(1, 2);
        string monthDayPart = dateStr.Substring(3);

        string fullDate = $"{century}{yearPart}{monthDayPart}";
        return DateTime.ParseExact(fullDate, "yyyyMMdd", null);
    }

    object ICustomHandler.Format(object value)
    {
        DateTime date = (DateTime)value;
        int century = date.Year >= 2000 ? 2 : 1;
        return $"{century}{date:yyMMdd}";
    }

    object ICustomHandler.Parse(object value)
    {
        return Parse(value);
    }
}