using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class AppConfig
{
	private static readonly int _calendarFontSize;

	private static readonly int _copyrightFontSize;

	private static readonly int _holidaysFontSize;

	private static readonly bool _showPastHolidays;

	private static readonly bool _useRandomImages;

	private static readonly bool _showCopyright;

	static AppConfig()
	{
		bool.TryParse(value: ConfigSetGet.GetConfigData(key: "UseRandomImages"),  result: out _useRandomImages);
		bool.TryParse(value: ConfigSetGet.GetConfigData(key: "ShowPastHolidays"), result: out _showPastHolidays);
		int.TryParse(s: ConfigSetGet.GetConfigData(key: "CalendarFontSize"),  result: out _calendarFontSize);
		int.TryParse(s: ConfigSetGet.GetConfigData(key: "CopyrightFontSize"), result: out _copyrightFontSize);
		int.TryParse(s: ConfigSetGet.GetConfigData(key: "HolidaysFontSize"),  result: out _holidaysFontSize);
		CalendarFontFileName = ConfigSetGet.GetConfigData(key: "CalendarFontFileName");
		CopyrightFontName    = ConfigSetGet.GetConfigData(key: "CopyrightFontName");
		bool.TryParse(value: ConfigSetGet.GetConfigData(key: "ShowCopyright"), result: out _showCopyright);
	}

	public static string CalendarFontFileName { get; }

	public static string CopyrightFontName { get; }

	public static int CalendarFontSize => _calendarFontSize;

	public static int CopyrightFontSize => _copyrightFontSize;

	public static int HolidaysFontSize => _holidaysFontSize;

	public static bool ShowPastHolidays => _showPastHolidays;

	public static bool UseRandomImages => _useRandomImages;

	public static bool ShowCopyright => _showCopyright;
}