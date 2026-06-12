using ElevatorSimulator.Core.Enums;

namespace ElevatorSimulator.Core.Interfaces;

public interface ISimulationEngine
{
    SimulationState CurrentState { get; }

    void Play();
    void Pause();
    void Resume();
    void Stop();
    void StepOneTick();
    void IncreaseSpeed();
    void DecreaseSpeed();
}
