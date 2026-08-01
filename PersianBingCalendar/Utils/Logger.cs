namespace PersianBingCalendar.Utils;

public static class Logger
{
    private static readonly object Lock = new();

    public static void LogException(this Exception? ex) => ex?.ToString().LogText();

    public static void LogText(this string text)
    {
        lock (Lock)
        {
            Console.WriteLine(text);

            File.AppendAllText(Path.Combine(DirUtils.GetAppPath(), path2: "errors.log"),
                FormattableString.Invariant(
                    $"{DateTime.Now}{Environment.NewLine}{text}{Environment.NewLine}{Environment.NewLine}"));
        }
    }
}
