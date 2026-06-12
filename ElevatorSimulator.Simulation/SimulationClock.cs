using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class SimulationClock : ISimulationClock, IDisposable
{
    public TimeSpan TickDuration { get; private set; }
    public long TotalTicks { get; private set; }

    public event EventHandler? Tick;

    private CancellationTokenSource? _cts;
    private Task? _timerTask;

    public SimulationClock(TimeSpan initialTickDuration)
    {
        TickDuration = initialTickDuration;
    }

    public void Start()
    {
        if (_cts != null) return; // Already running

        _cts = new CancellationTokenSource();
        _timerTask = RunTimerAsync(_cts.Token);
    }

    public void Stop()
    {
        if (_cts == null) return; // Not running

        _cts.Cancel();
        try
        {
            _timerTask?.Wait();
        }
        catch (AggregateException e) when (e.InnerException is TaskCanceledException)
        {
            // Expected
        }

        _cts.Dispose();
        _cts = null;
        _timerTask = null;
    }

    public void TickOnce()
    {
        TotalTicks++;
        Tick?.Invoke(this, EventArgs.Empty);
    }

    public void IncreaseSpeed()
    {
        var newMs = Math.Max(10, TickDuration.TotalMilliseconds / 2);
        SetTickDuration(TimeSpan.FromMilliseconds(newMs));
    }

    public void DecreaseSpeed()
    {
        var newMs = Math.Min(5000, TickDuration.TotalMilliseconds * 2);
        SetTickDuration(TimeSpan.FromMilliseconds(newMs));
    }

    public void SetTickDuration(TimeSpan duration)
    {
        TickDuration = duration;
    }

    private async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TickDuration, token);
                if (!token.IsCancellationRequested)
                {
                    TickOnce();
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when cancelling the task
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
