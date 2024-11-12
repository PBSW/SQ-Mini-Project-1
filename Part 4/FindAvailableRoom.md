# Code

```csharp
public int FindAvailableRoom(DateTime startDate, DateTime endDate)
{
    if (startDate <= DateTime.Today || startDate > endDate)
        throw new ArgumentException("The start date cannot be in the past or later than the end date.");

    var activeBookings = bookingRepository.GetAll().Where(b => b.IsActive);
    foreach (var room in roomRepository.GetAll())
    {
        var activeBookingsForCurrentRoom = activeBookings
        .Where(b => b.RoomId == room.Id);
        
        if (activeBookingsForCurrentRoom.All(b => 
        startDate < b.StartDate && endDate < b.StartDate 
        || startDate > b.EndDate && endDate > b.EndDate))
        {
            return room.Id;
        }
    }
    return -1;
}
```


