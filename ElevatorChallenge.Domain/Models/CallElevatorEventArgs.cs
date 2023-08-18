using ElevatorChallenge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.Domain.Models
{
    public class CallElevatorEventArgs : EventArgs
    {
        public int DestinationFloor { get; set; }
        public int CurrentFloor { get; set; }
        public Direction Direction { get; set; }
    }
}
