using ElevatorSimulator.ConsoleUI.Components;
using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Interfaces;
using ElevatorSimulator.Infrastructure.Services;
using ElevatorSimulator.Simulation;
using ElevatorSimulator.Simulation.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;

//  Simulation setup 

var building = new Building(
    floorCount: 4,
    elevatorCount: 4,
    elevatorCapacity: 5,
    tickDurationMs: 500,
    basementFloors: 1);
building.Elevators[1].DoorState = ElevatorDoorState.Open;
building.Elevators[3].AddStop(3, Direction.Up);


var eventBus  = new InMemoryEventBus();
var clock     = new SimulationClock(TimeSpan.FromMilliseconds(building.TickDurationMs));
var engine    = new SimulationEngine(clock, eventBus);
var strategy  = new NearestElevatorStrategy();
var dispatcher = new Dispatcher(strategy, eventBus);


_ = Task.Run(() => engine.Play());

var host = Host.CreateDefaultBuilder(args)
    .UseRazorConsole<BuildingComponent>()
    .ConfigureServices(services =>
    {
        // Register domain singletons for Razor @inject
        services.AddSingleton(building);
        services.AddSingleton<ISimulationEngine>(engine);
        services.AddSingleton<ISimulationClock>(clock);
        services.AddSingleton<IEventBus>(eventBus);
        services.AddSingleton<Dispatcher>(dispatcher);
    })
    .Build();

// Clear the console on startup
Spectre.Console.AnsiConsole.Clear();

try
{
    await host.RunAsync();
}
finally
{
    // Clear the console again on shutdown/stop
    Spectre.Console.AnsiConsole.Clear();
}
