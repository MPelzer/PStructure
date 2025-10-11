using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

public class ExecutionContext<TRequest> where TRequest : RequestContext
{
    public DbContext DbContext { get; set; }
    public ILogger Logger { get; set; }
    public TRequest RequestContext { get; set; }

    public List<Exception> ValidationErrors { get; } = new();

    public void Validate()
    {
        if (DbContext == null)
            ValidationErrors.Add(new InvalidOperationException("DbContext is required"));
        if (Logger == null)
            ValidationErrors.Add(new InvalidOperationException("Logger is required"));
        if (RequestContext == null)
            ValidationErrors.Add(new InvalidOperationException("RequestContext is required"));

        if (ValidationErrors.Any())
            throw new AggregateException("Validation failed for ExecutionContext", ValidationErrors);
    }
}
