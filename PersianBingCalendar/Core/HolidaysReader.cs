using System;
using System.Collections.Generic;
using System.IO;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core
{
    public static class HolidaysReader
    {
        public static IList<HolidayItem> GetHolidays()
        {
            var results = new List<HolidayItem>();

            var appPath = DirUtils.GetAppPath();
            var filePath = Path.Combine(appPath, "data", "holidays.csv");
            if (!File.Exists(filePath))
            {
                return results;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var items = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length < 2)
                {
                    continue;
                }

                var date = items[0].Replace("\"", string.Empty).Trim();
                var text = items[1].Replace("\"", string.Empty).Trim();

                var dateParts = date.Split('/');
                if (dateParts.Length != 3)
                {
                    continue;
                }

                results.Add(new HolidayItem
                {
                    Text = text,
                    Year = int.Parse(dateParts[0]),
                    Month = int.Parse(dateParts[1]),
                    Day = int.Parse(dateParts[2])
                });
            }

            return results;
        }
    }
}