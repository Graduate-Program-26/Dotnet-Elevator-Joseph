using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class RandomPassengerGenerator
{
    private readonly Building _building;
    private readonly IEventBus _eventBus;
    private readonly Random _random = new();

    // Chances of a passenger generating per tick
    public double SpawnProbabilityPerTick { get; set; } = 0.1;

    public RandomPassengerGenerator(Building building, IEventBus eventBus)
    {
        _building = building ?? throw new ArgumentNullException(nameof(building));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public void Tick()
    {
        if (_building.GetFloorCount() < 2) return;

        if (_random.NextDouble() < SpawnProbabilityPerTick)
        {
            GenerateRandomPassenger();
        }
    }

    private void GenerateRandomPassenger()
    {
        var floors = _building.Floors;

        var originFloor = floors[_random.Next(floors.Count)];

        Floor destFloor;
        do
        {
            destFloor = floors[_random.Next(floors.Count)];
        } while (destFloor.Level == originFloor.Level);

        var direction = destFloor.Level > originFloor.Level ? Direction.Up : Direction.Down;

        if (!originFloor.CanAddPassenger(direction))
        {
            return; // Queue is full
        }

        var passenger = new Passenger(originFloor.Level, destFloor.Level, direction);
        
        originFloor.AddPassenger(passenger);
        _eventBus.Publish(new PassengerCreatedEvent(passenger));

        // Create hall call if it doesn't exist
        var activeCalls = originFloor.GetActiveCalls();
        if (!activeCalls.Any(c => c.Direction == direction))
        {
            var call = new HallCall(originFloor.Level, direction);
            originFloor.AddCall(call);
            _eventBus.Publish(new HallCallCreatedEvent(call));
        }
    }
}
