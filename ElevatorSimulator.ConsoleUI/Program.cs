using ElevatorSimulator.ConsoleUI.Views;
using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Infrastructure.Services;
using ElevatorSimulator.Simulation;
using ElevatorSimulator.Simulation.Strategies;

var building = new Building(floorCount: 4, elevatorCount: 3, elevatorCapacity: 5, tickDurationMs: 500, basementFloors:1);

var eventBus = new InMemoryEventBus();
var clock = new SimulationClock(TimeSpan.FromMilliseconds(building.TickDurationMs));
var engine = new SimulationEngine(clock, eventBus);

var strategy = new NearestElevatorStrategy();
var dispatcher = new Dispatcher(strategy, eventBus);

// Set up UI
var dashboard = new DashboardView();

// Start simulation async
_ = Task.Run(() => engine.Play());

// Start UI rendering loop blocking the main thread
dashboard.RenderLoop(engine, clock, building);
