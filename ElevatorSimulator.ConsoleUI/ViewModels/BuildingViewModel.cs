namespace ElevatorSimulator.ConsoleUI.ViewModels;

public sealed record BuildingViewModel(
    IReadOnlyList<FloorViewModel> Floors,
    IReadOnlyList<ElevatorViewModel> Elevators);
