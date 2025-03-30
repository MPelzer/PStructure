using System;
using System.Data;
using Dapper;
using PStructure.Interfaces;

public class CustomDateHandler_MM_dd_yyyy : SqlMapper.TypeHandler<DateTime>, ICustomHandler
{
    private const string DateFormat = "MM/dd/yyyy";

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