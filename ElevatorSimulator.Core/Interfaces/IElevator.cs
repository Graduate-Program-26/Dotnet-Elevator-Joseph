using ElevatorSimulator.Core.Entities;

namespace ElevatorSimulator.Core.Interfaces;

public interface IElevator
{
    Guid Id { get; }
    string Name { get; }
    int CurrentFloor { get; }
    double Position { get; }
    Enums.Direction Direction { get; }
    Enums.ElevatorState State { get; }
    int Capacity { get; }
    int Occupancy { get; }
    bool IsFull { get; }
    IReadOnlyList<Passenger> Passengers { get; }
    IReadOnlySet<int> UpStops { get; }
    IReadOnlySet<int> DownStops { get; }

    void AddStop(int floor, Enums.Direction direction);
    void RemoveStop(int floor, Enums.Direction direction);
}
