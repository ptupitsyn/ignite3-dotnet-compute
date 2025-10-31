using Apache.Ignite;
using Apache.Ignite.Compute;
using Apache.Ignite.Network;
using BenchmarkDotNet.Attributes;
using IgniteCompute.Jobs;

namespace IgniteCompute.Benchmarks;

public class JobBenchmark
{
    private JobDescriptor<object?, int> _counterJobDescriptor = null!;
    private IIgniteClient _client = null!;
    private IJobTarget<IClusterNode> _target = null!;

    [GlobalSetup]
    public async Task DeployJobs()
    {
        var unit = await ManagementApi.DeployAssembly<CounterJob>("JobBenchmark");
        _counterJobDescriptor = JobDescriptor.Of(new CounterJob()) with { DeploymentUnits = [unit] };

        var cfg = new IgniteClientConfiguration("localhost");
        _client = await IgniteClient.StartAsync(cfg);

        _target = JobTarget.Node(_client.GetConnections().First().Node);
    }

    [GlobalCleanup]
    public async Task UndeployJobs() => await ManagementApi.UnitUndeploy(_counterJobDescriptor.DeploymentUnits!.Single());

    [Benchmark]
    public async Task ExecuteCounterJob()
    {
        var counterExec = await _client.Compute.SubmitAsync(_target, _counterJobDescriptor, arg: null);
        await counterExec.GetResultAsync();
    }
}
