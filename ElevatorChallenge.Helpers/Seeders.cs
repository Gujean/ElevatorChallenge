using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Domain.Models;
using ElevatorChallenge.Infrastructure;

namespace ElevatorChallenge.Helpers
{
    public static class Seeders
    {
        public static List<Elevator> InitializeElevators()
        {
            return new List<Elevator>()
{
    new Elevator(){ Id =1,
        CurrentFloor = 0,
        CurrentWeight = 0,
        ElevatorStatus = ElevatorStatus.Static
    },
    new Elevator(){ Id =2,
        CurrentFloor = 2,
        CurrentWeight = 0,
        ElevatorStatus = ElevatorStatus.Static
    },
    new Elevator(){ Id =3,
        CurrentFloor = 3,
        CurrentWeight = 0,
        ElevatorStatus = ElevatorStatus.Static
    },
    new Elevator(){ Id =4,
        CurrentFloor = 4,
        CurrentWeight = 0,
        ElevatorStatus = ElevatorStatus.Static
    },

};
        }

        public static Dispatcher InitializeDispatcher()
        {
            var dispatcher = new Dispatcher();

            dispatcher.RequestQueue.Enqueue(new Request()
            {
                CurrentFloor = 4,
                Direction = Direction.DOWN
            });
            dispatcher.RequestQueue.Enqueue(new Request()
            {
                CurrentFloor = 4,
                Direction = Direction.DOWN
            });
            dispatcher.RequestQueue.Enqueue(new Request()
            {
                CurrentFloor = 2,
                Direction = Direction.DOWN
            });
            dispatcher.RequestQueue.Enqueue(new Request()
            {
                CurrentFloor = 3,
                Direction = Direction.UP
            });
            dispatcher.RequestQueue.Enqueue(new Request()
            {
                CurrentFloor = 1,
                Direction = Direction.UP
            });

            return dispatcher;
        }

    }
}