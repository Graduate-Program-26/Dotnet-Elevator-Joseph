using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation.Strategies;

public class NearestElevatorStrategy : IDispatchStrategy
{
    public Elevator? AssignElevator(IEnumerable<Elevator> elevators, HallCall hallCall)
    {
        var eligibleElevators = elevators.Where(e => IsEligible(e, hallCall));

        return eligibleElevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - hallCall.Floor))
            .FirstOrDefault();
    }

    private bool IsEligible(Elevator elevator, HallCall hallCall)
    {
        if (elevator.State == ElevatorState.Idle)
            return true;

        if (elevator.Direction != hallCall.Direction)
            return false;

        if (elevator.IsFull)
            return false;

        if (elevator.Direction == Direction.Up && elevator.CurrentFloor > hallCall.Floor)
            return false;

        if (elevator.Direction == Direction.Down && elevator.CurrentFloor < hallCall.Floor)
            return false;

        return true;
    }
}
