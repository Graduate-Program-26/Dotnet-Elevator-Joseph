using ElevatorSimulator.Core.Entities;
using FluentAssertions;

namespace ElevatorSimulator.Tests.Core.Entities;

public class BuildingTests
{
    [Fact]
    public void Building_Constructor_ShouldCreateCorrectNumberOfFloorsAndElevators()
    {
        var building = new Building(floorCount: 10, elevatorCount: 3, elevatorCapacity: 5, basementFloors: 2);

        building.GetFloorCount().Should().Be(12);
        building.GetElevatorCount().Should().Be(3);
        building.Elevators.Select(e => e.Capacity).Should().BeEquivalentTo(Enumerable.Repeat(5, 3));
        building.TickDurationMs.Should().Be(500);
        building.Floors.Should().HaveCount(12);
        building.Elevators.Should().HaveCount(3);
    }

    [Fact]
    public void Building_Constructor_ShouldNameElevatorsCorrectly()
    {
        var building = new Building(floorCount: 5, elevatorCount: 2, elevatorCapacity: 5);

        building.Elevators[0].Name.Should().Be("Elevator-1");
        building.Elevators[1].Name.Should().Be("Elevator-2");
    }
}
