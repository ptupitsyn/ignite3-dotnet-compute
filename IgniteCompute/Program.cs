using Apache.Ignite;
using Apache.Ignite.Compute;
using Apache.Ignite.Network;
using IgniteCompute;
using IgniteCompute.Jobs;

var cfg = new IgniteClientConfiguration("localhost");
using var client = await IgniteClient.StartAsync(cfg);

IList<IClusterNode> nodes = await client.GetClusterNodesAsync();
Console.WriteLine($"Nodes: {string.Join(", ", nodes)}");

DeploymentUnit deploymentUnit = await ManagementApi.DeployExecutingAssembly("IgniteComputeDemo");
Console.WriteLine($"Unit deployed: {deploymentUnit}");

var anyNodeTarget = JobTarget.AnyNode(nodes);
var helloJobDesc = JobDescriptor.Of(new HelloJob());

IJobExecution<string> jobExec = await client.Compute.SubmitAsync(anyNodeTarget, helloJobDesc, "Ignite");
Console.WriteLine("Job result: " + await jobExec.GetResultAsync());
