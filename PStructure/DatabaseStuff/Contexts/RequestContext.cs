using System;
using System.Collections.Generic;

public abstract class RequestContext
{
    /// <summary>
    /// Optional raw SQL statement (used mainly by direct SQL contexts).
    /// </summary>
    public string? Sql { get; set; }

    /// <summary>
    /// Optional function for building parameter objects from items (used mainly by dynamic contexts).
    /// </summary>
    public Func<IEnumerable<object>, object>? ParameterFactory { get; set; }
}