using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using FluentAssertions;

namespace ElevatorSimulator.Tests.Core.Entities;

public class ElevatorTests
{
    [Fact]
    public void Elevator_ShouldInitialize_WithCorrectDefaults()
    {
        var elevator = new Elevator("Test-Elevator", 10);
        
        elevator.Name.Should().Be("Test-Elevator");
        elevator.Capacity.Should().Be(10);
        elevator.CurrentFloor.Should().Be(1);
        elevator.Direction.Should().Be(Direction.Idle);
        elevator.State.Should().Be(ElevatorState.Idle);
        elevator.Occupancy.Should().Be(0);
        elevator.IsFull.Should().BeFalse();
    }

    [Fact]
    public void Elevator_AddStop_ShouldAddUpStop_WhenDirectionIsUp()
    {
        var elevator = new Elevator("E1", 10);
        elevator.AddStop(5, Direction.Up);

        elevator.UpStops.Should().Contain(5);
        elevator.DownStops.Should().BeEmpty();
    }

    [Fact]
    public void Elevator_AddStop_ShouldAddDownStop_WhenDirectionIsDown()
    {
        var elevator = new Elevator("E1", 10);
        elevator.AddStop(2, Direction.Down);

        elevator.DownStops.Should().Contain(2);
        elevator.UpStops.Should().BeEmpty();
    }

    [Fact]
    public void Elevator_RemoveStop_ShouldRemoveCorrectStop()
    {
        var elevator = new Elevator("E1", 10);
        elevator.AddStop(5, Direction.Up);
        elevator.AddStop(2, Direction.Down);

        elevator.RemoveStop(5, Direction.Up);

        elevator.UpStops.Should().BeEmpty();
        elevator.DownStops.Should().Contain(2);
    }
}
