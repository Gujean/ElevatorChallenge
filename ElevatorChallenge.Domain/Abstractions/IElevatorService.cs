using ElevatorChallenge.Domain.Models;

namespace ElevatorChallenge.Domain.Abstractions
{
    public interface IElevatorService
    {
        Elevator PickElevator();
    }
}
