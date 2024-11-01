# SQ Mini Project

## Part 3

### Requirements
A hotel room can be booked for a period (start date – end date) in the future, if it is not already booked for one or more days during the desired period.

### Equivalence class testing
It is assumed that the user input represents valid dates (this can be assured by providing datetime pickers in the user interface).

Given your assumptions and constraints on valid dates, we can refine the equivalence classes as follows:

#### Invalid Classes

1. **Start Date on or Before Today**  
   - **Description**: The `StartDate` is today or a date in the past, which should not be allowed as bookings must be made for future dates.
   - **Input Example**: `StartDate = DateTime.Today`, `EndDate = DateTime.Today.AddDays(3)`
   - **Expected Result**: Throws `ArgumentException` with a message indicating the start date must be in the future.

2. **Start Date After End Date**  
   - **Description**: The `StartDate` is after the `EndDate`, which is invalid as the booking duration cannot be negative.
   - **Input Example**: `StartDate = DateTime.Today.AddDays(10)`, `EndDate = DateTime.Today.AddDays(5)`
   - **Expected Result**: Throws `ArgumentException` with a message indicating the start date must be before the end date.

#### Valid Classes

1. **Valid Booking Dates (Start Date Before End Date)**
   - **Description**: The `StartDate` is a future date and comes before the `EndDate`, allowing the system to proceed with room availability checks.
   - **Subclasses**:
     - **Room Available**  
       - **Input Example**: `StartDate = DateTime.Today.AddDays(2)`, `EndDate = DateTime.Today.AddDays(5)`
       - **Expected Result**: Returns `true`, indicating a booking was successfully created, and the `IsActive` flag is set to `true`.
     - **Room Unavailable**  
       - **Input Example**: `StartDate = DateTime.Today.AddDays(2)`, `EndDate = DateTime.Today.AddDays(5)`, but all rooms are booked during this period.
       - **Expected Result**: Returns `false`, indicating no room was available for the specified period, and no booking is created.

2. **Edge Case: Single Day Booking**
   - **Description**: The `StartDate` equals the `EndDate`, representing a booking for a single day.
   - **Input Example**: `StartDate = DateTime.Today.AddDays(3)`, `EndDate = DateTime.Today.AddDays(3)`
   - **Expected Result**: Returns `true` if a room is available for the day; otherwise, returns `false`.

3. **Boundary Date Non-Overlapping Booking**
   - **Description**: A booking request where the new `StartDate` or `EndDate` falls exactly on the `EndDate` or `StartDate` of an existing booking in the same room, but does not overlap.
   - **Input Example**: 
     - Existing Booking: `StartDate = DateTime.Today.AddDays(2)`, `EndDate = DateTime.Today.AddDays(4)`
     - New Booking Request: `StartDate = DateTime.Today.AddDays(4)`, `EndDate = DateTime.Today.AddDays(6)`
   - **Expected Result**: Returns `true`, indicating the booking was successfully created if a room is available on these boundaries.

Aim for testing with Weak Robust Equivalence Class Testing: 
- Like weak normal equivalence, weak robust testing too tests one variable from each equivalence class. However, unlike the former method, it is also focused on testing test cases for invalid values.


### Decision table-based testing

It is assumed that there is a single range of fully occupied dates. This range is represented by an equivalence class denoted occupied (O).

Dates before the fully occupied range is represented by an equivalence class denoted before (B).

Dates after the fully occupied range is represented by an equivalence class denoted after (A).

Startdate is denoted SD.

Enddate is denoted ED.

| Testcase no. 	| 1 	| 2 	| 3 	| 4-5   | 6-7 	| 8-10 	|
|--------------	|---	|---	|---	|-----  |------	|-----	|
| SD in        	| B 	| B 	| O 	| B     | O 	  | A   	|
| ED in        	| B 	| A 	| A 	| O     | O 	  | A   	|
| Book room    	| y 	| n 	| n 	| n     | n 	  | y   	|


Values of SD and ED should be tested at limits for each column in the decision table. For example, in the column where “SD in B” and “ED in O” (testcases 4-5), SD should be the day before the start of the fully occupied range, and ED should be the first day in the fully occupied range (testcase 4) and the last day in the fully occupied range (testcase 5).

- **1:** Both dates are before the occupied range (O) -> Success
  - Use a fitting start date, and an end date right up to the occupied range.
- **2:** Start date is before the occupied range (O), but the end date is after -> Fail. 
  - This test should use an dates just before and after (O)
- **3:** Start date is within the occupied range (O), and the end date is after. -> Fail
  - The room would also be booked during the occupied range.
- **4-5:** Start date is before the occupied range (O), but the end date is within -> Fail. 
  - This is for edge cases and should use end dates for the start of (O) and the end of (O).
- **6-7:** The start date and the end date are both  within the occupied range -> Fail. 
  - It is partially booked. Test 6 should use a state date value at the start of (O) and end of (O), and test 8 values on a single day within (O).
- **8-10:** Both dates are after (O) -> Success. 
  - Test 8 should be a single day booking after the occupied range.
  - Test 9 should be a short booking as close to the occupied range as possible.
  - test 10 should be a longer booking a while after the occupied range.


