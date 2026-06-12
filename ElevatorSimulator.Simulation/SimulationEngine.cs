using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class SimulationEngine : ISimulationEngine
{
    private readonly ISimulationClock _clock;
    private readonly IEventBus _eventBus;

    public SimulationState CurrentState { get; private set; } = SimulationState.Stopped;

    public SimulationEngine(ISimulationClock clock, IEventBus eventBus)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        
        _clock.Tick += OnClockTick;
    }

    private void OnClockTick(object? sender, EventArgs e)
    {
        // Todo: 1. Dispatcher 2. Elevator Updates 3. Passenger Updates 4. Event Processing 5. Statistics Updates 6. Render UI
    }

    public void Play()
    {
        if (CurrentState == SimulationState.Playing) return;

        CurrentState = SimulationState.Playing;
        _clock.Start();
        _eventBus.Publish(new SimulationStartedEvent());
    }

    public void Pause()
    {
        if (CurrentState != SimulationState.Playing) return;

        CurrentState = SimulationState.Paused;
        _clock.Stop();
        _eventBus.Publish(new SimulationPausedEvent());
    }

    public void Resume()
    {
        if (CurrentState != SimulationState.Paused) return;

        CurrentState = SimulationState.Playing;
        _clock.Start();
        _eventBus.Publish(new SimulationStartedEvent());
    }

    public void Stop()
    {
        if (CurrentState == SimulationState.Stopped) return;

        CurrentState = SimulationState.Stopped;
        _clock.Stop();
        _eventBus.Publish(new SimulationStoppedEvent());
    }

    public void StepOneTick()
    {
        if (CurrentState == SimulationState.Playing)
        {
            Pause();
        }

        _clock.TickOnce();
    }

    public void IncreaseSpeed()
    {
        _clock.IncreaseSpeed();
    }

    public void DecreaseSpeed()
    {
        _clock.DecreaseSpeed();
    }
}
