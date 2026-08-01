namespace PersianBingCalendar.Models;

public class HolidayItem
{
    public int Day { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public bool IsToday { get; set; }

    public string? Text { get; set; }
}
