using System;
using System.Collections.Generic;

public class ItemRequestContext<T> : RequestContext
{
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public Func<IEnumerable<T>, (string Sql, object Parameters)> StatementBuilder { get; set; }
}