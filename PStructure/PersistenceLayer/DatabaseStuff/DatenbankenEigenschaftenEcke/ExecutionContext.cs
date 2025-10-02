using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;

public class ExecutionContext : IExecutionContext
{
    public DbContext DbContext { get; set; }
    public RequestContext RequestContext { get; set; } = new();
    public ILogger Logger { get; set; }
    public List<Exception> ValidationExceptions { get; } = new();

    public bool HasErrors => ValidationExceptions.Count > 0;

    public void AddValidationError(string message)
    {
        ValidationExceptions.Add(new InvalidOperationException(message));
    }

    public void AddValidationError(Exception exception)
    {
        ValidationExceptions.Add(exception);
    }

    public void Validate()
    {
        if (DbContext == null)
            AddValidationError("DbContext must not be null.");

        if (Logger == null)
            AddValidationError("Logger must not be null.");

        if (RequestContext == null || string.IsNullOrWhiteSpace(RequestContext.Sql))
            AddValidationError("SqlContext must be set and contain SQL.");

        if (HasErrors)
            throw new AggregateException("Validation failed for DbExecutionContext", ValidationExceptions);
    }
}