using Apache.Ignite;

var cfg = new IgniteClientConfiguration("localhost");
using var client = await IgniteClient.StartAsync(cfg);
