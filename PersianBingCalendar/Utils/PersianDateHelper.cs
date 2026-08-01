using System.Globalization;

namespace PersianBingCalendar.Utils;

/// <summary>
///     Persian Date Helper class
/// </summary>
public static class PersianDateHelper
{
    /// <summary>
    ///     Finds 1st day of the given year and month.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="monthIndex"></param>
    /// <returns></returns>
    public static int Find1StDayOfMonth(int year, int monthIndex)
    {
        var dayWeek = 1;
        PersianDateToGregorian(year, monthIndex, inDay: 1, out var outYear, out var outMonth, out var outDay);

        var res = new DateTime(outYear, outMonth, outDay);

        dayWeek = res.DayOfWeek switch
        {
            DayOfWeek.Saturday => 0,
            DayOfWeek.Sunday => 1,
            DayOfWeek.Monday => 2,
            DayOfWeek.Tuesday => 3,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 5,
            DayOfWeek.Friday => 6,
            _ => dayWeek
        };

        return dayWeek;
    }

    /// <summary>
    ///     Converts Gregorian date To Hijri date.
    /// </summary>
    /// <param name="inYear"></param>
    /// <param name="inMonth"></param>
    /// <param name="inDay"></param>
    /// <param name="outYear"></param>
    /// <param name="outMonth"></param>
    /// <param name="outDay"></param>
    /// <returns></returns>
    public static bool GregorianToPersianDate(int inYear,
        int inMonth,
        int inDay,
        out int outYear,
        out int outMonth,
        out int outDay)
    {
        try
        {
            var ym = inYear;
            var mm = inMonth;
            var dm = inDay;

            var sss = new PersianCalendar();
            outYear = sss.GetYear(new DateTime(ym, mm, dm, new GregorianCalendar()));
            outMonth = sss.GetMonth(new DateTime(ym, mm, dm, new GregorianCalendar()));
            outDay = sss.GetDayOfMonth(new DateTime(ym, mm, dm, new GregorianCalendar()));

            return true;
        }
        catch //invalid date
        {
            outYear = -1;
            outMonth = -1;
            outDay = -1;

            return false;
        }
    }

    /// <summary>
    ///     Converts Hijri date To Gregorian date.
    /// </summary>
    /// <param name="inYear"></param>
    /// <param name="inMonth"></param>
    /// <param name="inDay"></param>
    /// <param name="outYear"></param>
    /// <param name="outMonth"></param>
    /// <param name="outDay"></param>
    /// <returns></returns>
    public static bool PersianDateToGregorian(int inYear,
        int inMonth,
        int inDay,
        out int outYear,
        out int outMonth,
        out int outDay)
    {
        try
        {
            var ys = inYear;
            var ms = inMonth;
            var ds = inDay;

            var sss = new GregorianCalendar();
            outYear = sss.GetYear(new DateTime(ys, ms, ds, new PersianCalendar()));
            outMonth = sss.GetMonth(new DateTime(ys, ms, ds, new PersianCalendar()));
            outDay = sss.GetDayOfMonth(new DateTime(ys, ms, ds, new PersianCalendar()));

            return true;
        }
        catch //invalid date
        {
            outYear = -1;
            outMonth = -1;
            outDay = -1;

            return false;
        }
    }

    /// <summary>
    ///     Is a given year leap?
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public static bool IsLeapYear(int year)
    {
        var r = year % 33;

        return r is 1 or 5 or 9 or 13 or 17 or 22 or 26 or 30;
    }
}
