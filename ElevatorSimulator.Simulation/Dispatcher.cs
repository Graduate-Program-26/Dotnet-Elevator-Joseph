using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class Dispatcher
{
    private readonly IDispatchStrategy _strategy;
    private readonly IEventBus _eventBus;

    public Dispatcher(IDispatchStrategy strategy, IEventBus eventBus)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public void Dispatch(IEnumerable<Elevator> elevators, IEnumerable<HallCall> pendingCalls)
    {
        foreach (var call in pendingCalls)
        {
            if (call.Status != HallCallStatus.Created)
                continue;

            var elevator = _strategy.AssignElevator(elevators, call);
            if (elevator != null)
            {
                call.Status = HallCallStatus.Assigned;
                call.AssignedElevatorId = elevator.Id;
                elevator.AddStop(call.Floor, call.Direction);
                
                _eventBus.Publish(new HallCallAssignedEvent(call, elevator.Id));
            }
        }
    }
}
