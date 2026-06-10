using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using FluentAssertions;

namespace ElevatorSimulator.Tests.Core.Entities;

public class PassengerTests
{
    [Fact]
    public void Passenger_Constructor_ShouldInferUpDirection_WhenDestinationIsGreater()
    {
        var passenger = new Passenger(1, 5);
        passenger.HallDirection.Should().Be(Direction.Up);
    }

    [Fact]
    public void Passenger_Constructor_ShouldInferDownDirection_WhenDestinationIsLesser()
    {
        var passenger = new Passenger(5, 1);
        passenger.HallDirection.Should().Be(Direction.Down);
    }

    [Fact]
    public void Passenger_Constructor_ShouldUseExplicitDirection_WhenProvided()
    {
        var passenger = new Passenger(5, 1, Direction.Up);
        passenger.HallDirection.Should().Be(Direction.Up);
    }

    [Fact]
    public void Passenger_DefaultState_ShouldBeWaiting()
    {
        var passenger = new Passenger(1, 5);
        passenger.State.Should().Be(PassengerState.Waiting);
    }
}
