using BenchmarkDotNet.Attributes;
using PgScalabilityTest.Services;

namespace PgScalabilityTest;

[MemoryDiagnoser]
[SimpleJob]
public class PostgreSqlBenchmark
{
    private WorkloadSimulator _simulatorAgg = null!;
    private WorkloadSimulator _simulatorRow = null!;

    [Params(10000)]
    public int WriterCount;

    [Params(5000)]
    public int ReaderCount;

    [GlobalSetup]
    public async Task SetupAsync()
    {
        var testServer = TestServerBuilder.CreateDefaultBuilder().Build();
        await testServer.StartAsync();

        // Seed the database with initial data if needed
        _simulatorAgg = new WorkloadSimulator(testServer.ServiceProvider, new SimulatorOptions { TenantStartId = 1 });
        _simulatorRow = new WorkloadRawSimulator(testServer.ServiceProvider, new SimulatorOptions { TenantStartId = 100 });
    }

    [Benchmark(Description = "Aggregated: Writer")]
    public async Task RunWriterAsync()
    {
        await _simulatorAgg.RunWriterAsync(
            writerCount: WriterCount,
            default
        );
    }

    [Benchmark(Description = "Aggregated: Reader")]
    public async Task RunReaderAsync()
    {
        await _simulatorAgg.RunReaderAsync(
            readerCount: ReaderCount,
            default
        );
    }

    [Benchmark(Description = "Raw: Writer")]
    public async Task RunRawWriterAsync()
    {
        await _simulatorRow.RunWriterAsync(
            writerCount: WriterCount,
            default
        );
    }

    [Benchmark(Description = "Raw: Reader")]
    public async Task RunRawReaderAsync()
    {
        await _simulatorRow.RunReaderAsync(
            readerCount: ReaderCount,
            default
        );
    }

    //[Benchmark(Description = "Aggregated")]
    //public async Task RunAggregatedEmailEventAsync()
    //{
    //    await _simulatorAgg.RunAsync(
    //        writerCount: WriterCount,
    //        readerCount: ReaderCount,
    //        default
    //    );
    //}

    //[Benchmark(Description = "Raw")]
    //public async Task RunRawEmailEventAsync()
    //{
    //    await _simulatorRow.RunAsync(
    //        writerCount: WriterCount,
    //        readerCount: ReaderCount,
    //        default
    //    );
    //}
}
