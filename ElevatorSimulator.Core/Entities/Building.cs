namespace ElevatorSimulator.Core.Entities;

public class Building
{
    public int FloorCount { get; init; }
    public int ElevatorCount { get; init; }
    public int ElevatorCapacity { get; init; }
    public int TickDurationMs { get; init; }
    
    public List<Floor> Floors { get; } = new();
    public List<Elevator> Elevators { get; } = new();

    public Building(int floorCount, int elevatorCount, int elevatorCapacity, int tickDurationMs = 500, int basementFloors = 0)
    {
        FloorCount = floorCount + basementFloors;
        ElevatorCount = elevatorCount;
        ElevatorCapacity = elevatorCapacity;
        TickDurationMs = tickDurationMs;

        for (int i = 0; i < FloorCount; i++)
        {
            Floors.Add(new Floor(i));
        }
        for ( int i = 1; i <= basementFloors; i++)
        {
            Floors.Add(new Floor(-i));
        }

        for (int i = 1; i <= ElevatorCount; i++)
        {
            Elevators.Add(new Elevator($"Elevator-{i}", ElevatorCapacity));
        }
    }
}
