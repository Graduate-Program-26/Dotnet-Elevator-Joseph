using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Entities;

public class Passenger
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int OriginFloor { get; init; }
    public int DestinationFloor { get; init; }
    public Direction HallDirection { get; init; }
    public PassengerState State { get; set; } = PassengerState.Waiting;

    public Passenger(int originFloor, int destinationFloor, Direction? explicitDirection = null)
    {
        OriginFloor = originFloor;
        DestinationFloor = destinationFloor;

        if (explicitDirection.HasValue)
        {
            HallDirection = explicitDirection.Value;
        }
        else
        {
            HallDirection = DestinationFloor > OriginFloor ? Direction.Up : Direction.Down;
        }
    }
}
