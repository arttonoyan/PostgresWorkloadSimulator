using BenchmarkDotNet.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PgScalabilityTest.Data;

namespace PgScalabilityTest.Services;

//public class SimulatedWorkloadService
//{
//    private readonly SemaphoreSlim _semaphoreWrite;
//    private readonly SemaphoreSlim _semaphoreRead;
//    private readonly int[] _tenantIds;
//    private readonly IServiceProvider _serviceProvider;

//    private int _tenantId => _tenantIds[Random.Shared.Next(_tenantIds.Length)];

//    public SimulatedWorkloadService(IServiceProvider serviceProvider, int tenantStartId)
//    {
//        _tenantIds = [.. Enumerable.Range(tenantStartId, 10)];
//        _semaphoreWrite = new SemaphoreSlim(12); // allow up to 12 concurrent writers
//        _semaphoreRead = new SemaphoreSlim(4);   // allow up to 4 concurrent readers
//        _serviceProvider = serviceProvider;
//    }

//    public async Task RunAggregatedEmailEventAsync(int writerCount, int readerCount, CancellationToken token)
//    {
//        var writers = Enumerable.Range(0, writerCount)
//            .Select(_ => Task.Run(() => SimulateWriteAsync(token)));

//        var readers = Enumerable.Range(0, readerCount)
//            .Select(_ => Task.Run(() => SimulateReadAsync(token)));

//        await Task.WhenAll(writers.Concat(readers));
//    }

//    public async Task RunRawEmailEventAsync(int writerCount, int readerCount, CancellationToken token)
//    {
//        var writers = Enumerable.Range(0, writerCount)
//            .Select(_ => Task.Run(() => SimulateWriteRawEmailEventAsync(token)));

//        var readers = Enumerable.Range(0, readerCount)
//            .Select(_ => Task.Run(() => SimulateReadRawEmailEventAsyncAsync(token)));

//        await Task.WhenAll(writers.Concat(readers));
//    }

//    private async Task SimulateWriteAsync(CancellationToken token)
//    {
//        await _semaphoreWrite.WaitAsync(token);
//        using var scope = _serviceProvider.CreateScope();

//        try
//        {
//            //Console.WriteLine($"WriteAsync: [{DateTime.UtcNow:HH:mm:ss}] Writing...");
//            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();

//            var evt = new EmailEvent
//            {
//                TenantId = _tenantId,
//                Timestamp = DateTime.UtcNow,
//                Value = Random.Shared.Next(1, 100)
//            };

//            await eventService.AddAsync(evt, token);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"WriteAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
//        }
//        finally
//        {
//            _semaphoreWrite.Release();
//        }
//        //await Task.Delay(10, token);
//    }

//    private async Task SimulateReadAsync(CancellationToken token)
//    {
//        await _semaphoreRead.WaitAsync(token);

//        using var scope = _serviceProvider.CreateScope();

//        try
//        {
//            //Console.WriteLine($"ReadAsync: [{DateTime.UtcNow:HH:mm:ss}] Reading...");
//            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();

//            var now = DateTime.UtcNow;
//            var total = await eventService.CalculateAsync(_tenantId, now.AddSeconds(Random.Shared.Next(-3, -1)), now, token);
//            //Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Calculated Total (1m): {total}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ReadAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
//        }
//        finally
//        {
//            _semaphoreRead.Release();
//        }
//        //await Task.Delay(10, token);
//    }

//    private async Task SimulateWriteRawEmailEventAsync(CancellationToken token)
//    {
//        await _semaphoreWrite.WaitAsync(token);
//        using var scope = _serviceProvider.CreateScope();

//        try
//        {
//            //Console.WriteLine($"WriteRawEmailEventAsync: [{DateTime.UtcNow:HH:mm:ss}] Writing Raw Email Event...");
//            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();

//            var evt = new RawEmailEvent
//            {
//                TenantId = _tenantId,
//                Timestamp = DateTime.UtcNow,
//                Value = Random.Shared.Next(1, 100)
//            };

//            await eventService.AddAsync(evt, token);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"WriteRawEmailEventAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
//        }
//        finally
//        {
//            _semaphoreWrite.Release();
//        }
//        //await Task.Delay(10, token);
//    }

//    private async Task SimulateReadRawEmailEventAsyncAsync(CancellationToken token)
//    {
//        await _semaphoreRead.WaitAsync(token);

//        using var scope = _serviceProvider.CreateScope();

//        try
//        {
//            //Console.WriteLine($"ReadRawEmailEventAsync: [{DateTime.UtcNow:HH:mm:ss}] Reading Raw Email Event...");
//            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();

//            var now = DateTime.UtcNow;
//            var total = await eventService.CalculateFromRawAsync(_tenantId, now.AddSeconds(Random.Shared.Next(-3, -1)), now, token);
//            //Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Calculated Total (1m): {total}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ReadRawEmailEventAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
//        }
//        finally
//        {
//            _semaphoreRead.Release();
//        }
//        //await Task.Delay(100, token);
//    }
//}

