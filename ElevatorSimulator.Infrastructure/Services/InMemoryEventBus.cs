using System.Collections.Concurrent;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Infrastructure.Services;

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Delegate>> _handlers = new();

    public void Publish<TEvent>(TEvent @event) where TEvent : DomainEvent
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var delegates))
        {
            foreach (var handler in delegates)
            {
                if (handler is Action<TEvent> typedHandler)
                {
                    typedHandler(@event);
                }
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent
    {
        var eventType = typeof(TEvent);
        var bag = _handlers.GetOrAdd(eventType, _ => new ConcurrentBag<Delegate>());
        bag.Add(handler);
    }
}
