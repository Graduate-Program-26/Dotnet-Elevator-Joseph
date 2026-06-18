using ElevatorSimulator.Simulation;

namespace ElevatorSimulator.Tests.Simulation;

public class SimulationClockTests
{
    [Fact]
    public void TickOnce_IncrementsTotalTicks_AndFiresEvent()
    {
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));
        var eventFiredCount = 0;
        clock.Tick += (sender, e) => eventFiredCount++;

        clock.TickOnce();

        Assert.Equal(1, clock.TotalTicks);
        Assert.Equal(1, eventFiredCount);
    }

    [Fact]
    public void IncreaseSpeed_HalvesTickDuration()
    {
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));

        clock.IncreaseSpeed();

        Assert.Equal(TimeSpan.FromMilliseconds(250), clock.TickDuration);
    }

    [Fact]
    public void DecreaseSpeed_DoublesTickDuration()
    {
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(500));

        clock.DecreaseSpeed();

        Assert.Equal(TimeSpan.FromMilliseconds(1000), clock.TickDuration);
    }

    [Fact]
    public async Task StartAndStop_ManagesTimerCorrectly()
    {
        var clock = new SimulationClock(TimeSpan.FromMilliseconds(10));
        var eventFiredCount = 0;
        clock.Tick += (sender, e) => eventFiredCount++;


        clock.Start();
        await Task.Delay(50);
        clock.Stop();

        var ticksAfterStop = eventFiredCount;
        await Task.Delay(50); // time to ensure no more ticks have occured

        Assert.True(ticksAfterStop > 0);
        Assert.Equal(ticksAfterStop, eventFiredCount);
    }
}
