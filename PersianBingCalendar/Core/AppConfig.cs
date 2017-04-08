using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core
{
    public static class AppConfig
    {
        private static readonly int _calendarFontSize;

        private static readonly int _copyrightFontSize;

        private static readonly int _holidaysFontSize;

        private static readonly bool _showPastHolidays;

        private static readonly bool _useRandomImages;

        static AppConfig()
        {
            bool.TryParse(ConfigSetGet.GetConfigData("UseRandomImages"), out _useRandomImages);
            bool.TryParse(ConfigSetGet.GetConfigData("ShowPastHolidays"), out _showPastHolidays);
            int.TryParse(ConfigSetGet.GetConfigData("CalendarFontSize"), out _calendarFontSize);
            int.TryParse(ConfigSetGet.GetConfigData("CopyrightFontSize"), out _copyrightFontSize);
            int.TryParse(ConfigSetGet.GetConfigData("HolidaysFontSize"), out _holidaysFontSize);
            CalendarFontFileName = ConfigSetGet.GetConfigData("CalendarFontFileName");
            CopyrightFontName = ConfigSetGet.GetConfigData("CopyrightFontName");
        }

        public static string CalendarFontFileName { get; }

        public static string CopyrightFontName { get; }

        public static int CalendarFontSize => _calendarFontSize;

        public static int CopyrightFontSize => _copyrightFontSize;

        public static int HolidaysFontSize => _holidaysFontSize;

        public static bool ShowPastHolidays => _showPastHolidays;

        public static bool UseRandomImages => _useRandomImages;
    }
}