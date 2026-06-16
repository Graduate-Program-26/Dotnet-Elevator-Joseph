namespace ElevatorSimulator.ConsoleUI.ViewModels;

public sealed record ElevatorViewModel(
    Guid Id,
    string Label,
    double Position,
    DirectionViewModel Direction,
    int Occupancy,
    int Capacity,
    DoorState DoorState);
