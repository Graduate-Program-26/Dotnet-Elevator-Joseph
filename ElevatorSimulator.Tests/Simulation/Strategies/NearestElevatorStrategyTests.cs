using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Simulation.Strategies;

namespace ElevatorSimulator.Tests.Simulation.Strategies;

public class NearestElevatorStrategyTests
{
    [Fact]
    public void AssignElevator_ReturnsIdleElevator()
    {
        var strategy = new NearestElevatorStrategy();
        var hallCall = new HallCall(5, Direction.Up);
        var elevator = new Elevator("E1", 10) { State = ElevatorState.Idle, CurrentFloor = 0 };

        var assigned = strategy.AssignElevator(new[] { elevator }, hallCall);

        Assert.Equal(elevator, assigned);
    }

    [Fact]
    public void AssignElevator_ReturnsMovingElevator_IfSameDirection_AndNotPassed()
    {
        var strategy = new NearestElevatorStrategy();
        var hallCall = new HallCall(5, Direction.Up);
        var elevator = new Elevator("E1", 10) 
        { 
            State = ElevatorState.MovingUp, 
            Direction = Direction.Up, 
            CurrentFloor = 2 
        };

        var assigned = strategy.AssignElevator(new[] { elevator }, hallCall);

        Assert.Equal(elevator, assigned);
    }

    [Fact]
    public void AssignElevator_IgnoresMovingElevator_IfPassedFloor()
    {
        var strategy = new NearestElevatorStrategy();
        var hallCall = new HallCall(5, Direction.Up);
        var elevator = new Elevator("E1", 10) 
        { 
            State = ElevatorState.MovingUp, 
            Direction = Direction.Up, 
            CurrentFloor = 7 
        };

        var assigned = strategy.AssignElevator(new[] { elevator }, hallCall);

        Assert.Null(assigned);
    }

    [Fact]
    public void AssignElevator_IgnoresFullElevator()
    {
        var strategy = new NearestElevatorStrategy();
        var hallCall = new HallCall(5, Direction.Up);
        var elevator = new Elevator("E1", 1) 
        { 
            State = ElevatorState.MovingUp, 
            Direction = Direction.Up, 
            CurrentFloor = 2 
        };
        elevator.Passengers.Add(new Passenger(2, 8)); // Fill elevator

        var assigned = strategy.AssignElevator(new[] { elevator }, hallCall);

        Assert.Null(assigned);
    }

    [Fact]
    public void AssignElevator_SelectsNearestEligibleElevator()
    {
        var strategy = new NearestElevatorStrategy();
        var hallCall = new HallCall(5, Direction.Up);
        
        var e1 = new Elevator("E1", 10) { State = ElevatorState.Idle, CurrentFloor= 0 };
        var e2 = new Elevator("E2", 10) { State = ElevatorState.Idle, CurrentFloor = 4 };
        var e3 = new Elevator("E3", 10) { State = ElevatorState.MovingUp, Direction = Direction.Up, CurrentFloor = 7 };

        var assigned = strategy.AssignElevator(new[] { e1, e2, e3 }, hallCall);

        Assert.Equal(e2, assigned);
    }
}
