Feature: Create Booking

    Scenario: Book a room with a valid future date range
        Given there are available rooms on given date
        When the user attempts to book a room
        Then the booking is created successfully
       
    