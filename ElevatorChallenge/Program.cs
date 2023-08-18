// See https://aka.ms/new-console-template for more information
using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Domain.Models;
using ElevatorChallenge.Helpers;
using ElevatorChallenge.Infrastructure;
using System.ComponentModel;


var elevators = Seeders.InitializeElevators();

var dispatcher = Seeders.InitializeDispatcher();

var elevatorService = new ElevatorService();
var elevatorMaster = new ElevatorMaster();

dispatcher.ElevatorCalled += (sender, args) =>
{
    var request = new Request()
    {
        CurrentFloor = args.CurrentFloor,
        DestinationFloor = args.DestinationFloor,
        Direction = args.Direction,
    };
    
    var pickedElevator = elevatorService.PickBestElevator(request, elevators);
    Console.WriteLine($"elevator called at floor {args.CurrentFloor}, Elevator E{pickedElevator.Id} chosen");

    if (pickedElevator == null)
    {
        dispatcher.RequestQueue.Enqueue(request);
    }
    else
    {
        var elevator = elevators.Where(x => x.Id == pickedElevator.Id).FirstOrDefault();

        if (elevator != null)
        {
            elevator.Requests.Add(request);

            elevator.Requests.OrderBy(request => request.CurrentFloor);
        }
    }
};

BackgroundWorker workerE1 = new BackgroundWorker();
workerE1.DoWork += (sender, args) =>
{
    elevatorService.MoveElevator(elevators[0]);
};

BackgroundWorker workerE2 = new BackgroundWorker();
workerE1.DoWork += (sender, args) =>
{
    elevatorService.MoveElevator(elevators[1]);
};
BackgroundWorker workerE3 = new BackgroundWorker();
workerE1.DoWork += (sender, args) =>
{
    elevatorService.MoveElevator(elevators[2]);
};
BackgroundWorker workerE4 = new BackgroundWorker();
workerE1.DoWork += (sender, args) =>
{
    elevatorService.MoveElevator(elevators[3]);
};

workerE1.RunWorkerCompleted += (sender, args) =>
{
    
    workerE1.RunWorkerAsync();
};

workerE2.RunWorkerCompleted += (sender, args) =>
{
    
    workerE2.RunWorkerAsync();
};

workerE3.RunWorkerCompleted += (sender, args) =>
{
    
    workerE3.RunWorkerAsync();
};

workerE4.RunWorkerCompleted += (sender, args) =>
{
    
    workerE4.RunWorkerAsync();
};

workerE1.RunWorkerAsync();
workerE2.RunWorkerAsync();
workerE3.RunWorkerAsync();
workerE4.RunWorkerAsync();

while (true)
{
    elevatorService.ShowElevatorsStatus(elevators);

    ProcessQueue();

    Console.Write("Current Floor: ");
    var keyCodeCurrentFloor = Int32.Parse(Console.ReadKey(true).KeyChar.ToString());
    Console.Write(keyCodeCurrentFloor + "  ");
    Console.Write("Destination Floor: ");
    var keyCodeNewDestination = Int32.Parse(Console.ReadKey(true).KeyChar.ToString());
    Console.Write(keyCodeNewDestination + " ");
    Console.Write("Direction: ");
    var keyCodeDirection = Console.ReadKey(true).KeyChar;
    Direction directionKeyCode;
    if (keyCodeDirection.ToString().Equals("\0"))
    {
        directionKeyCode = Direction.UP;
    }
    else
    {
        directionKeyCode = Direction.DOWN;
    }
    Console.WriteLine(directionKeyCode.ToString());

    dispatcher.OnElevatorCalled(new CallElevatorEventArgs()
    {
        DestinationFloor = keyCodeNewDestination,
        Direction = directionKeyCode,
        CurrentFloor = keyCodeCurrentFloor,
    });
}


void ProcessQueue()
{
    Console.WriteLine("Process Queue");

    //here i wanted to do something that handles the request that cannot be proccessed
    //because the weight limit is reached for example
    //until an elevator gets freed or switches directions in order to make the correct decisions on what requests to accept

    //foreach (var request in dispatcher.RequestQueue)
    //{
    //    elevatorService.PickBestElevator(request, elevators);
    //};
}




