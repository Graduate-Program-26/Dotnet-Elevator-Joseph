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
using Serilog;
using ElevatorSimulator.ConsoleUI.Setup;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/elevator_simulation.txt", rollingInterval: RollingInterval.Day) 
    .Enrich.FromLogContext()
    .CreateLogger();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    Log.Fatal(
        e.ExceptionObject as Exception,
        "UNHANDLED EXCEPTION");
};
TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Log.Fatal(
        e.Exception,
        "UNOBSERVED TASK EXCEPTION");

    e.SetObserved();
};

try
{
    Log.Information("Starting Elevator Simulation System...");

var config = ConfigurationPrompt.GetConfiguration();

var building = new Building(
    floorCount: config.FloorCount,
    elevatorCount: config.ElevatorCount,
    elevatorCapacity: config.ElevatorCapacity,
    tickDurationMs: config.TickDurationMs,
    basementFloors: config.BasementFloors);

var eventBus  = new InMemoryEventBus();
var clock     = new SimulationClock(TimeSpan.FromMilliseconds(building.TickDurationMs));
var strategy  = new NearestElevatorStrategy();
var dispatcher = new Dispatcher(strategy, eventBus);
var engine    = new SimulationEngine(building, clock, eventBus, dispatcher);


_ = Task.Run(() =>
{
    try
    {
        engine.Play();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Simulation Engine Crashed");
        throw;
    }
});

var restartCount = 0;

var shuttingDown = false;

Console.CancelKeyPress += (_, e) =>
{
    Log.Information("Shutdown requested (Ctrl+C)");

    shuttingDown = true;

    e.Cancel = false;
};

while (!shuttingDown)
{
    try
    {
        Log.Information("Starting UI Host...");

        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Debug()
                    .WriteTo.File(
                        "logs/elevator-simulation-.log",
                        rollingInterval: RollingInterval.Day);
            })
            .UseRazorConsole<BuildingComponent>()
            .ConfigureServices(services =>
            {
                services.AddSingleton(building);
                services.AddSingleton<ISimulationEngine>(engine);
                services.AddSingleton<ISimulationClock>(clock);
                services.AddSingleton<IEventBus>(eventBus);
                services.AddSingleton<Dispatcher>(dispatcher);
            })
            .Build();

        await host.RunAsync();
        restartCount = 0;

        Log.Warning("UI Host exited normally.");
    }
    catch (Exception ex)
    {
        restartCount++;

        Log.Error(ex,
            "UI crashed. Restart attempt {RestartCount}",
            restartCount);

        if (restartCount > 10)
        {
            Log.Fatal(
                "UI exceeded maximum restart attempts.");
            break;
        }

        await Task.Delay(
            TimeSpan.FromSeconds(
                Math.Min(restartCount * 2, 30)));
    }
}}
catch (Exception ex) {
    Log.Error(ex, "UI crashed");
}