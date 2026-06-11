using ElevatorSimulator.Core.Enums;
using FluentAssertions;

namespace ElevatorSimulator.Tests.Core.Enums;

public class DirectionTests
{
    [Fact]
    public void Direction_Should_Have_Up()
    {
        Enum.IsDefined(Direction.Up)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Direction_Should_Have_Down()
    {
        Enum.IsDefined(Direction.Down)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Direction_Should_Have_Idle()
    {
        Enum.IsDefined(Direction.Idle)
            .Should()
            .BeTrue();
    }
}