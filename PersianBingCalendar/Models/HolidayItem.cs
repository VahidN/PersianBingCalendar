namespace PersianBingCalendar.Models
{
    public class HolidayItem
    {
        public int Day { set; get; }
        public int Month { set; get; }
        public int Year { set; get; }
        public bool IsToday { set; get; }
        public string Text { set; get; }
    }
}