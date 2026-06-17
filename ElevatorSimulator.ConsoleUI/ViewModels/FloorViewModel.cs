namespace ElevatorSimulator.ConsoleUI.ViewModels;

public sealed record FloorViewModel(
    int Level,
    HallCallViewModel HallCall);
