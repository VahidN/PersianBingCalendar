using System.Configuration;
using System.Globalization;

namespace PersianBingCalendar.Utils;

public static class ConfigSetGet
{
    /// <summary>
    ///     read settings from app.config/web.config file
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ConfigurationErrorsException">
    ///     Could not retrieve a
    ///     <see cref="T:System.Collections.Specialized.NameValueCollection" /> object with the application settings data.
    /// </exception>
    /// <exception cref="InvalidOperationException">Undefined key in app.config/web.config.</exception>
    public static string GetConfigData(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (!ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem.Equals(key, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                format: "Undefined key in app.config/web.config: {0}", key));
        }

        return ConfigurationManager.AppSettings[key];
    }

    public static void SetConfigData(string key, string data)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings[key].Value = data;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(sectionName: "appSettings");
    }
}
