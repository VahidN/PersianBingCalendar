using System;
using System.Globalization;

namespace PersianBingCalendar.Utils
{
    /// <summary>
    /// Persian Date Helper class
    /// </summary>
    public static class PersianDateHelper
    {
        /// <summary>
        /// Finds 1st day of the given year and month.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthIndex"></param>
        /// <returns></returns>
        public static int Find1StDayOfMonth(int year, int monthIndex)
        {
            int outYear, outMonth, outDay, dayWeek = 1;
            PersianDateToGregorian(year, monthIndex, 1, out outYear, out outMonth, out outDay);

            var res = new DateTime(outYear, outMonth, outDay);

            switch (res.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    dayWeek = 0;
                    break;

                case DayOfWeek.Sunday:
                    dayWeek = 1;
                    break;

                case DayOfWeek.Monday:
                    dayWeek = 2;
                    break;

                case DayOfWeek.Tuesday:
                    dayWeek = 3;
                    break;

                case DayOfWeek.Wednesday:
                    dayWeek = 4;
                    break;

                case DayOfWeek.Thursday:
                    dayWeek = 5;
                    break;

                case DayOfWeek.Friday:
                    dayWeek = 6;
                    break;
            }

            return dayWeek;
        }

        /// <summary>
        /// Converts Gregorian date To Hijri date.
        /// </summary>
        /// <param name="inYear"></param>
        /// <param name="inMonth"></param>
        /// <param name="inDay"></param>
        /// <param name="outYear"></param>
        /// <param name="outMonth"></param>
        /// <param name="outDay"></param>
        /// <returns></returns>
        public static bool GregorianToPersianDate(int inYear, int inMonth, int inDay,
                                            out int outYear, out int outMonth, out int outDay)
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
        /// Converts Hijri date To Gregorian date.
        /// </summary>
        /// <param name="inYear"></param>
        /// <param name="inMonth"></param>
        /// <param name="inDay"></param>
        /// <param name="outYear"></param>
        /// <param name="outMonth"></param>
        /// <param name="outDay"></param>
        /// <returns></returns>
        public static bool PersianDateToGregorian(
                    int inYear, int inMonth, int inDay,
                    out int outYear, out int outMonth, out int outDay)
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
        /// Is a given year leap?
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int year)
        {
            var r = year % 33;
            return (r == 1 || r == 5 || r == 9 || r == 13 || r == 17 || r == 22 || r == 26 || r == 30);
        }
    }
}