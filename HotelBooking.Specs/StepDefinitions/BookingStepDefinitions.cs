using System;
using System.Collections.Generic;
using FluentAssertions;
using HotelBooking.Core;
using Reqnroll;
using Xunit;
using Moq;

namespace HotelBooking.Specs.StepDefinitions;

[Binding]
public sealed class BookingStepDefinitions
{
    private Mock<IRepository<Room>> _roomRepo;
    private Mock<IRepository<Booking>> _bookRepo;
    private IBookingManager _bookingManager;
    private bool _bookingResult;
    private Action _bookingAction;

    public BookingStepDefinitions()
    {
        _bookRepo = new Mock<IRepository<Booking>>();
        _roomRepo = new Mock<IRepository<Room>>();
        
        _bookingManager = new BookingManager(_bookRepo.Object, _roomRepo.Object);
        
        RoomSetup();
    }
    
    [Given("there are available rooms")]
    public void GivenThereAreAvailableRooms()
    {
        _bookRepo.Setup(repo => repo.GetAll()).Returns(new List<Booking>());
    }
    
    [Given("there are no available rooms")]
    public void GivenThereAreNoAvailableRooms()
    {
        BookingsSetup();
    }

    [When("the user attempts to book a room from {int} days from now to {int} days from now")]
    public void WhenTheUserAttemptsToBookARoom(int startOffset, int endOffset)
    {
        var newBooking = new Booking
        {
            StartDate = DateTime.Now.AddDays(startOffset),
            EndDate = DateTime.Now.AddDays(endOffset),
            CustomerId = 1
        };
        
        _bookingAction = () => _bookingManager.CreateBooking(newBooking);
        
        if (startOffset <= endOffset) 
        {
            _bookingResult = _bookingManager.CreateBooking(newBooking);
        }
    }

    [Then("the booking is created successfully")]
    public void ThenBookingIsSuccessful()
    {
        Assert.True(_bookingResult);
    }
    
    [Then("the booking is not created")]
    public void ThenBookingIsNotCreated()
    {
        Assert.False(_bookingResult);
    }

    [Then("an ArgumentException is thrown")]
    public void ThenAnArgumentExceptionIsThrown()
    {
        _bookingAction.Should().Throw<ArgumentException>("the end date is before the start date, which is invalid");
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
                Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(5), IsActive = true,
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
