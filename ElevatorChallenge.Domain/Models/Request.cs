using ElevatorChallenge.Domain.Enums;

namespace ElevatorChallenge.Domain.Models
{
    public class Request
    {
        public int CurrentFloor { get; set; }
        public Direction Direction { get; set; }
        public int DestinationFloor { get; set; }
    }

    
}
