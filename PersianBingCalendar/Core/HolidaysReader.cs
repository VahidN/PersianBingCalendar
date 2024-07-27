using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class HolidaysReader
{
	public static IList<HolidayItem> GetHolidays()
	{
		List<HolidayItem> results = new();

		string appPath  = DirUtils.GetAppPath();
		string filePath = Path.Combine(path1: appPath, path2: "data", path3: "holidays.csv");
		if (!File.Exists(path: filePath))
		{
			return results;
		}

		results.AddRange(collection: from line in File.ReadAllLines(path: filePath)
			where !string.IsNullOrWhiteSpace(value: line)
			select line.Split(separator: new[] { ',' }, options: StringSplitOptions.RemoveEmptyEntries)
			into items
			where items.Length >= 2
			let date = items[0].Replace(oldValue: "\"", newValue: string.Empty).Trim()
			let text = items[1].Replace(oldValue: "\"", newValue: string.Empty).Trim()
			let dateParts = date.Split('/')
			where dateParts.Length == 3
			select new HolidayItem
			{
				Text = text, Year = int.Parse(s: dateParts[0]), Month = int.Parse(s: dateParts[1]),
				Day  = int.Parse(s: dateParts[2])
			});

		return results;
	}
}