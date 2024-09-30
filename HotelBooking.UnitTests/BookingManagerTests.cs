using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private BookingManager bookingManager;
        private Mock<IRepository<Booking>> bookingRepository;
        private Mock<IRepository<Room>> roomRepository;

        // Test setup method
        public BookingManagerTests()
        {
            // Initialize the mock repositories
            bookingRepository = new Mock<IRepository<Booking>>();
            roomRepository = new Mock<IRepository<Room>>();

            // Inject the mock repositories into the BookingManager
            bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);
        }

        // Test for CreateBooking method using data-driven tests
        [Theory]
        [Trait("Category", "CreateBooking")]
        [InlineData(1, 3, 4, true)]  // Booking can be created, no overlap with existing bookings
        [InlineData(1, 5, 6, true)]  // Booking can be created, no overlap
        [InlineData(1, 1, 1, false)] // Booking cannot be created (overlap with existing booking)
        public void CreateBooking_Test(int customerId, int startOffset, int endOffset, bool expectedResult)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "Single Room" },
                new Room { Id = 2, Description = "Double Room" }
            };

            var bookings = new List<Booking>
            {
                new Booking { Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true, CustomerId = 1, RoomId = 1 }
            };

            // Mock repository behavior
            roomRepository.Setup(repo => repo.GetAll()).Returns(rooms);
            bookingRepository.Setup(repo => repo.GetAll()).Returns(bookings);

            var newBooking = new Booking
            {
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate,
            };

            // Act
            bool result = bookingManager.CreateBooking(newBooking);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        // Test for FindAvailableRoom method using data-driven tests
        [Theory]
        [Trait("Category", "FindAvailableRoom")]
        [InlineData(1, 3, -1)]  // Room not available (Room 1 booked)
        [InlineData(3, 4, 1)]   // Room available (Room 1 free)
        [InlineData(5, 6, 2)]   // Room available (Room 2 free)
        public void FindAvailableRoom_Test(int startOffset, int endOffset, int expectedRoomId)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "Single Room" },
                new Room { Id = 2, Description = "Double Room" }
            };

            var bookings = new List<Booking>
            {
                new Booking { Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true, CustomerId = 1, RoomId = 1 }
            };

            // Mock repository behavior
            roomRepository.Setup(repo => repo.GetAll()).Returns(rooms);
            bookingRepository.Setup(repo => repo.GetAll()).Returns(bookings);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(expectedRoomId, roomId);
        }

        // Test for GetFullyOccupiedDates method using data-driven tests
        [Theory]
        [Trait("Category", "FullyOccupiedDates")]
        [InlineData(1, 5, 1)]  // One fully occupied day (Room 1 booked)
        [InlineData(1, 3, 0)]  // No fully occupied days in this range
        [InlineData(2, 6, 1)]  // One fully occupied day
        public void GetFullyOccupiedDates_Test(int startOffset, int endOffset, int expectedFullyOccupiedCount)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "Single Room" },
                new Room { Id = 2, Description = "Double Room" }
            };

            var bookings = new List<Booking>
            {
                new Booking { Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true, CustomerId = 1, RoomId = 1 },
                new Booking { Id = 2, StartDate = DateTime.Today.AddDays(4), EndDate = DateTime.Today.AddDays(5), IsActive = true, CustomerId = 2, RoomId = 2 }
            };

            // Mock repository behavior
            roomRepository.Setup(repo => repo.GetAll()).Returns(rooms);
            bookingRepository.Setup(repo => repo.GetAll()).Returns(bookings);

            // Act
            List<DateTime> fullyOccupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectedFullyOccupiedCount, fullyOccupiedDates.Count);
        }
    }
}
