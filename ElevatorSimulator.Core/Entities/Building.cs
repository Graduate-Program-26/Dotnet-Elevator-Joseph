namespace ElevatorSimulator.Core.Entities;

public class Building
{
    public int TickDurationMs { get; init; }
    public List<Floor> Floors { get; } = new();
    public List<Elevator> Elevators { get; } = new();

    public Building(int floorCount, int elevatorCount, int elevatorCapacity, int tickDurationMs = 500, int basementFloors = 0)
    {
        TickDurationMs = tickDurationMs;

        for (int i = 0; i < floorCount; i++)
        {
            Floors.Add(new Floor(i));
        }
        for (int i = 1; i <= basementFloors; i++)
        {
            Floors.Add(new Floor(-i));
        }

        for (int i = 1; i <= elevatorCount; i++)
        {
            Elevators.Add(new Elevator($"Elevator-{i}", elevatorCapacity));
        }
    }
    public int GetFloorCount() => Floors.Count();
    public int GetElevatorCount() => Elevators.Count();
    public IEnumerable<HallCall> GetActiveHallCalls() => Floors.SelectMany(f => f.GetActiveCalls());
}
