namespace ElevatorSimulator.Core.Interfaces;

public interface ISimulationClock
{
    TimeSpan TickDuration { get; }
    long TotalTicks { get; }
    
    event EventHandler Tick;

    void Start();
    void Stop();
    void TickOnce();
    
    void IncreaseSpeed();
    void DecreaseSpeed();
    void SetTickDuration(TimeSpan duration);
}
