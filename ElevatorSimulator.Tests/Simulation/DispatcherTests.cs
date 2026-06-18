using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Infrastructure.Services;
using ElevatorSimulator.Simulation;
using ElevatorSimulator.Simulation.Strategies;

namespace ElevatorSimulator.Tests.Simulation;

public class DispatcherTests
{
    [Fact]
    public void Dispatch_AssignsHallCall_AndPublishesEvent()
    {
        // Arrange
        var strategy = new NearestElevatorStrategy();
        var eventBus = new InMemoryEventBus();
        var dispatcher = new Dispatcher(strategy, eventBus);

        var elevator = new Elevator("E1", 10) { State = ElevatorState.Idle, CurrentFloor = 0 };
        var hallCall = new HallCall(5, Direction.Up);
        
        var eventReceived = false;
        eventBus.Subscribe<HallCallAssignedEvent>(e => 
        {
            if (e.HallCall == hallCall && e.ElevatorId == elevator.Id)
                eventReceived = true;
        });

        // Act
        dispatcher.Dispatch(new[] { elevator }, new[] { hallCall });

        // Assert
        Assert.Equal(HallCallStatus.Assigned, hallCall.Status);
        Assert.Equal(elevator.Id, hallCall.AssignedElevatorId);
        Assert.Contains(5, elevator.UpStops);
        Assert.True(eventReceived);
    }

    [Fact]
    public void Dispatch_IgnoresAlreadyAssignedCalls()
    {
        // Arrange
        var strategy = new NearestElevatorStrategy();
        var eventBus = new InMemoryEventBus();
        var dispatcher = new Dispatcher(strategy, eventBus);

        var elevator = new Elevator("E1", 10) { State = ElevatorState.Idle, CurrentFloor = 0 };
        var hallCall = new HallCall(5, Direction.Up) { Status = HallCallStatus.Assigned };

        // Act
        dispatcher.Dispatch(new[] { elevator }, new[] { hallCall });

        // Assert
        Assert.DoesNotContain(5, elevator.UpStops);
    }
}
