using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Domain.Models;

namespace ElevatorChallenge.Infrastructure
{
    public class ElevatorService //: IElevatorService
    {
        public Elevator PickBestElevator(Request request, List<Elevator> elevators)
        {
            var closestStaticElevator = elevators
                    .Where(elevator => elevator.ElevatorStatus == ElevatorStatus.Static)
                    .OrderBy(elev =>
                    request.CurrentFloor - elev.CurrentFloor < 0 ? Math.Abs(request.CurrentFloor - elev.CurrentFloor)
                    : request.CurrentFloor - elev.CurrentFloor)
                    .FirstOrDefault();

            if (closestStaticElevator != null && closestStaticElevator.CurrentFloor == request.CurrentFloor)
            {
                return closestStaticElevator;
            }

            var elevatorsGoingSameDirectionOrStatic = ExcludeElevatorsGoingOppositeDirection(elevators, request.Direction);
            var elevatorsGoingSameDirectionOrStaticAndCanPickTheRequestOnTheGo =
                ExcludeElevatorsThatAlreadyPassedByTheRequestedFloor(elevatorsGoingSameDirectionOrStatic, request.CurrentFloor);
            var futureWeightsForElevators =
                CalculateFutureWeightUntillRequestedFloor(elevatorsGoingSameDirectionOrStaticAndCanPickTheRequestOnTheGo, request.CurrentFloor);

            foreach (var elevator in elevatorsGoingSameDirectionOrStaticAndCanPickTheRequestOnTheGo)
            {
                if (futureWeightsForElevators[elevator] == 5)
                {
                    elevatorsGoingSameDirectionOrStaticAndCanPickTheRequestOnTheGo.Remove(elevator);
                }
            }

            var closestMovingElevator = SelectClosestElevatorToRequest(
                elevatorsGoingSameDirectionOrStaticAndCanPickTheRequestOnTheGo, request.CurrentFloor);

            var distanceFromClosestStaticElevatorToRequestFloor = request.CurrentFloor - closestStaticElevator.CurrentFloor < 0
                ? Math.Abs(request.CurrentFloor - closestStaticElevator.CurrentFloor)
                    : request.CurrentFloor - closestStaticElevator.CurrentFloor;

            int distanceFromClosestMovingElevatorToRequestFloor = 0;
            if (closestMovingElevator != null)
            {
                distanceFromClosestMovingElevatorToRequestFloor = request.CurrentFloor - closestMovingElevator.CurrentFloor < 0
                ? Math.Abs(request.CurrentFloor - closestMovingElevator.CurrentFloor)
                    : request.CurrentFloor - closestMovingElevator.CurrentFloor;
            }


            if (distanceFromClosestStaticElevatorToRequestFloor == distanceFromClosestMovingElevatorToRequestFloor)
            {
                return closestMovingElevator;
            }
            else
            {
                return closestStaticElevator;
            }


            return elevators.FirstOrDefault();
        }

        private List<Elevator> ExcludeElevatorsThatAlreadyPassedByTheRequestedFloor(List<Elevator> elevators, int requestedFloor)
        {
            foreach (var elevator in elevators)
            {
                if (elevator.CurrentFloor > requestedFloor && elevator.Direction == Direction.UP)
                {
                    elevators.Remove(elevator);
                    continue;
                }

                if (elevator.CurrentFloor < requestedFloor && elevator.Direction == Direction.DOWN)
                {
                    elevators.Remove(elevator);
                }
            }

            return elevators;
        }


        private List<Elevator> ExcludeElevatorsGoingOppositeDirection(List<Elevator> elevators, Direction direction)
        {
            var elevatorsGoingSameDirection = new List<Elevator>();

            foreach (var elevator in elevators)
            {
                if (elevator.Direction == direction)
                {
                    elevatorsGoingSameDirection.Add(elevator);
                }
            }

            return elevatorsGoingSameDirection;
        }


