Feature: Create Booking

    Scenario: Book a room with a valid future date range
        Given there are available rooms 3 days from now till 5 days from now
        When the user attempts to book a room
        Then the booking is created successfully
       
    