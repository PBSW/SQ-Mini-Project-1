using System;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelBooking.IntegrationTests
{
    public class BookingManagerTests : IDisposable
    {
        // This test class uses a separate Sqlite in-memory database. While the
        // .NET Core built-in in-memory database is not a relational database,
        // Sqlite in-memory database is. This means that an exception is thrown,
        // if a database constraint is violated, and this is a desirable behavior
        // when testing.

        SqliteConnection connection;
        BookingManager bookingManager;
        BookingRepository bookingRepos;
        RoomRepository roomRepos;

        public BookingManagerTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");

            // In-memory database only exists while the connection is open
            connection.Open();

            // Initialize test database
            var options = new DbContextOptionsBuilder<HotelBookingContext>()
                .UseSqlite(connection).Options;
            var dbContext = new HotelBookingContext(options);
            IDbInitializer dbInitializer = new DbInitializer();
            dbInitializer.Initialize(dbContext);

            // Create repositories and BookingManager
            bookingRepos = new BookingRepository(dbContext);
            roomRepos = new RoomRepository(dbContext);
            bookingManager = new BookingManager(bookingRepos, roomRepos);
        }

        public void Dispose()
        {
            // This will delete the in-memory database
            connection.Close();
        }

        [Fact]
        public void FindAvailableRoom_RoomNotAvailable_RoomIdIsMinusOne()
        {
            // Act
            var roomId = bookingManager.FindAvailableRoom(DateTime.Today.AddDays(8), DateTime.Today.AddDays(8));
            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public void CreateBooking_AvailableRoom_ReturnsTrueAndAddsBooking()
        {
            // Arrange
            var booking = new Booking
            {
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(25),
                EndDate = DateTime.Today.AddDays(30)
            };

            // Act
            var result = bookingManager.CreateBooking(booking);

            // Assert
            Assert.True(result);
            Assert.Equal(4, bookingRepos.GetAll().Count()); // Expect 4 bookings (3 existing + 1 new)
            Assert.Contains(bookingRepos.GetAll(), b => b.CustomerId == 1 && b.StartDate == booking.StartDate);
        }

        [Fact]
        public void CreateBooking_NoAvailableRoom_ReturnsFalse()
        {
            // Arrange
            var booking = new Booking
            {
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(4),
                EndDate = DateTime.Today.AddDays(6)
            };

            // Act
            var result = bookingManager.CreateBooking(booking);

            // Assert
            Assert.False(result); // No room should be available for this date range
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsRoomId()
        {
            // Act
            var availableRoomId =
                bookingManager.FindAvailableRoom(DateTime.Today.AddDays(25), DateTime.Today.AddDays(30));

            // Assert
            Assert.InRange(availableRoomId, 1, 3); // Expect one of the room IDs to be available
        }

        [Fact]
        public void FindAvailableRoom_NoRoomAvailable_ReturnsMinusOne()
        {
            // Act
            var availableRoomId =
                bookingManager.FindAvailableRoom(DateTime.Today.AddDays(4), DateTime.Today.AddDays(14));

            // Assert
            Assert.Equal(-1, availableRoomId); // No rooms should be available in this range (all booked)
        }

        [Fact]
        public void GetFullyOccupiedDates_AllRoomsOccupied_ReturnsOccupiedDates()
        {
            // Act
            var fullyOccupiedDates =
                bookingManager.GetFullyOccupiedDates(DateTime.Today.AddDays(4), DateTime.Today.AddDays(14));

            // Assert
            Assert.NotEmpty(fullyOccupiedDates);
            Assert.Equal(11, fullyOccupiedDates.Count); // Since all rooms are booked during this period
        }

        [Fact]
        public void GetFullyOccupiedDates_PartiallyOccupied_ReturnsEmptyList()
        {
            // Act
            var fullyOccupiedDates =
                bookingManager.GetFullyOccupiedDates(DateTime.Today.AddDays(25), DateTime.Today.AddDays(40));

            // Assert
            Assert.Empty(fullyOccupiedDates); // No fully occupied dates in this period, as rooms are available
        }
    }
}