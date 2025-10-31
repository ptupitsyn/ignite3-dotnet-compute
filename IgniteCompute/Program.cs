using Apache.Ignite;
using Apache.Ignite.Compute;
using Apache.Ignite.Network;
using IgniteCompute;
using IgniteCompute.Jobs;

var cfg = new IgniteClientConfiguration("localhost");
using var client = await IgniteClient.StartAsync(cfg);

IList<IClusterNode> nodes = await client.GetClusterNodesAsync();
var anyNodeTarget = JobTarget.AnyNode(nodes);
Console.WriteLine($"Nodes: {string.Join(", ", nodes)}");

DeploymentUnit deploymentUnit = await ManagementApi.DeployAssembly<HelloJob>("IgniteComputeDemo");
Console.WriteLine($"Unit deployed: {deploymentUnit}");

// Simple Hello job.
var helloJobDesc = JobDescriptor.Of(new HelloJob()) with { DeploymentUnits = [deploymentUnit] };
IJobExecution<string> jobExec = await client.Compute.SubmitAsync(anyNodeTarget, helloJobDesc, "Ignite");
Console.WriteLine("Hello job result: " + await jobExec.GetResultAsync());

// Collect runtime info from all nodes with a broadcast job.
var runtimeInfoJobDesc = JobDescriptor.Of(new RuntimeInfoJob()) with { DeploymentUnits = [deploymentUnit] };
var broadcastExec = await client.Compute.SubmitBroadcastAsync(BroadcastJobTarget.Nodes(nodes), runtimeInfoJobDesc, arg: null);

foreach (var exec in broadcastExec.JobExecutions)
{
    RuntimeInfo info = await exec.GetResultAsync();
    Console.WriteLine($"Node: {exec.Node}, Runtime info: {info}");
}

// Jobs can use Ignite API to work with tables, call other jobs, etc.
// TODO

// Job contexts are cached and statics are preserved between job executions on the same node.
var counterJobDesc = JobDescriptor.Of(new CounterJob()) with { DeploymentUnits = [deploymentUnit] };
var singleNodeTarget = JobTarget.Node(nodes.First());

for (int i = 0; i < 5; i++)
{
    var counterExec = await client.Compute.SubmitAsync(singleNodeTarget, counterJobDesc, arg: null);
    int invokeCount = await counterExec.GetResultAsync();

    Console.WriteLine($"Counter job invocation {i + 1}, result: {invokeCount}");
}
