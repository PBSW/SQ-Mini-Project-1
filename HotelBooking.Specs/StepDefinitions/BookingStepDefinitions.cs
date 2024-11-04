using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Reqnroll;
using Xunit;
using Moq;

namespace HotelBooking.Specs.StepDefinitions;

[Binding]
public sealed class BookingStepDefinitions
{
    public Mock<IRepository<Room>> _roomRepo;
    public Mock<IRepository<Booking>> _bookRepo;
    public IBookingManager _bookingManager;
    public bool _bookingResult;

    public BookingStepDefinitions()
    {
        _bookRepo = new Mock<IRepository<Booking>>();
        _roomRepo = new Mock<IRepository<Room>>();
        
        _bookingManager = new BookingManager(_bookRepo.Object, _roomRepo.Object);
        
        RoomSetup();
    }
    
    // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

    [Given("there are available rooms {int} days from now till {int} days from now")]
    public void GivenThereAreAvailableRoomsOnDate(int daysFromNow, int daysToNow)
    {
        _bookingManager.FindAvailableRoom(DateTime.Now.AddDays(daysFromNow), DateTime.Now.AddDays(daysToNow));
    }

    [When("the user attempts to book a room")]
    public void WhenTheUserAttemptsToBookARoom()
    {
        var newBooking = new Booking
        {
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(5),
            CustomerId = 1
        };

        _bookingResult = _bookingManager.CreateBooking(newBooking);
    }

    [Then("the booking is created successfully")]
    public void ThenBookingIsSuccessful()
    {
        Assert.True(_bookingResult);
    }

    private void RoomSetup()
    {
        var rooms = new List<Room>
        {
            new Room { Id = 1, Description = "Single Room" },
            new Room { Id = 2, Description = "Double Room" }
        };
        
        _roomRepo.Setup(repo => repo.GetAll()).Returns(rooms);
    }

    private void BookingsSetup()
    {
        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3), IsActive = true,
                CustomerId = 1, RoomId = 1
            },
            new Booking
            {
                Id = 2, StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(6), IsActive = true,
                CustomerId = 2, RoomId = 2
            }
        };
        
            _bookRepo.Setup(repo => repo.GetAll()).Returns(bookings);
        }
}