        private Elevator SelectClosestElevatorToRequest(List<Elevator> elevators, int requestedFloor)
        {
            if (elevators.Count == 1)
            {
                return elevators[0];
            }
            else
            {
                Dictionary<Elevator, int> distances = elevators.ToDictionary(e => e, e => 0);

                foreach (var elevator in elevators)
                {
                    //int distanceToRequestedFloor = 0;
                    //foreach (var request in elevator.Destinations)
                    //{
                    //    if (elevator.CurrentFloor < requestedFloor)
                    //    {
                    //        var lastFloor = elevator.
                    //        distanceToRequestedFloor = requestedFloor - elevator.CurrentFloor ;
                    //    }
                    //    else
                    //    {
                    //        var lastDestinationFloor = elevator.Destinations.Last();
                    //        distanceToRequestedFloor = elevator.CurrentFloor - requestedFloor;
                    //}
                    //}


                    distances[elevator] = Math.Abs(elevator.Requests.Last().CurrentFloor - requestedFloor);
                }

                return distances.OrderBy(x => x.Value).FirstOrDefault().Key;
            }
        }

        public void PostElevatorMoved(Elevator elevator)
        {
            var requests = elevator.Requests.Where(request => request.CurrentFloor == elevator.CurrentFloor).ToList();

            //if there are already people on the elevator you only need to pickup the people going same direction
            //if there is no one on the elevator then the elevator can go UP and then pickup someone who wants to go DOWN
            if(elevator.CurrentWeight > 0)
            {
                requests = requests.Where(req => req.Direction == elevator.Direction).ToList();
            }

            var destinations = elevator.Destinations.Where(dest => dest.DestinationFloor == elevator.CurrentFloor).ToList();

            if (requests.Count() == 0 && destinations.Count() == 0)
            {
                
                return;
            }

            if (destinations.Count() > 0 && destinations.FirstOrDefault().DestinationFloor == elevator.CurrentFloor)
            {
                OpenElevatorDoors(elevator);
                elevator.CurrentWeight -= destinations.Count();
                Console.WriteLine($"{destinations.Count()} people got out of E{elevator.Id} at floor {elevator.CurrentFloor}");
                //simulating people getting out of the elevator 
                Thread.Sleep(1000);

                foreach (var destination in destinations)
                {
                    elevator.Destinations.Remove(destination);
                }
            }

            if (requests.Count() > 0 && requests.FirstOrDefault().CurrentFloor == elevator.CurrentFloor)
            {
                if(elevator.DoorStatus != DoorStatus.DoorsOpened)
                {
                    OpenElevatorDoors(elevator);
                }
                
                elevator.CurrentWeight += requests.Count();
                Console.WriteLine($"{requests.Count()} people got in E{elevator.Id} at floor {elevator.CurrentFloor}");
                //simulating people getting in the elevator 
                Thread.Sleep(1000);

                foreach (var req in requests)
                {
                    elevator.Requests.Remove(req);
                    elevator.Destinations.Add(new Destination()
                    {
                        DestinationFloor = req.DestinationFloor
                    });
                }

            }
            CloseElevatorDoors(elevator);
            return;
        }

        public void MoveElevator(Elevator elevator)
        {
            if (elevator.Requests.Count() > 0 || elevator.Destinations.Count() > 0)
            {
                MoveElevatorToNextFloor(elevator);
                PostElevatorMoved(elevator);
            }
        }

        public void MoveElevatorToNextFloor(Elevator elevator)
        {
            var request = elevator.Requests.FirstOrDefault();
            var destination = elevator.Destinations.FirstOrDefault();

            if (elevator.ElevatorStatus == ElevatorStatus.Static)
            {
                elevator.ElevatorStatus = ElevatorStatus.Moving;
                elevator.DoorStatus = DoorStatus.DoorsClosed;
            }

            if (request != null)
            {
                if (elevator.CurrentFloor < request.CurrentFloor)
                {
                    elevator.Direction = Direction.UP;
                    elevator.CurrentFloor++;
                }
                else if (elevator.CurrentFloor > request.CurrentFloor)
                {
                    elevator.Direction = Direction.DOWN;
                    elevator.CurrentFloor--;
                }
            }
            else if (destination != null)
            {
                if (elevator.CurrentFloor < destination.DestinationFloor)
                {
                    elevator.Direction = Direction.UP;
                    elevator.CurrentFloor++;
                }
                else if (elevator.CurrentFloor > destination.DestinationFloor)
                {
                    elevator.Direction = Direction.DOWN;
                    elevator.CurrentFloor--;
                }

                
            }

            Console.WriteLine($"E{elevator.Id} moved to floor {elevator.CurrentFloor}");

            //simulating the elevator moving a floor
            Thread.Sleep(2000);

            //return elevator;
        }

