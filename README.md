# PostgresWorkloadSimulator

A .NET-based simulation tool for benchmarking PostgreSQL under concurrent transactional workloads.  
It supports configurable writer/reader concurrency, simulates transactional inserts and aggregate updates, and evaluates read consistency in parallel.

---

## ✨ Features

- Multi-tenant simulation with customizable tenant range
- Parallel write/read execution using thread semaphores
- Timestamp-based event aggregation (10-second buckets)
- Performance metrics capture (start, end, elapsed)
- Designed for stress-testing transactional behavior

---

## 🛠 Configuration

Update simulation parameters via `SimulatorOptions`:

```csharp
var options = new SimulatorOptions
{
    WriterThreadCount = 1000,
    ReaderThreadCount = 500,
    TenantStartId = 1
};
```

---

## ▶️ Usage

```csharp
var simulator = new WorkloadSimulator(serviceProvider, options);
await simulator.RunAsync(writerCount: 1000, readerCount: 500, cancellationToken);
```

Use iteration loop to repeat the workload and measure timings.

---

## 📈 Metrics Output (Sample)

```
Iteration 1
Start:   2025-04-21T04:22:02.321Z
End:     2025-04-21T04:22:04.593Z
Elapsed: 00:00:02.273
...
```

---

## 📦 Dependencies

- [.NET 8](https://learn.microsoft.com/en-us/shows/dotnetconf-2023/welcome-to-dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) and [Postgres Provider](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL)
- [PostgreSQL](https://www.postgresql.org/)
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)
- [Testcontainers PostgreSQL Module](https://dotnet.testcontainers.org/modules/postgres/)

---

## 📌 Notes

- Aggregated data is stored per tenant in fixed 10-second intervals.
- Consider expanding tenant range to reduce row lock contention.
- Add instrumentation for DB diagnostics (e.g., `pg_stat_activity`) for deeper profiling.
