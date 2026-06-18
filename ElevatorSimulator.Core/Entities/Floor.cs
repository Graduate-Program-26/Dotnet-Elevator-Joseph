using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Entities;

public class Floor
{
    public int Level { get; init; }

    public HallCall? UpCall { get; private set; }
    public HallCall? DownCall { get; private set; }

    public List<Passenger> WaitingUpPassengers { get; } = new();
    public List<Passenger> WaitingDownPassengers { get; } = new();

    public const int MaxWaitingPassengersPerDirection = 10;

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

    public bool CanAddPassenger(Direction direction)
    {
        if (direction == Direction.Up)
            return WaitingUpPassengers.Count < MaxWaitingPassengersPerDirection;
        return WaitingDownPassengers.Count < MaxWaitingPassengersPerDirection;
    }

    public void AddPassenger(Passenger passenger)
    {
        if (passenger.OriginFloor != Level)
            throw new ArgumentException("Passenger origin does not match this floor.");

        if (passenger.HallDirection == Direction.Up)
        {
            if (WaitingUpPassengers.Count >= MaxWaitingPassengersPerDirection) return;
            WaitingUpPassengers.Add(passenger);
        }
        else
        {
            if (WaitingDownPassengers.Count >= MaxWaitingPassengersPerDirection) return;
            WaitingDownPassengers.Add(passenger);
        }
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