public class WorkloadRawSimulator : WorkloadSimulator
{
    public WorkloadRawSimulator(IServiceProvider serviceProvider, SimulatorOptions options)
        : base(serviceProvider, options)
    { }

    protected override async Task AddAsync(EmailEventService emailEventService, CancellationToken token)
    {
        var evt = new RawEmailEvent
        {
            TenantId = _tenantId,
            Timestamp = DateTime.UtcNow,
            Value = Random.Shared.Next(1, 100)
        };

        await emailEventService.AddAsync(evt, token);
    }

    protected override async Task<int?> CalculateAsync(EmailEventService emailEventService, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        return await emailEventService.CalculateFromRawAsync(_tenantId, now.AddSeconds(Random.Shared.Next(-3, -1)), now, token);
    }
}

public class SimulatorOptions
{
    public int WriterThreadCount { get; set; } = 10;
    public int ReaderThreadCount { get; set; } = 5;
    public int TenantStartId { get; set; } = 1;
}

public class WorkloadSimulator
{
    private readonly SemaphoreSlim _semaphoreWrite;
    private readonly SemaphoreSlim _semaphoreRead;
    private readonly int[] _tenantIds;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WorkloadSimulator> _logger;

    protected int _tenantId
    {
        get
        {
            try
            {
                return _tenantIds[Random.Shared.Next(_tenantIds.Length)];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tenant ID: {ex.Message}");
                return 1;
            }
        }
    }

    public WorkloadSimulator(IServiceProvider serviceProvider, SimulatorOptions options)
    {
        _tenantIds = [.. Enumerable.Range(options.TenantStartId, 10)];
        _semaphoreWrite = new SemaphoreSlim(options.WriterThreadCount); // allow up to 12 concurrent writers
        _semaphoreRead = new SemaphoreSlim(options.ReaderThreadCount);   // allow up to 4 concurrent readers
        _serviceProvider = serviceProvider;
        _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<WorkloadSimulator>();
    }

    public Task RunAsync(int writerCount, int readerCount, CancellationToken token)
    {
        var writers = Enumerable.Range(0, writerCount)
            .Select(_ => Task.Run(() => SimulateWriteAsync(token)));

        var readers = Enumerable.Range(0, readerCount)
            .Select(_ => Task.Run(() => SimulateReadAsync(token)));

        return Task.WhenAll(writers.Concat(readers));
    }

    public async Task RunWriterAsync(int writerCount, CancellationToken token)
    {
        var writers = Enumerable.Range(0, writerCount)
            .Select(_ => Task.Run(() => SimulateWriteAsync(token)));

        await Task.WhenAll(writers);
    }

    public async Task RunReaderAsync(int readerCount, CancellationToken token)
    {
        var readers = Enumerable.Range(0, readerCount)
            .Select(_ => Task.Run(() => SimulateReadAsync(token)));

        await Task.WhenAll(readers);
    }

    private async Task SimulateWriteAsync(CancellationToken token)
    {
        await _semaphoreWrite.WaitAsync(token);
        using var scope = _serviceProvider.CreateScope();

        try
        {
            //Console.WriteLine($"WriteAsync: [{DateTime.UtcNow:HH:mm:ss}] Writing...");
            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();
            await AddAsync(eventService, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"WriteAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
        }
        finally
        {
            _semaphoreWrite.Release();
        }
    }

    private async Task SimulateReadAsync(CancellationToken token)
    {
        await _semaphoreRead.WaitAsync(token);

        using var scope = _serviceProvider.CreateScope();

        try
        {
            //Console.WriteLine($"ReadAsync: [{DateTime.UtcNow:HH:mm:ss}] Reading...");
            var eventService = scope.ServiceProvider.GetRequiredService<EmailEventService>();
            var total = await CalculateAsync(eventService, token);
            //Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Calculated Total (1m): {total}");
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"ReadAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
            _logger.LogError(ex, $"ReadAsync: [{DateTime.UtcNow:HH:mm:ss}] Exception: {ex.Message}");
        }
        finally
        {
            _semaphoreRead.Release();
        }
    }

    protected virtual async Task AddAsync(EmailEventService emailEventService, CancellationToken token)
    {
        var evt = new EmailEvent
        {
            TenantId = _tenantId,
            Timestamp = DateTime.UtcNow,
            Value = Random.Shared.Next(1, 100)
        };

        await emailEventService.AddAsync(evt, token);
    }

    protected virtual async Task<int?> CalculateAsync(EmailEventService emailEventService, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var total = await emailEventService.CalculateAsync(_tenantId, now.AddSeconds(Random.Shared.Next(-3, -1)), now, token);
        return total;
    }
}

