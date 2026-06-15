using ElevatorSimulator.Core.Interfaces;
using Spectre.Console;

namespace ElevatorSimulator.ConsoleUI.Renderers;

public class StatusBarRenderer
{
    public Panel Render(ISimulationEngine engine, ISimulationClock clock)
    {
        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn(new GridColumn());

        grid.AddRow(
            $"[bold cyan]Tick:[/] {clock.TotalTicks}",
            $"[bold cyan]Speed:[/] {clock.TickDuration.TotalMilliseconds}ms",
            $"[bold cyan]State:[/] {(engine.CurrentState.ToString() == "Playing" ? "[green]Playing[/]" : $"[yellow]{engine.CurrentState}[/]")}",
        );

        return new Panel(grid)
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0)
            };
    }
}
