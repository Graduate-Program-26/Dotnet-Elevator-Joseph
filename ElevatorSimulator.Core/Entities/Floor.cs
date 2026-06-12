using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Entities;

public class Floor
{
    public int Level { get; init; }

    public HallCall? UpCall { get; private set; }
    public HallCall? DownCall { get; private set; }

    public Floor(int level)
    {
        Level = level;
    }

    public void AddCall(HallCall call)
    {
        if (call.Floor != Level)
            throw new ArgumentException("Call does not belong to this floor.");

        if (call.Direction == Direction.Up)
            UpCall = call;
        else if (call.Direction == Direction.Down)
            DownCall = call;
    }

    public void ClearCall(Direction direction)
    {
        if (direction == Direction.Up)
            UpCall = null;
        else if (direction == Direction.Down)
            DownCall = null;
    }

    public IEnumerable<HallCall> GetActiveCalls()
    {
        if (UpCall != null) yield return UpCall;
        if (DownCall != null) yield return DownCall;
    }
}
