namespace ElevatorSimulator.ConsoleUI.ViewModels;


public sealed record HallCallViewModel(
    bool HasUpCall,
    bool HasDownCall,
    int UpPassengerCount = 0,
    int DownPassengerCount = 0);
