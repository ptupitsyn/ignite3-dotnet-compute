using Apache.Ignite.Compute;

namespace IgniteCompute.Jobs;

public class HelloJob : IComputeJob<string, string>
{
    public ValueTask<string> ExecuteAsync(IJobExecutionContext context, string arg, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"Hello, {arg}!");
    }
}
