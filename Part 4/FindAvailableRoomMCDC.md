# FindAvailableRoom MCDC Analysis

## Method: `FindAvailableRoom`

```csharp
1. public int FindAvailableRoom(DateTime startDate, DateTime endDate)
2. {
3. if (startDate <= DateTime.Today || startDate > endDate)
4. throw new ArgumentException("The start date cannot be in the past or later than the end date.");
5. 
6.    var activeBookings = bookingRepository.GetAll().Where(b => b.IsActive);
7.    foreach (var room in roomRepository.GetAll())
8.    {
9.         var activeBookingsForCurrentRoom = activeBookings.Where(b => b.RoomId == room.Id);
10.        if (activeBookingsForCurrentRoom.All(b => startDate < b.StartDate &&
11.            endDate < b.StartDate || startDate > b.EndDate && endDate > b.EndDate))
12.        {
13.            return room.Id;
14.        }
15.    }
16.    return -1;
17. }
```
MCDC Analysis

**Outer if Statement**:
```
if (startDate <= DateTime.Today || startDate > endDate)
    throw new ArgumentException("The start date cannot be in the past or later than the end date.");
```
- Conditions:
  `C1`: startDate <= DateTime.Today
  `C2`: startDate > endDate
- Decision: `C1 || C2` (logical OR)


| Test Case |  C1 (startDate <= DateTime.Today)  |  C2 (startDate > endDate)  |  Decision (C1 ∨ C2)   |
|-----------|:----------------------------------:|:--------------------------:|:---------------------:|
| TC1       |                True                |           False            |         True          |
| TC2       |               False                |            True            |         True          |
| TC3       |               False                |           False            |         False         |

**Inner if Statement**:
```
if (activeBookingsForCurrentRoom.All(b => startDate < b.StartDate &&
    endDate < b.StartDate || startDate > b.EndDate && endDate > b.EndDate))
```
- Conditions:
  - `C3`: `startDate < b.StartDate`
  - `C4`: `endDate < b.StartDate`
  - `C5`: `startDate > b.EndDate`
  - `C6`: `endDate > b.EndDate`
- Decision:
  - `(C3 && C4) || (C5 && C6)`

| Test Case  |                        C3 (`startDate < b.StartDate`)                         | C4 (`endDate < b.StartDate`) | C5 (`startDate > b.EndDate`) | C6 (`endDate > b.EndDate`) | Decision (`(C3 && C4) v (C5 && C6)`) |
|------------|:-----------------------------------------------------------------------------:|:----------------------------:|:----------------------------:|:--------------------------:|:------------------------------------:|
| TC4        |                                     True                                      |             True             |            False             |           False            |                 True                 |
| TC5        |                                     False                                     |            False             |             True             |            True            |                 True                 |
| TC6        |                                     False                                     |            False             |            False             |           False            |                False                 |
| TC7        |      (Loop completes without matching any conditions in the `Inner if`)       |              -               |               -              |             -              |                False                 |

## Test Cases

| **Test Case** | **Scenario**                                                                                    | **Expected Result**                                |
|---------------|-------------------------------------------------------------------------------------------------|--------------------------------------------------|
| TC1           | `startDate <= DateTime.Today`, valid `endDate`.                                                 | Exception is thrown.                             |
| TC2           | `startDate > DateTime.Today`, but `startDate > endDate`.                                        | Exception is thrown.                             |
| TC3           | `startDate > DateTime.Today`, `startDate <= endDate`.                                           | Method proceeds to room checking logic.          |
| TC4           | No overlapping bookings: `startDate` and `endDate` are both before any existing booking period. | Room is available; returns `room.Id`.            |
| TC5           | No overlapping bookings: `startDate` and `endDate` are both after all existing booking periods. | Room is available; returns `room.Id`.            |
| TC6           | Overlapping booking exists: `startDate` and `endDate` conflict with an existing booking period. | Room is unavailable; continues checking others.  |
| TC7           | All rooms checked but none available (loop completes).                                          | Returns `-1` (no available room).                |
