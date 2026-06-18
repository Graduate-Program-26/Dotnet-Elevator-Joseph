namespace ElevatorSimulator.ConsoleUI.ViewModels;

public sealed record ElevatorViewModel(
    Guid Id,
    string Label,
    int CurrentFloor,
    int TransitionTicks,
    DirectionViewModel Direction,
    int Occupancy,
    int Capacity,
    DoorState DoorState,
    IReadOnlyList<int> UpStops,
    IReadOnlyList<int> DownStops);
