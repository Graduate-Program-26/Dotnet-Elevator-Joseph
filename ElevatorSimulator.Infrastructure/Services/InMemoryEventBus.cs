using System.Collections.Concurrent;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Infrastructure.Services;

public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<TEvent>(TEvent @event) where TEvent : DomainEvent
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var delegates))
        {
            List<Delegate> snapshot;
            lock (delegates)
            {
                snapshot = delegates.ToList();
            }

            foreach (var handler in snapshot)
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
        var delegates = _handlers.GetOrAdd(eventType, _ => new List<Delegate>());
        lock (delegates)
        {
            delegates.Add(handler);
        }
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var delegates))
        {
            lock (delegates)
            {
                delegates.Remove(handler);
            }
        }
    }
}
