using ElevatorChallenge.Domain.Models;

namespace ElevatorChallenge.Infrastructure
{
    public class Dispatcher
    {
        public Queue<Request> RequestQueue { get; set; } = new Queue<Request>();
        public event EventHandler<CallElevatorEventArgs>? ElevatorCalled;
        public void OnElevatorCalled(CallElevatorEventArgs moveEventArgs)
        {
            ElevatorCalled?.Invoke(this, moveEventArgs);
        }
    }
}
