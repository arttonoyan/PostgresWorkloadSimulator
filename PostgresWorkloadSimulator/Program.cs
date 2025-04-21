using PgScalabilityTest.Services;

//var summary = BenchmarkRunner.Run<PostgreSqlBenchmark>();
//var benchmark = new PostgreSqlBenchmark
//{
//    WriterThreadCount = 10000,
//    ReaderThreadCount = 5000
//};
//await benchmark.SetupAsync();
//await benchmark.RunRawEmailEventAsync();
//await benchmark.RunAggregatedEmailEventAsync();

Console.ResetColor();
var testServer = TestServerBuilder.CreateDefaultBuilder().Build();
await testServer.StartAsync();

var options = new SimulatorOptions {
    WriterThreadCount = 100,
    ReaderThreadCount = 50
};
var simulator = new WorkloadSimulator(testServer.ServiceProvider, options);

var metrics = SimulationMetrics.Start();

try
{
    const int writerCount = 1000;
    const int readerCount = 500;
    const int iterations = 10;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Starting simulation with {writerCount} writers and {readerCount} readers for {iterations} iterations...");
    Console.ResetColor();

    for (int i = 0; i < iterations; i++)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Iteration {i + 1}, WriterCount: {writerCount}, ReaderCount: {readerCount}");
        simulator.RunAsync(writerCount: writerCount, readerCount: readerCount, default)
            .GetAwaiter().GetResult();

        metrics.StopStart();

        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(metrics.ToString());
        Console.ResetColor();
    }
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

metrics.Stop();


Console.WriteLine(metrics.ToString());
