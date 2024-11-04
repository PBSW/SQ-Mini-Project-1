Feature: Create Booking

    Scenario: Book a room with a valid future date range
        Given there are available rooms
        When the user attempts to book a room from 3 days from now to 5 days from now
        Then the booking is created successfully

    Scenario: Attempt to book an occupied room
        Given there are no available rooms
        When the user attempts to book a room from 1 days from now to 4 days from now
        Then the booking is not created

    Scenario: Attempt to book a room with end date before start date
        Given there are available rooms
        When the user attempts to book a room from 5 days from now to 2 days from now
        Then an ArgumentException is thrown

    Scenario: Attempt to book a room with overlapping dates
        Given there are no available rooms
        When the user attempts to book a room from 3 days from now to 6 days from now
        Then the booking is not created

    Scenario: Book a room with dates outside the occupied range
        Given there are no available rooms
        When the user attempts to book a room from 7 days from now to 10 days from now
        Then the booking is created successfully