using ElevatorChallenge.Domain.Enums;
using System.ComponentModel;

namespace ElevatorChallenge.Domain.Models
{


    public class Elevator
    {
    //public event EventHandler<MoveElevatorEventArgs>? ElevatorMoved;

        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public List<Request> Requests { get; set; } = new List<Request>();
        public List<Destination> Destinations { get; set; } = new List<Destination>();
        public int CurrentWeight { get; set; } = 0;
        public ElevatorStatus ElevatorStatus { get; set; }
        public Direction Direction { get; set; } = Direction.NONE;
        public DoorStatus DoorStatus { get; set; } = DoorStatus.DoorsClosed;


        public const int TOP_FLOOR = 9;
        public const int MAX_WEIGHT = 5;


        //public void OnElevatorMoved(MoveElevatorEventArgs moveEventArgs)
        //{
        //    ElevatorMoved?.Invoke(this, moveEventArgs);
        //}
    }


}
