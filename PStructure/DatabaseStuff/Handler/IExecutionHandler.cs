using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PStructure.DatabaseStuff.Handler;

public interface IExecutionHandler<TRequest, TResult>
    where TRequest : RequestContext
{
    int Execute(ExecutionContext<TRequest> context);
    IEnumerable<TResult> Query(ExecutionContext<TRequest> context);

    Task<int> ExecuteAsync(ExecutionContext<TRequest> context, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> QueryAsync(ExecutionContext<TRequest> context, CancellationToken cancellationToken = default);
}