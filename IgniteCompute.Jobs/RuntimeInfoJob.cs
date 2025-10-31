using Apache.Ignite.Compute;

namespace IgniteCompute.Jobs;

public class RuntimeInfoJob : IComputeJob<object?, RuntimeInfo>
{
    public ValueTask<RuntimeInfo> ExecuteAsync(IJobExecutionContext context, object? arg, CancellationToken cancellationToken)
    {
        // Collect OS and runtime information here.
        var info = new RuntimeInfo(
            Os: Environment.OSVersion,
            OsArch: System.Runtime.InteropServices.RuntimeInformation.OSArchitecture,
            CpuArh: System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture,
            MachineName: Environment.MachineName,
            FrameworkDescription: System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);

        return ValueTask.FromResult(info);
    }
}
