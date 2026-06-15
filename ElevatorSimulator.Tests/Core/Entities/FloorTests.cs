using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Tests.Core.Entities;

public class FloorTests
{
    [Fact]
    public void AddCall_SetsUpCall_WhenDirectionIsUp()
    {
        var floor = new Floor(5);
        var call = new HallCall(5, Direction.Up);

        floor.AddCall(call);

        Assert.Equal(call, floor.UpCall);
        Assert.Null(floor.DownCall);
        Assert.Contains(call, floor.GetActiveCalls());
    }

    [Fact]
    public void AddCall_SetsDownCall_WhenDirectionIsDown()
    {
        var floor = new Floor(5);
        var call = new HallCall(5, Direction.Down);

        floor.AddCall(call);

        Assert.Equal(call, floor.DownCall);
        Assert.Null(floor.UpCall);
        Assert.Contains(call, floor.GetActiveCalls());
    }

    [Fact]
    public void AddCall_ThrowsException_IfFloorLevelMismatches()
    {
        var floor = new Floor(5);
        var call = new HallCall(3, Direction.Up);

        Assert.Throws<ArgumentException>(() => floor.AddCall(call));
    }

    [Fact]
    public void ClearCall_RemovesCallForDirection()
    {
        var floor = new Floor(5);
        floor.AddCall(new HallCall(5, Direction.Up));
        floor.AddCall(new HallCall(5, Direction.Down));

        floor.ClearCall(Direction.Up);

        Assert.Null(floor.UpCall);
        Assert.NotNull(floor.DownCall);
        Assert.Single(floor.GetActiveCalls());
    }
}
