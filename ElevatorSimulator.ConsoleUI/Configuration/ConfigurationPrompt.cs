using ElevatorSimulator.ConsoleUI.Configuration;

namespace ElevatorSimulator.ConsoleUI.Setup;

public static class ConfigurationPrompt
{
    public static SimulationConfiguration GetConfiguration()
    {
        Console.Clear();

        Console.WriteLine("==================================");
        Console.WriteLine(" Elevator Simulation Configuration");
        Console.WriteLine("==================================");
        Console.WriteLine();

        var config = new SimulationConfiguration
        {
            FloorCount = ReadInt(
                "Number of floors (2-6): ",
                2,
                6),

            BasementFloors = ReadInt(
                "Number of basement floors (0-3): ",
                0,
                3),

            ElevatorCount = ReadInt(
                "Number of elevators (1-8): ",
                1,
                8),

            ElevatorCapacity = ReadInt(
                "Elevator capacity (1-20): ",
                1,
                20),

            TickDurationMs = ReadInt(
                "Tick duration in milliseconds (100-5000): ",
                100,
                5000)
        };

        Console.WriteLine();
        Console.WriteLine("Configuration Complete");
        Console.WriteLine("----------------------");
        Console.WriteLine($"Regular Floors : {config.FloorCount}");
        Console.WriteLine($"Basements      : {config.BasementFloors}");
        Console.WriteLine($"Elevators      : {config.ElevatorCount}");
        Console.WriteLine($"Capacity       : {config.ElevatorCapacity}");
        Console.WriteLine($"Tick Duration  : {config.TickDurationMs} ms");
        Console.WriteLine();

        Console.Write("Press ENTER to start...");
        Console.ReadLine();

        return config;
    }

    private static int ReadInt(
        string prompt,
        int min,
        int max)
    {
        while (true)
        {
            Console.Write(prompt);

            var input = Console.ReadLine();

            if (!int.TryParse(input, out int value))
            {
                Console.WriteLine(
                    $"Invalid input. Enter a number between {min} and {max}.");
                continue;
            }

            if (value < min || value > max)
            {
                Console.WriteLine(
                    $"Value must be between {min} and {max}.");
                continue;
            }

            return value;
        }
    }
}