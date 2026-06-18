using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class SimulationEngine : ISimulationEngine
{
    private readonly ISimulationClock _clock;
    private readonly IEventBus _eventBus;
    private readonly Building _building;
    private readonly Dispatcher _dispatcher;
    private readonly ElevatorUpdater _elevatorUpdater;
    private readonly RandomPassengerGenerator _passengerGenerator;

    public SimulationState CurrentState { get; private set; } = SimulationState.Stopped;
    public bool IsAutoGenerationEnabled { get; set; } = true;

    public SimulationEngine(Building building, ISimulationClock clock, IEventBus eventBus, Dispatcher dispatcher)
    {
        _building = building ?? throw new ArgumentNullException(nameof(building));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _elevatorUpdater = new ElevatorUpdater(_building, _eventBus);
        _passengerGenerator = new RandomPassengerGenerator(_building, _eventBus);

        _clock.Tick += OnClockTick;
    }

    private void OnClockTick(object? sender, EventArgs e)
    {
        // 1. Generate Passengers (only when auto-generation is enabled)
        if (IsAutoGenerationEnabled)
            _passengerGenerator.Tick();

        // 2. Dispatcher
        var pendingCalls = _building.GetActiveHallCalls().Where(c => c.Status == HallCallStatus.Created).ToList();
        if (pendingCalls.Any())
        {
            _dispatcher.Dispatch(_building.Elevators, pendingCalls);
        }

        // 3. Elevator Updates
        foreach (var elevator in _building.Elevators)
        {
            _elevatorUpdater.Update(elevator);
        }
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
