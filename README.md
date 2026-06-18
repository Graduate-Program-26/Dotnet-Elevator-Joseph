# Dotnet-Elevator-Joseph
# Elevator Simulator

This is a elevator simulation project built using .NET. The system simulates how elevators move between floors, handle requests, and decide which elevator should respond to a request.
---

## Features

- Simulates multiple elevators in a building
- Handles floor requests (up and down calls)
- Internal elevator requests
- Basic elevator dispatching logic
- Step by step simulation loop
- Console based user interface
- Structured using multiple layers (Core, Simulation, Infrastructure, UI)

---

## Core Idea

The system works by taking user requests and assigning them to the best available elevator. Each elevator moves step by step until it reaches the requested floor. The simulation runs in a loop and updates the state of all elevators continuously.

---

## Project Structure
ElevatorSimulator/
│
├── ElevatorSimulator.Core/ # domain models and interfaces
├── ElevatorSimulator.Simulation/ # simulation logic and engine
├── ElevatorSimulator.Infrastructure/# service implementations
├── ElevatorSimulator.ConsoleUI/ # console user interface
└── ElevatorSimulator.Tests/ # unit tests 

---

## Packages
- Serilog
- Razor.Console https://razorconsole.github.io/RazorConsole/

---
## How to run
- Clone Repo
- run "dotnet restore"
- run "dotnet Build"
- run "cd ElevatorSimulator.ConsoleUI/"
- run "dotnet run"
- use "tab" to navigate forward and "shift+Tab" to navigate backward, "Enter" to click focused button

---

### Requirements

 - C#10.0