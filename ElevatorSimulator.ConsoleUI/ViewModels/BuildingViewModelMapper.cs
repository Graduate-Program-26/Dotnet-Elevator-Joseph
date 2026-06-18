using ElevatorSimulator.Core.Entities;
using CoreDirection = ElevatorSimulator.Core.Enums.Direction;
using CoreElevatorState = ElevatorSimulator.Core.Enums.ElevatorState;
using CoreDoorState = ElevatorSimulator.Core.Enums.ElevatorDoorState;


namespace ElevatorSimulator.ConsoleUI.ViewModels;

public static class BuildingViewModelMapper
{
    public static BuildingViewModel Map(Building building)
    {
        // Floors rendered top → bottom (descending level)
        var floors = building.Floors
            .OrderByDescending(f => f.Level)
            .Select(MapFloor)
            .ToList();

        // Elevators labelled A, B, C … by fleet index
        var elevators = building.Elevators
            .Select((e, index) => MapElevator(e, index))
            .ToList();

        return new BuildingViewModel(floors, elevators);
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private static FloorViewModel MapFloor(Floor floor) =>
        new(floor.Level, MapHallCall(floor));

    private static HallCallViewModel MapHallCall(Floor floor) =>
        new(
            HasUpCall:   floor.UpCall   != null,
            HasDownCall: floor.DownCall != null,
            UpPassengerCount:   floor.WaitingUpPassengers.Count,
            DownPassengerCount: floor.WaitingDownPassengers.Count);

    private static ElevatorViewModel MapElevator(Elevator elevator, int fleetIndex)
    {
        // Single-letter label: 0→A, 1→B, 2→C, …
        // Falls back to numeric label for >26 elevators.
        var label = fleetIndex < 26
            ? ((char)('A' + fleetIndex)).ToString()
            : (fleetIndex + 1).ToString();

        return new ElevatorViewModel(
            Id:              elevator.Id,
            Label:           label,
            CurrentFloor:    elevator.CurrentFloor,
            TransitionTicks: elevator.TransitionTicks,
            Direction:       MapDirection(elevator.Direction),
            Occupancy: elevator.Occupancy,
            Capacity:  elevator.Capacity,
            DoorState: MapDoorState(elevator.DoorState),
            UpStops:   elevator.UpStops.OrderBy(s => s).ToList(),
            DownStops: elevator.DownStops.OrderByDescending(s => s).ToList());
    }

       private static DirectionViewModel MapDirection(CoreDirection direction) => direction switch
    {
        CoreDirection.Up   => DirectionViewModel.Up,
        CoreDirection.Down => DirectionViewModel.Down,
        _                  => DirectionViewModel.Idle,
    };
    private static DoorState MapDoorState(CoreDoorState state) => state switch
    {
        CoreDoorState.Opening => DoorState.Opening,
        CoreDoorState.Open    => DoorState.Open,
        CoreDoorState.Closing => DoorState.Closing,
        CoreDoorState.PartialyClosed => DoorState.PartialyClosed,
        CoreDoorState.PartialyOpened => DoorState.PartialyOpened,
        _                     => DoorState.Closed, 
    };
}
