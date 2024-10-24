# SQ Mini Project

## Part 3

### Requirements
A hotel room can be booked for a period (start date – end date) in the future, if it is not already booked for one or more days during the desired period.

### Equivalence class testing
It is assumed that the user input represents valid dates (this can be assured by providing datetime pickers in the user interface).

 

Invalid classes

Startdate <= today
Startdate > enddate

### Decision table-based testing

It is assumed that there is a single range of fully occupied dates. This range is represented by an equivalence class denoted occupied (O).

Dates before the fully occupied range is represented by an equivalence class denoted before (B).

Dates after the fully occupied range is represented by an equivalence class denoted after (A).

Startdate is denoted SD.

Enddate is denoted ED.

| Testcase no. 	| 1 	| 2 	| 3 	| 4-5 	| 6-7 	| 8-10 	|
|--------------	|---	|---	|---	|-----	|-----	|------	|
| SD in        	| - 	| - 	| - 	| B   	| -   	| -    	|
| ED in        	| - 	| - 	| - 	| O   	| -   	| -    	|
| Book room    	| - 	| - 	| - 	| n   	| -   	| -    	|


Values of SD and ED should be tested at limits for each column in the decision table. For example, in the column where “SD in B” and “ED in O” (testcases 4-5), SD should be the day before the start of the fully occupied range, and ED should be the first day in the fully occupied range (testcase 4) and the last day in the fully occupied range (testcase 5).