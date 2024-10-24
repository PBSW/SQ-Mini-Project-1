# SQ Mini Project

## Part 3

### Requirements
A hotel room can be booked for a period (start date – end date) in the future, if it is not already booked for one or more days during the desired period.

### Equivalence class testing
It is assumed that the user input represents valid dates (this can be assured by providing datetime pickers in the user interface).

Invalid classes:
- Startdate <= today
- Startdate > enddate

### Decision table-based testing

It is assumed that there is a single range of fully occupied dates. This range is represented by an equivalence class denoted occupied (O).

Dates before the fully occupied range is represented by an equivalence class denoted before (B).

Dates after the fully occupied range is represented by an equivalence class denoted after (A).

Startdate is denoted SD.

Enddate is denoted ED.

| Testcase no. 	| 1 	| 2 	| 3 	| 4-5   | 6 	| 7-8 	| 9-10 	|
|--------------	|---	|---	|---	|-----  |------	|------	|-----	|
| SD in        	| B 	| B 	| B 	| B     | O    	| O 	| A   	|
| ED in        	| B 	| O 	| A 	| O     | O    	| A 	| A   	|
| Book room    	| y 	| n 	| n 	| n     | n    	| n 	| y   	|


Values of SD and ED should be tested at limits for each column in the decision table. For example, in the column where “SD in B” and “ED in O” (testcases 4-5), SD should be the day before the start of the fully occupied range, and ED should be the first day in the fully occupied range (testcase 4) and the last day in the fully occupied range (testcase 5).

- **1:** Both dates are before the occupied range (O) -> Success
  - Use a fitting start date, and an end date right up to the occupied range.
- **2:** Start date is before the occupied range (O), but the end date is within -> Fail. 
  - This test should use an enddate in the middle of (O)
- **3:** Start date is before the occupied range (O), and the end date is after. -> Fail
  - The room would also be booked during the occupied range.
- **4-5:** Start date is before the occupied range (O), but the end date is within -> Fail. 
  - This is for edge cases and should use end dates for the start of (O) and the end of (O).
- **6:** Both dates are within the occupied range (O) -> Fail
  - Use start and end values of the occupied range.
- **7-8:** The start date is within the occupied range, but the end date is after -> Fail. 
  - It is partially booked. Test 7 should use a state date value at the start of (O), and test 8 a value and the very end of (O).
- **9-10:** Both dates are after (O) -> Success. 
  - Test 9 should be a short booking as close to the occupied range as possible, test 10 should be a longer booking a while after the occupied range.


