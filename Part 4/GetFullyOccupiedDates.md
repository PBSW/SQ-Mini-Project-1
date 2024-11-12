# Code

```csharp
public List<DateTime> GetFullyOccupiedDates(DateTime startDate, DateTime endDate)
{
    if (startDate > endDate)
        throw new ArgumentException("The start date cannot be later than the end date");

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


