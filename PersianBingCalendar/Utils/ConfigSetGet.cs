using System;
using System.Configuration;
using System.Linq;

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
		if (string.IsNullOrWhiteSpace(value: key))
			throw new ArgumentNullException(paramName: nameof(key));


		if (!ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem.Equals(value: key)))
		{
			throw new InvalidOperationException(
				message: string.Format(format: "Undefined key in app.config/web.config: {0}", arg0: key));
		}

		return ConfigurationManager.AppSettings[name: key];
	}

	public static void SetConfigData(string key, string data)
	{
		if (string.IsNullOrWhiteSpace(value: key))
			throw new ArgumentNullException(paramName: nameof(key));

		Configuration config = ConfigurationManager.OpenExeConfiguration(userLevel: ConfigurationUserLevel.None);
		config.AppSettings.Settings[key: key].Value = data;
		config.Save(saveMode: ConfigurationSaveMode.Modified);
		ConfigurationManager.RefreshSection(sectionName: "appSettings");
	}
}