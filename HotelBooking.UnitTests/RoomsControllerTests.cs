using System;
using System.Collections.Generic;
using System.Linq;
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
            // Act
            Action act = () => controller.Delete(3);
            
            // Assert
            Assert.Throws<InvalidOperationException>(act);

            // Assert against the mock object
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }
        
        [Fact]
        public void Post_ValidRoom_AddsRoom_ReturnsCreatedAtRoute()
        {
            // Arrange
            var newRoom = new Room { Id = 3, Description = "C" };

            // Act
            var result = controller.Post(newRoom) as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(result);
            fakeRoomRepository.Verify(x => x.Add(newRoom), Times.Once);
        }

        [Fact]
        public void Post_NullRoom_ReturnsBadRequest()
        {
            // Act
            var result = controller.Post(null) as BadRequestResult;

            // Assert
            Assert.IsType<BadRequestResult>(result);
            fakeRoomRepository.Verify(x => x.Add(It.IsAny<Room>()), Times.Never);
        }
        
        
        [Fact]
        public void GetAll_EmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            fakeRoomRepository = RepositoryMockFactory.CreateMockRepository<Room>(new List<Room>());
            controller = new RoomsController(fakeRoomRepository.Object);

            // Act
            var result = controller.Get() as List<Room>;

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetById_RoomDoesNotExist_ReturnsNotFound()
        {
            // Act
            var result = controller.Get(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        
        [Fact]
        public void Delete_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = controller.Delete(-1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        
    }
}
