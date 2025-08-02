using System.Collections.Generic;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

public interface ICustomSqlParameterStrategy
{
    object GenerateFromRawValues(object[] values, SqlContext context);
    object GenerateFromDictionary(IDictionary<string, object> values, SqlContext context);
}