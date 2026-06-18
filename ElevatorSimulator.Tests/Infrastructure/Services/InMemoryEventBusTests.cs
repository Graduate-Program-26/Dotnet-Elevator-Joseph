using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Infrastructure.Services;
using FluentAssertions;

namespace ElevatorSimulator.Tests.Infrastructure.Services;

public class InMemoryEventBusTests
{
    private record TestEvent : DomainEvent;

    [Fact]
    public void Publish_ShouldInvokeSubscribedHandler()
    {
        var eventBus = new InMemoryEventBus();
        bool handled = false;

        eventBus.Subscribe<TestEvent>(e => handled = true);
        eventBus.Publish(new TestEvent());

        handled.Should().BeTrue();
    }

    [Fact]
    public void Publish_ShouldInvokeMultipleSubscribedHandlers()
    {
        var eventBus = new InMemoryEventBus();
        int handleCount = 0;

        eventBus.Subscribe<TestEvent>(e => handleCount++);
        eventBus.Subscribe<TestEvent>(e => handleCount++);

        eventBus.Publish(new TestEvent());

        handleCount.Should().Be(2);
    }

    [Fact]
    public void Publish_WithNoSubscribers_ShouldNotThrow()
    {
        var eventBus = new InMemoryEventBus();

        Action act = () => eventBus.Publish(new TestEvent());

        act.Should().NotThrow();
    }

    [Fact]
    public void Unsubscribe_ShouldRemoveHandler()
    {
        var eventBus = new InMemoryEventBus();
        int handleCount = 0;

        Action<TestEvent> handler = e => handleCount++;

        eventBus.Subscribe(handler);
        eventBus.Unsubscribe(handler);

        eventBus.Publish(new TestEvent());

        handleCount.Should().Be(0);
    }
}
