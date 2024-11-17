# GetFullyoccupiedDates MCDC Analysis

## Method Analysis

### Method 2: `GetFullyOccupiedDates`

#### Code
```csharp
public List<DateTime> GetFullyOccupiedDates(DateTime startDate, DateTime endDate)
{
    if (startDate > endDate)
        throw new ArgumentException("The start date cannot be later than the end date.");

    List<DateTime> fullyOccupiedDates = new List<DateTime>();
    int noOfRooms = roomRepository.GetAll().Count();
    var bookings = bookingRepository.GetAll();

    if (bookings.Any())
    {
        for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
        {
            var noOfBookings = from b in bookings
                               where b.IsActive && d >= b.StartDate && d <= b.EndDate
                               select b;
            if (noOfBookings.Count() >= noOfRooms)
                fullyOccupiedDates.Add(d);
        }
    }
    return fullyOccupiedDates;
}
```

###  MCDC Analysis

**Outer `if` Statement**:
```
if (startDate > endDate)
    throw new ArgumentException("The start date cannot be later than the end date.");
```
- Condition:
  - `C1`: `startDate > endDate`

| Test Case | C1 (startDate > endDate) | Decision |
|-----------|:------------------------:|:--------:|
| TC1       |          True            |  True    |
| TC2       |          False           |  False   |

**Inner `if` Statement**:

```
if (noOfBookings.Count() >= noOfRooms)
    fullyOccupiedDates.Add(d);
```

- Condition:
    - `C2`: `noOfBookings.Count() >= noOfRooms`

| Test Case | C2 (noOfBookings.Count() >= noOfRooms) | Decision |
|-----------|:--------------------------------------:|:--------:|
| TC3       |                  True                  |  True    |
| TC4       |                 False                  |  False   |

**LINQ**:
```
var noOfBookings = from b in bookings
                   where b.IsActive && d >= b.StartDate && d <= b.EndDate
                   select b;
```

- Condition:
    - `C1`: `b.IsActive`
    - `C2`: `d >= b.StartDate`
    - `C3`: `d <= b.EndDate`

The decision is:
 - `C1` && `C2` && `C3`

| Test Case |           	C1 (b.IsActive)          | C2 (d >= b.StartDate) | C3 (d <= b.EndDate) | Decision (C1 && C2 && C3) |
|:----------|:-------------------------------------:|:---------------------:|:-------------------:|:-------------------------:|
| TC5       |                 	True                 |         True          |   	True           |          	True           |
| TC6       |                	False                |         True          |        	True       |          	False          |
| TC7       |                 	True                 |        	False        |        	True       |          	False          |
| TC8       |                 	True                 |         True          |       	False      |          	False          |

## Test Cases

| Test Case | Description                                                        | Expected Result                  |
|-----------|--------------------------------------------------------------------|----------------------------------|
| TC1       | `startDate > endDate`.                                             | Exception is thrown.             |
| TC2       | `startDate <= endDate`.                                            | Method proceeds to logic.        |
| TC3       | Number of bookings equals or exceeds the total number of rooms.    | Date is counted as occupied.     |
| TC4       | Number of bookings is less than the total number of rooms.         | Date is not counted as occupied. |
| TC5       | A booking is active, and the date is within the booking period.    | Date is counted as occupied.     |
| TC6       | A booking is inactive (`b.IsActive` is false).                     | Date is not counted as occupied. |
| TC7       | The date is before the booking’s start date (`d < b.StartDate`).   | Date is not counted as occupied. |
| TC8       | The date is after the booking’s end date (`d > b.EndDate`).        | Date is not counted as occupied. |