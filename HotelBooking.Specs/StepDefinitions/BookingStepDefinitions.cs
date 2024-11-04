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
    }
    
    // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

    [Given("there are available rooms on given date")]
    public void GivenThereAreAvailableRoomsOnDate()
    {
        RoomSetup();
        _bookingManager.FindAvailableRoom(DateTime.Now.AddDays(2), DateTime.Now.AddDays(5));
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
}