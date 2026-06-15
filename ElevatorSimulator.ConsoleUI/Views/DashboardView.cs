using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Interfaces;
using Spectre.Console;

namespace ElevatorSimulator.ConsoleUI.Views;

public class DashboardView
{
    private readonly Renderers.StatusBarRenderer _statusBarRenderer = new();
    private readonly Renderers.BuildingShaftRenderer _buildingShaftRenderer = new();
    private readonly Renderers.DetailsRenderer _detailsRenderer = new();

    public void RenderLoop(ISimulationEngine engine, ISimulationClock clock, Building building)
    {
        AnsiConsole.Clear();

        AnsiConsole.Live(new Markup("Initializing..."))
            .AutoClear(false)
            .Overflow(VerticalOverflow.Visible)
            .Start(ctx =>
            {
                while (true)
                {
                    var dashboard = new Rows(
                        _statusBarRenderer.Render(engine, clock),
                        _buildingShaftRenderer.Render(building),
                        _detailsRenderer.Render(building)
                    );

                    ctx.UpdateTarget(dashboard);

                    Thread.Sleep(100); // UI refresh rate
                }
            });
    }
}
