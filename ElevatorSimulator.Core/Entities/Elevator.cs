using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Entities;

public class Elevator
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; }
    public int CurrentFloor { get; set; } = 0;
    public double Position { get; set; } = 0.0;
    public Direction Direction { get; set; } = Direction.Idle;
    public ElevatorState State { get; set; } = ElevatorState.Idle;
    public int Capacity { get; init; }
    
    public int Occupancy => Passengers.Count;
    public bool IsFull => Occupancy >= Capacity;

    public List<Passenger> Passengers { get; } = new();
    
    public HashSet<int> UpStops { get; } = new();
    public HashSet<int> DownStops { get; } = new();

    public Elevator(string name, int capacity)
    {
        Name = name;
        Capacity = capacity;
    }

    public void AddStop(int floor, Direction direction)
    {
        if (direction == Direction.Up)
        {
            UpStops.Add(floor);
        }
        else if (direction == Direction.Down)
        {
            DownStops.Add(floor);
        }
    }

    public void RemoveStop(int floor, Direction direction)
    {
        if (direction == Direction.Up)
        {
            UpStops.Remove(floor);
        }
        else if (direction == Direction.Down)
        {
            DownStops.Remove(floor);
        }
    }
}
