using ElevatorSimulator.Core.Events;

namespace ElevatorSimulator.Core.Interfaces;

public interface IEventBus
{
    void Publish<TEvent>(TEvent @event) where TEvent : DomainEvent;
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent;
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent;
}
