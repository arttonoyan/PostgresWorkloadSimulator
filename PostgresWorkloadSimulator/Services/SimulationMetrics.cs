using System.Diagnostics;

public class SimulationMetrics
{
    private readonly Stopwatch _stopwatch;
    public DateTimeOffset StartTimestamp { get; }
    public DateTimeOffset EndTimestamp { get; private set; }

    private SimulationMetrics()
    {
        StartTimestamp = DateTimeOffset.UtcNow;
        _stopwatch = Stopwatch.StartNew();
    }

    public static SimulationMetrics Start() => new();

    public void Stop()
    {
        _stopwatch.Stop();
        EndTimestamp = DateTimeOffset.UtcNow;
    }

    public void StopStart()
    {
        Stop();
        _stopwatch.Start();
    }

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public override string ToString()
    {
        return $"""
        Start:   {StartTimestamp:O}
        End:     {EndTimestamp:O}
        Elapsed: {Elapsed:hh\:mm\:ss\.fff}
        """;
    }
}