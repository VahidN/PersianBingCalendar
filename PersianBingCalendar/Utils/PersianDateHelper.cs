using System;
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
		int dayWeek = 1;
		PersianDateToGregorian(inYear: year, inMonth: monthIndex, inDay: 1, outYear: out int outYear,
			outMonth: out int outMonth,      outDay: out int outDay);

		DateTime res = new(year: outYear, month: outMonth, day: outDay);

		dayWeek = res.DayOfWeek switch
		{
			DayOfWeek.Saturday  => 0,
			DayOfWeek.Sunday    => 1,
			DayOfWeek.Monday    => 2,
			DayOfWeek.Tuesday   => 3,
			DayOfWeek.Wednesday => 4,
			DayOfWeek.Thursday  => 5,
			DayOfWeek.Friday    => 6,
			_                   => dayWeek
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
	public static bool GregorianToPersianDate(int inYear,  int     inMonth,  int     inDay,
		out int                                   outYear, out int outMonth, out int outDay)
	{
		try
		{
			int ym = inYear;
			int mm = inMonth;
			int dm = inDay;

			PersianCalendar sss = new();
			outYear  = sss.GetYear(time: new(year: ym, month: mm, day: dm, calendar: new GregorianCalendar()));
			outMonth = sss.GetMonth(time: new(year: ym, month: mm, day: dm, calendar: new GregorianCalendar()));
			outDay   = sss.GetDayOfMonth(time: new(year: ym, month: mm, day: dm, calendar: new GregorianCalendar()));
			return true;
		}
		catch //invalid date
		{
			outYear  = -1;
			outMonth = -1;
			outDay   = -1;
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
	public static bool PersianDateToGregorian(
		int     inYear,  int     inMonth,  int     inDay,
		out int outYear, out int outMonth, out int outDay)
	{
		try
		{
			int ys = inYear;
			int ms = inMonth;
			int ds = inDay;

			GregorianCalendar sss = new();
			outYear  = sss.GetYear(time: new(year: ys, month: ms, day: ds, calendar: new PersianCalendar()));
			outMonth = sss.GetMonth(time: new(year: ys, month: ms, day: ds, calendar: new PersianCalendar()));
			outDay   = sss.GetDayOfMonth(time: new(year: ys, month: ms, day: ds, calendar: new PersianCalendar()));

			return true;
		}
		catch //invalid date
		{
			outYear  = -1;
			outMonth = -1;
			outDay   = -1;
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
		int r = year % 33;
		return r is 1 or 5 or 9 or 13 or 17 or 22 or 26 or 30;
	}
}