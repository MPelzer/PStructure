using System.Collections.Generic;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

public interface IParameterStrategy<T>
{
    object GenerateSingle(T item, SqlContext context);
    IEnumerable<object> GenerateBatch(IEnumerable<T> items, SqlContext context);
}