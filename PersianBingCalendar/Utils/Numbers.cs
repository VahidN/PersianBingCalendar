namespace PersianBingCalendar.Utils;

/// <summary>
///     English numbers to Persian numbers converter and vice versa.
/// </summary>
public static class Numbers
{
	/// <summary>
	///     Converts English digits of a given string to their equivalent Persian digits.
	/// </summary>
	/// <param name="data">English number</param>
	/// <returns></returns>
	public static string ToPersianNumbers(this string data)
	{
		if (string.IsNullOrWhiteSpace(value: data)) return string.Empty;
		return
			data.Replace(oldValue: "0", newValue: "۰")
				.Replace(oldValue: "1", newValue: "۱")
				.Replace(oldValue: "2", newValue: "۲")
				.Replace(oldValue: "3", newValue: "۳")
				.Replace(oldValue: "4", newValue: "۴")
				.Replace(oldValue: "5", newValue: "۵")
				.Replace(oldValue: "6", newValue: "۶")
				.Replace(oldValue: "7", newValue: "۷")
				.Replace(oldValue: "8", newValue: "۸")
				.Replace(oldValue: "9", newValue: "۹");
	}
}