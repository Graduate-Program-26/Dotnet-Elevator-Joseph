using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Entities;

public class HallCall
{
    public int Floor { get; init; }
    public Direction Direction { get; init; }
    public HallCallStatus Status { get; set; } = HallCallStatus.Created;
    public Guid? AssignedElevatorId { get; set; }

    public HallCall(int floor, Direction direction)
    {
        Floor = floor;
        Direction = direction;
    }
}
