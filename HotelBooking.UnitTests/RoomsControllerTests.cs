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
                new Room { Id=1, Description="Single Room" },
                new Room { Id=2, Description="Double Room" },
                new Room { Id=3, Description="Suite" },
                new Room { Id=4, Description="Deluxe Room" },
                new Room { Id=5, Description="Penthouse" },
            };

            // Create fake RoomRepository. 
            fakeRoomRepository = RepositoryMockFactory.CreateMockRepository<Room>(rooms);

            // Create RoomsController
            controller = new RoomsController(fakeRoomRepository.Object);
        }

        [Theory]
        [InlineData(5)]
        public void GetAll_ReturnsListWithCorrectNumberOfRooms(int expectedRoomCount)
        {
            // Act
            var result = controller.Get() as List<Room>;
            var noOfRooms = result.Count;

            // Assert
            Assert.Equal(expectedRoomCount, noOfRooms);
        }

        [Theory]
        [InlineData(1, "Single Room")]
        [InlineData(2, "Double Room")]
        [InlineData(3, "Suite")]
        [InlineData(4, "Deluxe Room")]
        [InlineData(5, "Penthouse")]
        public void GetById_RoomExists_ReturnsIActionResultWithRoom(int roomId, string expectedDescription)
        {
            // Act
            var result = controller.Get(roomId) as ObjectResult;
            var room = result?.Value as Room;

            // Assert
            Assert.Equal(roomId, room?.Id);
            Assert.Equal(expectedDescription, room?.Description);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(5, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void Delete_VerifyRemoveCalledBasedOnId(int roomId, bool shouldCallRemove)
        {
            // Act
            controller.Delete(roomId);

            // Assert
            if (shouldCallRemove)
            {
                fakeRoomRepository.Verify(x => x.Remove(roomId), Times.Once);
            }
            else
            {
                fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
            }
        }

        [Theory]
        [InlineData(6)]
        [InlineData(10)]
        public void Delete_WhenIdIsLargerThanExisting_RemoveThrowsException(int roomId)
        {
            // Act
            Action act = () => controller.Delete(roomId);
            
            // Assert
            Assert.Throws<InvalidOperationException>(act);
            fakeRoomRepository.Verify(x => x.Remove(It.IsAny<int>()));
        }

        [Theory]
        [InlineData(6, "VIP Room")]
        [InlineData(7, "Presidential Suite")]
        public void Post_ValidRoom_AddsRoom_ReturnsCreatedAtRoute(int roomId, string description)
        {
            // Arrange
            var newRoom = new Room { Id = roomId, Description = description };

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

        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        public void GetById_RoomDoesNotExist_ReturnsNotFound(int roomId)
        {
            // Act
            var result = controller.Get(roomId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        public void Delete_InvalidId_ReturnsBadRequest(int roomId)
        {
            // Act
            var result = controller.Delete(roomId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
