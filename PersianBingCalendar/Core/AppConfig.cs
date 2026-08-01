using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public static class AppConfig
{
    public static readonly string CalendarFontFileName;

    public static readonly string CopyrightFontName;

    public static readonly int CalendarFontSize;

    public static readonly int CopyrightFontSize;

    public static readonly int HolidaysFontSize;

    public static readonly bool ShowPastHolidays;

    public static readonly bool UseRandomImages;

    public static readonly bool ShowCopyright;

    static AppConfig()
    {
        if (!bool.TryParse(ConfigSetGet.GetConfigData(key: "UseRandomImages"), out UseRandomImages))
        {
            UseRandomImages = true;
        }

        if (!bool.TryParse(ConfigSetGet.GetConfigData(key: "ShowPastHolidays"), out ShowPastHolidays))
        {
            ShowPastHolidays = false;
        }

        if (!int.TryParse(ConfigSetGet.GetConfigData(key: "CalendarFontSize"), out CalendarFontSize))
        {
            CalendarFontSize = 23;
        }

        if (!int.TryParse(ConfigSetGet.GetConfigData(key: "CopyrightFontSize"), out CopyrightFontSize))
        {
            CopyrightFontSize = 23;
        }

        if (!int.TryParse(ConfigSetGet.GetConfigData(key: "HolidaysFontSize"), out HolidaysFontSize))
        {
            HolidaysFontSize = 21;
        }

        CalendarFontFileName = ConfigSetGet.GetConfigData(key: "CalendarFontFileName");
        CopyrightFontName = ConfigSetGet.GetConfigData(key: "CopyrightFontName");

        if (!bool.TryParse(ConfigSetGet.GetConfigData(key: "ShowCopyright"), out ShowCopyright))
        {
            ShowCopyright = true;
        }
    }
}
