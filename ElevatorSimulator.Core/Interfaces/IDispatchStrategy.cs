using ElevatorSimulator.Core.Entities;

namespace ElevatorSimulator.Core.Interfaces;

public interface IDispatchStrategy
{
    Elevator? AssignElevator(IEnumerable<Elevator> elevators, HallCall hallCall);
}
