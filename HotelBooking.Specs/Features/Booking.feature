Feature: Create Booking
# Room 1 is occupied from tommorow (1 day from now) to 5 days from now.
# Room 2 is occupied from 3 days from now to 6 days from now.
# The range in which both are occupied is from 3 days from now to 5 days from now.

    Scenario: Book a room with a valid future date range
        Given that there are available rooms
        When the user attempts to book a room from 3 days from now to 5 days from now
        Then the booking is created successfully

# Testcase 1

    Scenario: Book a room with before the occupied date range
        Given that there is a period of no available rooms
        When the user attempts to book a room from 1 days from now to 2 days from now
        Then the booking is created successfully

# Testcase 2, 3, 4, 5

    Scenario: Attempt to book an occupied room
        Given that there is a period of no available rooms
        When the user attempts to book a room from <StartDate> to <EndDate>
        Then the booking is not created

    Examples:
      | StartDate       | EndDate         |
      | 2 days from now | 6 days from now |
      | 4 days from now | 6 days from now |
      | 2 days from now | 4 days from now |
      | 2 days from now | 5 days from now |

# Invalid class test

    Scenario: Attempt to book a room with end date before start date
        Given that there are available rooms
        When the user attempts to book a room from 5 days from now to 2 days from now
        Then an ArgumentException is thrown


# Testcase 6, 7

    Scenario: Attempt to book a room with overlapping dates
        Given that there is a period of no available rooms
        When the user attempts to book a room from <StartDate> to <EndDate>
        Then the booking is not created

    Examples:
      | StartDate       | EndDate         |
      | 3 days from now | 5 days from now |
      | 4 days from now | 4 days from now |

# Testcase 8, 9, 10

    Scenario: Book a room with dates after the occupied range
        Given that there is a period of no available rooms
        When the user attempts to book a room from <StartDate> to <EndDate>
        Then the booking is created successfully

    Examples:
      | StartDate        | EndDate          |
      | 6 days from now  | 6 days from now  |
      | 6 days from now  | 12 days from now |
      | 12 days from now | 24 days from now |