        public void OpenElevatorDoors(Elevator elevator)
        {
            Console.WriteLine($"E{elevator.Id} opened the doors at floor {elevator.CurrentFloor}");
            elevator.DoorStatus = DoorStatus.DoorsOpened;
        }

        public void CloseElevatorDoors(Elevator elevator)
        {
            Console.WriteLine($"E{elevator.Id} closed the doors at floor {elevator.CurrentFloor}");
            elevator.DoorStatus = DoorStatus.DoorsClosed;
        }

        private Dictionary<Elevator, int> CalculateFutureWeightUntillRequestedFloor(List<Elevator> elevators, int requestedFloor)
        {
            Dictionary<Elevator, int> futureWeights = elevators.ToDictionary(e => e, e => 0);

            foreach (var elevator in elevators)
            {
                if (elevator.ElevatorStatus == ElevatorStatus.Static && elevator.Requests.Count == 0)
                {
                    continue;
                }

                var futureWeight = elevator.CurrentWeight;

                if (elevator.Direction == Direction.UP)
                {
                    for (var floor = elevator.CurrentFloor; floor <= requestedFloor; floor++)
                    {
                        var currentFloorDestinationsReached = elevator.Destinations.
                            Where(dest => dest.DestinationFloor == floor).Count();
                        futureWeight -= currentFloorDestinationsReached;

                        var currentFloorCallsReached = elevator.Requests.
                            Where(dest => dest.CurrentFloor == floor).Count();
                        futureWeight += currentFloorCallsReached;
                    }
                }
                else
                {
                    for (var floor = elevator.CurrentFloor; floor >= requestedFloor; floor--)
                    {
                        var currentFloorDestinationsReached = elevator.Destinations.
                            Where(dest => dest.DestinationFloor == floor).Count();
                        futureWeight -= currentFloorDestinationsReached;

                        var currentFloorCallsReached = elevator.Requests.
                            Where(dest => dest.CurrentFloor == floor).Count();
                        futureWeight += currentFloorCallsReached;
                    }
                }

                futureWeights[elevator] = futureWeight;
            }


            return futureWeights;
        }


        private Dictionary<Elevator, int> CalculateClosestElevatorsToRequestedFloor(List<Elevator> elevators, int requestedFloor)
        {
            Dictionary<Elevator, int> distances = elevators.ToDictionary(e => e, e => 0);

            foreach (var elevator in elevators)
            {
                int distanceToRequestedFloor;

                if (elevator.CurrentFloor < requestedFloor)
                {
                    distanceToRequestedFloor = (elevator.CurrentFloor - requestedFloor) * -1;
                }
                else
                {
                    var lastDestinationFloor = elevator.Destinations.Last();
                    distanceToRequestedFloor = elevator.CurrentFloor - requestedFloor;
                }

                distances[elevator] = distanceToRequestedFloor;
            }


            return distances.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }


        private string FormatELevatorStatusCharacter(ElevatorStatus status, Direction? direction)
        {
            switch (status)
            {
                case ElevatorStatus.Moving: return "MOVING" + direction.ToString();
                case ElevatorStatus.Maintenance: return "Maintenance";
                case ElevatorStatus.Static: return "STATIC";
                default: return "default";
            }
        }

        public void ShowElevatorsStatus(List<Elevator> elevators)
        {
            foreach (var elevator in elevators)
            {
                Console.WriteLine($"E{elevator.Id} CurrentFloor: {elevator.CurrentFloor} " +
                    $"CurrentWeight: {elevator.CurrentWeight}  Status:{FormatELevatorStatusCharacter(elevator.ElevatorStatus, elevator.Direction)}");
            }
        }

    }
}
