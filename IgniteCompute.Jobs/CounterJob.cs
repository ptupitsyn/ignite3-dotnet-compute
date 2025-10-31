using Apache.Ignite.Compute;

namespace IgniteCompute.Jobs;

public class CounterJob : IComputeJob<object?, int>
{
    private static int _counter;

    public ValueTask<int> ExecuteAsync(IJobExecutionContext context, object? arg, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Interlocked.Increment(ref _counter));
    }
}
