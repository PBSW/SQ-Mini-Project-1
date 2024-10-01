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

        public BookingManagerTests()
        {
            // Mock repository setup for isolation
            bookingRepository = new Mock<IRepository<Booking>>();
            roomRepository = new Mock<IRepository<Room>>();
            bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);
        }

        [Theory]
        [InlineData(1, 3, 4, true)] // Room available, booking can be created
        [InlineData(1, 2, 4, false)] // Room not available, overlap with existing booking
        public void CreateBooking_Should_ReturnExpectedResult(int customerId, int startOffset, int endOffset,
            bool expectedResult)
        {
            // Arrange
            SetupRooms();
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true,
                    CustomerId = 1, RoomId = 1
                },
            };
            SetupBookings(bookings);

            var newBooking = new Booking
            {
                CustomerId = customerId,
                StartDate = DateTime.Today.AddDays(startOffset),
                EndDate = DateTime.Today.AddDays(endOffset),
            };

            // Act
            bool result = bookingManager.CreateBooking(newBooking);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(2, 3, 1)] // Room 1 available
        [InlineData(2, 4, -1)] // No room available (overlap)
        public void FindAvailableRoom_Should_ReturnExpectedRoomId(int startOffset, int endOffset, int expectedRoomId)
        {
            // Arrange
            SetupRooms();
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true,
                    CustomerId = 1, RoomId = 1
                }
            };
            SetupBookings(bookings);

            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(expectedRoomId, roomId);
        }

        [Theory]
        [InlineData(-1)] // Past date
        [InlineData(1)] // Invalid: startDate > endDate
        public void FindAvailableRoom_Should_ThrowArgumentException_WhenInvalidDates(int dateOffset)
        {
            // Arrange
            SetupRooms();
            SetupBookings(new List<Booking>());

            DateTime startDate = DateTime.Today.AddDays(dateOffset);
            DateTime endDate = dateOffset < 0 ? startDate.AddDays(1) : startDate.AddDays(-1); // Invalid endDate

            // Act & Assert
            Assert.Throws<ArgumentException>(() => bookingManager.FindAvailableRoom(startDate, endDate));
        }

        [Theory]
        [InlineData(1, 5, 1)] // One fully occupied day (Room 1 booked)
        [InlineData(1, 3, 0)] // No fully occupied days in this range
        public void GetFullyOccupiedDates_Should_ReturnOccupiedDays(int startOffset, int endOffset,
            int expectedOccupiedDays)
        {
            // Arrange
            SetupRooms();
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true,
                    CustomerId = 1, RoomId = 1
                },
                new Booking
                {
                    Id = 2, StartDate = DateTime.Today.AddDays(4), EndDate = DateTime.Today.AddDays(5), IsActive = true,
                    CustomerId = 2, RoomId = 2
                }
            };
            SetupBookings(bookings);

            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            // Act
            List<DateTime> fullyOccupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectedOccupiedDays, fullyOccupiedDates.Count);
        }

        [Fact]
        public void GetFullyOccupiedDates_Should_ReturnZero_WhenNoFullyBookedDays()
        {
            // Arrange
            SetupRooms();
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), IsActive = true,
                    CustomerId = 1, RoomId = 1
                }
            };
            SetupBookings(bookings);

            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // Act
            List<DateTime> occupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Empty(occupiedDates);
        }
        
        
        // Helper Methods
        private void SetupRooms()
        {
            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "Single Room" },
                new Room { Id = 2, Description = "Double Room" }
            };
            roomRepository.Setup(repo => repo.GetAll()).Returns(rooms);
        }

        // Helper method to set up booking repository mock
        private void SetupBookings(List<Booking> bookings)
        {
            bookingRepository.Setup(repo => repo.GetAll()).Returns(bookings);
        }
    }
}
