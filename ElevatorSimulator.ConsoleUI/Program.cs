using ElevatorSimulator.ConsoleUI.Views;
using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Infrastructure.Services;
using ElevatorSimulator.Simulation;
using ElevatorSimulator.Simulation.Strategies;

var building = new Building(floorCount: 10, elevatorCount: 2, elevatorCapacity: 8, tickDurationMs: 500);

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
