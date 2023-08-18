using ElevatorChallenge.Domain.Enums;
using ElevatorChallenge.Domain.Models;
using ElevatorChallenge.Helpers;
using ElevatorChallenge.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;

namespace ElevatorChallenge.UnitTests
{
    public class Tests
    {
        private ElevatorService _elevatorService;

        [SetUp]
        public void Setup()
        {
            _elevatorService = new ElevatorService();
        }

        [Test]
        public void Should_Return_E4()
        {
            List<Elevator> elevators = Seeders.InitializeElevators();

            var request = new Request()
            {
                CurrentFloor = 5,
                DestinationFloor = 2,
                Direction = Direction.DOWN
            };

            var elevator = _elevatorService.PickBestElevator(request, elevators);

            Assert.AreEqual(elevator.Id,4);
        }
        
    }
}