using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Events;

public abstract record DomainEvent;

public record PassengerCreatedEvent(Passenger Passenger) : DomainEvent;
public record HallCallCreatedEvent(HallCall HallCall) : DomainEvent;
public record HallCallAssignedEvent(HallCall HallCall, Guid ElevatorId) : DomainEvent;
public record ElevatorArrivedEvent(Elevator Elevator, int Floor) : DomainEvent;
public record PassengerBoardedEvent(Passenger Passenger, Elevator Elevator) : DomainEvent;
public record PassengerExitedEvent(Passenger Passenger, Elevator Elevator) : DomainEvent;
public record DirectionChangedEvent(Elevator Elevator, Direction NewDirection) : DomainEvent;
public record ElevatorFullEvent(Elevator Elevator) : DomainEvent;
public record SimulationStartedEvent() : DomainEvent;
public record SimulationPausedEvent() : DomainEvent;
public record SimulationStoppedEvent() : DomainEvent;
