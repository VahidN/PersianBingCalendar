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
        if (string.IsNullOrWhiteSpace(data))
        {
            return string.Empty;
        }

        return data.Replace(oldChar: '0', newChar: '۰')
            .Replace(oldChar: '1', newChar: '۱')
            .Replace(oldChar: '2', newChar: '۲')
            .Replace(oldChar: '3', newChar: '۳')
            .Replace(oldChar: '4', newChar: '۴')
            .Replace(oldChar: '5', newChar: '۵')
            .Replace(oldChar: '6', newChar: '۶')
            .Replace(oldChar: '7', newChar: '۷')
            .Replace(oldChar: '8', newChar: '۸')
            .Replace(oldChar: '9', newChar: '۹');
    }
}
