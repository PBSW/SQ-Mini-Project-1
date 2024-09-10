using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class RoomsControllerTests
    {
        private RoomsController controller;
        private Mock<IRepository<Room>> fakeRoomRepository;

        public RoomsControllerTests()
        {
            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            // Create fake RoomRepository. 
            fakeRoomRepository = RepositoryMockFactory.CreateMockRepository<Room>(rooms);

            // Create RoomsController
            controller = new RoomsController(fakeRoomRepository.Object);
        }

        [Fact]
        public void GetAll_ReturnsListWithCorrectNumberOfRooms()
        {
            // Act
            var result = controller.Get() as List<Room>;
            var noOfRooms = result.Count;

            // Assert
            Assert.Equal(2, noOfRooms);
        }

        [Fact]
        public void GetById_RoomExists_ReturnsIActionResultWithRoom()
        {
            // Act
            var result = controller.Get(2) as ObjectResult;
            var room = result.Value as Room;
            var roomId = room.Id;

            // Assert
            Assert.InRange<int>(roomId, 1, 2);
        }

        [Fact]
        public void Delete_WhenIdIsLargerThanZero_RemoveIsCalled()
        {
            // Act
            controller.Delete(1);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(1), Times.Once);
        }

        [Fact]
        public void Delete_WhenIdIsLessThanOne_RemoveIsNotCalled()
        {
            // Act
            controller.Delete(0);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Delete_WhenIdIsLargerThanTwo_RemoveThrowsException()
        {
            // Instruct the fake Remove method to throw an InvalidOperationException, if a room id that
            // does not exist in the repository is passed as a parameter. This behavior corresponds to
            // the behavior of the real repoository's Remove method.
            fakeRoomRepository.Setup(x =>
                    x.Remove(It.Is<int>(id => id < 1 || id > 2))).
                    Throws<InvalidOperationException>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => controller.Delete(3));

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }
    }
}
