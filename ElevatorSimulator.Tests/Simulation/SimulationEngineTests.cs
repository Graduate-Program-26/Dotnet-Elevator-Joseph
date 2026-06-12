using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;
using ElevatorSimulator.Infrastructure.Services;
using ElevatorSimulator.Simulation;

namespace ElevatorSimulator.Tests.Simulation;

public class SimulationEngineTests
{
    [Fact]
    public void Play_StartsClock_AndChangesState_AndPublishesEvent()
    {
        // Arrange
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));
        var eventBus = new InMemoryEventBus();
        var engine = new SimulationEngine(clock, eventBus);
        
        var startedEventReceived = false;
        eventBus.Subscribe<SimulationStartedEvent>(e => startedEventReceived = true);

        // Act
        engine.Play();

        // Assert
        Assert.Equal(SimulationState.Playing, engine.CurrentState);
        Assert.True(startedEventReceived);
        
        engine.Stop(); // Cleanup
    }

    [Fact]
    public void Pause_StopsClock_AndChangesState_AndPublishesEvent()
    {
        // Arrange
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));
        var eventBus = new InMemoryEventBus();
        var engine = new SimulationEngine(clock, eventBus);
        
        engine.Play();
        
        var pausedEventReceived = false;
        eventBus.Subscribe<SimulationPausedEvent>(e => pausedEventReceived = true);

        // Act
        engine.Pause();

        // Assert
        Assert.Equal(SimulationState.Paused, engine.CurrentState);
        Assert.True(pausedEventReceived);
        
        engine.Stop(); // Cleanup
    }

    [Fact]
    public void Stop_StopsClock_AndChangesState_AndPublishesEvent()
    {
        // Arrange
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));
        var eventBus = new InMemoryEventBus();
        var engine = new SimulationEngine(clock, eventBus);
        
        engine.Play();
        
        var stoppedEventReceived = false;
        eventBus.Subscribe<SimulationStoppedEvent>(e => stoppedEventReceived = true);

        // Act
        engine.Stop();

        // Assert
        Assert.Equal(SimulationState.Stopped, engine.CurrentState);
        Assert.True(stoppedEventReceived);
    }

    [Fact]
    public void StepOneTick_CallsClockTickOnce_AndPausesIfPlaying()
    {
        // Arrange
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));
        var eventBus = new InMemoryEventBus();
        var engine = new SimulationEngine(clock, eventBus);
        
        engine.Play();
        
        // Act
        engine.StepOneTick();

        // Assert
        Assert.Equal(SimulationState.Paused, engine.CurrentState);
        Assert.Equal(1, clock.TotalTicks);
        
        engine.Stop(); // Cleanup
    }
}
