using ElevatorSimulator.Core.Entities;
using Spectre.Console;

namespace ElevatorSimulator.ConsoleUI.Renderers;

public class DetailsRenderer
{
    public Grid Render(Building building)
    {
        var grid = new Grid()
            .AddColumn(new GridColumn())
            .AddColumn(new GridColumn());

        var elevatorPanel = RenderElevatorDetails(building.Elevators.FirstOrDefault());//Todo: select elevator by user selection
        var floorPanel = RenderFloorDetails(building.Floors.FirstOrDefault()); // Todo: select floor by user selection

        grid.AddRow(elevatorPanel, floorPanel);
        return grid;
    }

    private Panel RenderElevatorDetails(Elevator? elevator)
    {
        if (elevator == null) return new Panel("No Elevator Selected");

        var text = $@"
[bold]Floor:[/] {elevator.CurrentFloor}
[bold]Direction:[/] {elevator.Direction}
[bold]Occupancy:[/] {elevator.Occupancy}/{elevator.Capacity}
[bold]State:[/] {elevator.State}

[bold]Up Stops:[/]
{string.Join("\n", elevator.UpStops)}

[bold]Down Stops:[/]
{string.Join("\n", elevator.DownStops)}
";
        return new Panel(new Markup(text))
        {
            Header = new PanelHeader($"Elevator {elevator.Name}"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
    }

    private Panel RenderFloorDetails(Floor? floor)
    {
        if (floor == null) return new Panel("No Floor Selected");

        var text = $@"
[bold]UP CALL[/]
Status: {(floor.UpCall != null ? floor.UpCall.Status.ToString() : "None")}
Elevator: {(floor.UpCall?.AssignedElevatorId?.ToString() ?? "None")}

[bold]DOWN CALL[/]
Status: {(floor.DownCall != null ? floor.DownCall.Status.ToString() : "None")}
Elevator: {(floor.DownCall?.AssignedElevatorId?.ToString() ?? "None")}
";
        return new Panel(new Markup(text))
        {
            Header = new PanelHeader($"Floor {floor.Level}"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
    }
}
