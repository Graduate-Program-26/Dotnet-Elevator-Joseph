using ElevatorSimulator.Core.Entities;
using ElevatorSimulator.Core.Enums;
using Spectre.Console;
using System.Text;

namespace ElevatorSimulator.ConsoleUI.Renderers;

public class BuildingShaftRenderer
{
    private const int FloorLabelWidth = 4;  // width of left label column (floor # + calls)
    private const int ShaftInnerWidth = 11; // inner width of each elevator shaft
    private const int FloorContentLines = 5; // content lines per floor

    public Panel Render(Building building)
    {
        var text = BuildShaftText(building);
        return new Panel(new Text(text))
        {
            Header = new PanelHeader("Building Shaft View"),
            Border = BoxBorder.Rounded,
        };
    }

    private string BuildShaftText(Building building)
    {
        var elevators = building.Elevators;
        var floors = building.Floors.OrderByDescending(f => f.Level).ToList();
        var sb = new StringBuilder();

        sb.AppendLine(Separator(elevators.Count));

        foreach (var floor in floors)
        {
            for (int line = 0; line < FloorContentLines; line++)
            {
                sb.Append('|');

                // Floor label column
                if (line == 0)
                {
                    var upI  = floor.UpCall   != null ? "^" : " ";
                    var downI = floor.DownCall != null ? "v" : " ";
                    var label = $" {floor.Level,2} {upI}{downI} ";
                    sb.Append(Fit(label, FloorLabelWidth));
                }
                else
                {
                    sb.Append(new string(' ', FloorLabelWidth));
                }

                // Shaft columns
                foreach (var elevator in elevators)
                {
                    var isOnFloor = (int)Math.Round(elevator.Position) == floor.Level;
                    sb.Append('|');
                    if (isOnFloor)
                    {
                        sb.Append(ElevatorBoxLine(elevator, line));
                    }
                    else
                    {
                        sb.Append(new string(' ', ShaftInnerWidth));
                    }
                }

                sb.AppendLine("|");
            }

            sb.AppendLine(Separator(elevators.Count));
        }

        return sb.ToString();
    }

    private string ElevatorBoxLine(Elevator elevator, int line)
    {
        var inner = ShaftInnerWidth;
        var boxInner = inner - 2; // inside the +---+ borders

        var dir = elevator.Direction switch
        {
            Direction.Up   => "^",
            Direction.Down => "v",
            _              => "-"
        };

        return line switch
        {
            0 => $"+{new string('-', boxInner)}+",           // top border
            1 => $"|{Fit($" {elevator.Name[..1]} {dir} {elevator.Occupancy}/{elevator.Capacity} ", boxInner)}|", // info
            2 => $"+{new string('-', boxInner)}+",           // mid border
            3 => $"|{new string(' ', boxInner)}|",           // interior
            4 => $"|{new string(' ', boxInner)}|",           // interior
            _ => new string(' ', inner)
        };
    }

    private string Separator(int elevatorCount)
    {
        var sb = new StringBuilder();
        sb.Append('+');
        sb.Append(new string('-', FloorLabelWidth));
        for (int i = 0; i < elevatorCount; i++)
        {
            sb.Append('+');
            sb.Append(new string('-', ShaftInnerWidth));
        }
        sb.Append('+');
        return sb.ToString();
    }

    private static string Fit(string text, int width)
    {
        if (text.Length >= width) return text[..width];
        return text.PadRight(width);
    }
}
