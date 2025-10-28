using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

public class ExecutionContext<TRequest> where TRequest : RequestContext
{
    
    public DbContext DbContext { get; set; }
    public ILogger Logger { get; set; }
    public TRequest RequestContext { get; set; }

    private List<Exception> Exceptions { get; } = new();


    public void AddException(Exception exceptions)
    {
        Exceptions.Add(exceptions);
    }
}
