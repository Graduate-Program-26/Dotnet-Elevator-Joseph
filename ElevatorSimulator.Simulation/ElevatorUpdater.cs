using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using ElevatorSimulator.Core.Events;
using ElevatorSimulator.Core.Interfaces;

namespace ElevatorSimulator.Simulation;

public class ElevatorUpdater
{
    private readonly Building _building;
    private readonly IEventBus _eventBus;

    public ElevatorUpdater(Building building, IEventBus eventBus)
    {
        _building = building ?? throw new ArgumentNullException(nameof(building));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    /// <summary>
    /// Advances the elevator one simulation tick through its state machine.
    /// Movement is bounded to the floors that actually exist in the building,
    /// including negative basement levels (e.g. -1, -2 …).
    /// Passengers alight at their destination and board at hall-call floors;
    /// partial boarding leaves the hall call active for re-dispatch.
    /// </summary>
    public void Update(Elevator elevator)
    {
        // ── Door logic ────────────────────────────────────────────────────────
        if (elevator.DoorState == ElevatorDoorState.Opening)
        {
            elevator.DoorState = ElevatorDoorState.PartialyOpened;
            return;
        }
        if (elevator.DoorState == ElevatorDoorState.PartialyOpened)
        {
            elevator.DoorState = ElevatorDoorState.Open;
            return;
        }

        if (elevator.DoorState == ElevatorDoorState.Open)
        {
            // Transfer passengers while doors are fully open
            var floor = _building.Floors.FirstOrDefault(f => f.Level == elevator.CurrentFloor);
            if (floor != null)
                BoardAndAlight(elevator, floor);

            elevator.DoorState = ElevatorDoorState.Closing;
            return;
        }

        if (elevator.DoorState == ElevatorDoorState.Closing)
        {
            elevator.DoorState = ElevatorDoorState.PartialyClosed;
            return;
        }
        if (elevator.DoorState == ElevatorDoorState.PartialyClosed)
        {
            elevator.DoorState = ElevatorDoorState.Closed;

            // Resume movement if a direction is still set
            if (elevator.Direction != Direction.Idle)
                elevator.State = elevator.Direction == Direction.Up ? ElevatorState.MovingUp : ElevatorState.MovingDown;
            else
                elevator.State = ElevatorState.Idle;
            return;
        }

        // Movement logic
        if (elevator.State == ElevatorState.MovingUp || elevator.State == ElevatorState.MovingDown)
        {
            int minFloor = _building.Floors.Min(f => f.Level);
            int maxFloor = _building.Floors.Max(f => f.Level);

            elevator.TransitionTicks++;

            if (elevator.TransitionTicks >= Elevator.TicksPerFloor)
            {
                // Find next floor based on direction
                var nextFloorQuery = elevator.State == ElevatorState.MovingUp
                    ? _building.Floors.Where(f => f.Level > elevator.CurrentFloor).OrderBy(f => f.Level)
                    : _building.Floors.Where(f => f.Level < elevator.CurrentFloor).OrderByDescending(f => f.Level);

                var nextFloor = nextFloorQuery.FirstOrDefault();

                if (nextFloor != null)
                {
                    elevator.CurrentFloor = nextFloor.Level;
                }
                elevator.TransitionTicks = 0;

                // Safety clamp
                if (elevator.CurrentFloor <= minFloor && elevator.State == ElevatorState.MovingDown)
                {
                    elevator.CurrentFloor = minFloor;
                    elevator.State = ElevatorState.Idle;
                    elevator.Direction = Direction.Idle;
                    elevator.DownStops.RemoveWhere(s => s < minFloor);
                }
                else if (elevator.CurrentFloor >= maxFloor && elevator.State == ElevatorState.MovingUp)
                {
                    elevator.CurrentFloor = maxFloor;
                    elevator.State = ElevatorState.Idle;
                    elevator.Direction = Direction.Idle;
                    elevator.UpStops.RemoveWhere(s => s > maxFloor);
                }

                bool hasUpStop = elevator.UpStops.Contains(elevator.CurrentFloor);
                bool hasDownStop = elevator.DownStops.Contains(elevator.CurrentFloor);

                if (hasUpStop || hasDownStop)
                {
                    // Arrived at a stop
                    Direction stopDirection = elevator.Direction;
                    if (stopDirection == Direction.Up && !hasUpStop && hasDownStop) stopDirection = Direction.Down;
                    if (stopDirection == Direction.Down && !hasDownStop && hasUpStop) stopDirection = Direction.Up;

                    elevator.RemoveStop(elevator.CurrentFloor, stopDirection);
                    elevator.Direction = stopDirection;
                    elevator.State = ElevatorState.Idle;
                    elevator.DoorState = ElevatorDoorState.Opening;
                    _eventBus.Publish(new ElevatorArrivedEvent(elevator, elevator.CurrentFloor));

                    // Become idle when no further stops remain in the travel direction
                    if (elevator.Direction == Direction.Up && !elevator.UpStops.Any(s => s >= elevator.CurrentFloor))
                        elevator.Direction = Direction.Idle;
                    if (elevator.Direction == Direction.Down && !elevator.DownStops.Any(s => s <= elevator.CurrentFloor))
                        elevator.Direction = Direction.Idle;
                }
            }
            return;
        }

        // Idle state: decide if we should start moving
        if (elevator.State == ElevatorState.Idle)
        {
            if (elevator.UpStops.Any() || elevator.DownStops.Any())
            {
                var allStops = elevator.UpStops.Select(s => new { Floor = s, Dir = Direction.Up })
                    .Concat(elevator.DownStops.Select(s => new { Floor = s, Dir = Direction.Down }));

                var nextStop = allStops.OrderBy(s => Math.Abs(s.Floor - elevator.CurrentFloor)).FirstOrDefault();

                if (nextStop != null)
                {
                    elevator.Direction = nextStop.Floor > elevator.CurrentFloor ? Direction.Up :
                                         nextStop.Floor < elevator.CurrentFloor ? Direction.Down : nextStop.Dir;

                    if (elevator.CurrentFloor == nextStop.Floor)
                    {
                        // Already on that floor — open doors immediately
                        elevator.DoorState = ElevatorDoorState.Opening;
                        elevator.RemoveStop(nextStop.Floor, nextStop.Dir);
                    }
                    else
                    {
                        elevator.State = elevator.Direction == Direction.Up ? ElevatorState.MovingUp : ElevatorState.MovingDown;
                        elevator.TransitionTicks = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called once per door-open cycle.
    /// 1. Alights any passengers whose destination is this floor.
    /// 2. Boards waiting passengers in the stop direction up to remaining capacity.
    ///    If passengers remain after the elevator fills, the hall call is reset to
    ///    Created so the dispatcher can reassign another elevator.
    /// </summary>
    private void BoardAndAlight(Elevator elevator, Floor floor)
    {
        // Alight passengers whose destination is this floor
        var alighting = elevator.Passengers
            .Where(p => p.DestinationFloor == floor.Level)
            .ToList();

        foreach (var passenger in alighting)
        {
            passenger.State = PassengerState.Arrived;
            elevator.Passengers.Remove(passenger);
            _eventBus.Publish(new PassengerExitedEvent(passenger, elevator));
        }

        // Board waiting passengers 
        // Determine which queue to drain based on the elevator's current direction.
        // If idle (just alighted everyone), prefer the queue that has people.
        Direction boardingDirection = elevator.Direction;
        if (boardingDirection == Direction.Idle)
        {
            if (floor.WaitingUpPassengers.Any()) boardingDirection = Direction.Up;
            else if (floor.WaitingDownPassengers.Any()) boardingDirection = Direction.Down;
        }

        var waitingQueue = boardingDirection == Direction.Up
            ? floor.WaitingUpPassengers
            : floor.WaitingDownPassengers;

        if (!waitingQueue.Any()) return;

        int boarded = 0;
        while (waitingQueue.Any() && !elevator.IsFull)
        {
            var passenger = waitingQueue[0];
            waitingQueue.RemoveAt(0);

            passenger.State = PassengerState.Riding;
            elevator.Passengers.Add(passenger);

            // Register the passenger's destination as a stop
            elevator.AddStop(passenger.DestinationFloor, passenger.HallDirection);

            _eventBus.Publish(new PassengerBoardedEvent(passenger, elevator));
            boarded++;
        }

        // Decide whether to keep or clear the hall call 
        if (waitingQueue.Any())
        {
            // Passengers remain — elevator is full. Reset the hall call so the
            // dispatcher re-assigns a different elevator.
            var hallCall = boardingDirection == Direction.Up ? floor.UpCall : floor.DownCall;
            if (hallCall != null)
            {
                hallCall.Status = HallCallStatus.Created;
                hallCall.AssignedElevatorId = null;
            }

            if (elevator.IsFull)
                _eventBus.Publish(new ElevatorFullEvent(elevator));
        }
        else
        {
            // All passengers boarded — clear the hall call
            floor.ClearCall(boardingDirection);
        }
    }
}
