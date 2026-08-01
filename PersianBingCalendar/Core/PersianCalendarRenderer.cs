using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public sealed class PersianCalendarRenderer : IDisposable
{
    private const int BingLogoRightMargin = 246;
    private const int BrightnessThreshold = 130;
    private const int CellMargin = 15;
    private const string LargestDayStringInMonth = "31";
    private const int RightMargin = 70;
    private const int TopHeaderMargin = 20;

    private readonly int _day;
    private readonly Graphics? _graphics;
    private readonly Image? _image;
    private readonly int _month;
    private readonly int _year;
    private Color _avgColor;
    private bool _disposed;
    private Color _headerColor;
    private Color _holidayColor;
    private int _leftMargin;
    private int _maxCellHeight;
    private int _maxCellWidth;
    private List<Point>? _points;
    private int _topMargin;

    public PersianCalendarRenderer(string imageFileName)
    {
        PersianDateHelper.GregorianToPersianDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, out _year,
            out _month, out _day);

        _image = Image.FromFile(imageFileName);
        _graphics = Graphics.FromImage(_image);
        _graphics.SmoothingMode = SmoothingMode.AntiAlias;
        _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        _graphics.InterpolationMode = InterpolationMode.High;
        _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
    }

    public string? CalendarFontFileName { get; set; }

    public int CalendarFontSize { get; set; }

    public string? CopyrightFontName { get; set; }

    public int CopyrightFontSize { get; set; }

    public string? CopyrightText { get; set; }

    public IList<HolidayItem>? Holidays { get; set; }

    public int HolidaysFontSize { get; set; }

    public bool ShowPastHolidays { get; set; }

    public Color TodayColor { get; set; }

    public bool ShowCopyright { get; set; }

    public void Dispose() => Dispose(disposeManagedResources: true);

    public Image? DrawThisMonthsCalendar()
    {
        SetSizes();
        SetColors();
        PrintHeaderText();
        _points = CreateMonthPoints();
        DrawDaysOfWeek();
        DrawCalendar();
        PrintCopyright();

        return _image;
    }

    private void Dispose(bool disposeManagedResources)
    {
        if (_disposed)
        {
            return;
        }

        if (!disposeManagedResources)
        {
            return;
        }

        _graphics?.Dispose();
        _image?.Dispose();
        _disposed = true;
    }

    private List<Point> CreateMonthPoints()
    {
        var cellUpperLeftPoints = new List<Point>();

        for (var row = 0; row < 7; row++)
        {
            for (var col = 0; col < 7; col++)
            {
                var left = _leftMargin + (col * _maxCellWidth);
                var top = _topMargin + (row * _maxCellHeight);
                var point = new Point(left, top);

                cellUpperLeftPoints.Add(point);
            }
        }

        return cellUpperLeftPoints;
    }

    private void DrawCalendar()
    {
        var holidayItems = new List<HolidayItem>();

        var startDay = PersianDateHelper.Find1StDayOfMonth(_year, _month);
        var noOfDays = _month <= 6 ? 31 : 30;
        var currentMonth = _month - 1;

        if (currentMonth == 11)
        {
            noOfDays = PersianDateHelper.IsLeapYear(_year) ? 30 : 29;
        }

        var curDay = 1;
        const int oneRowItems = 7;
        var weekDay = startDay + 1;
        var firstRowPoints = _points.Skip(oneRowItems).Take(oneRowItems - startDay).Reverse().ToList();

        foreach (var point in firstRowPoints)
        {
            DrawDay(curDay, weekDay, point, holidayItems);

            weekDay++;
            curDay++;
        }

        const int first2Rows = oneRowItems + oneRowItems;
        var otherMonthPoints = _points.Skip(first2Rows).ToList();
        var rowCount = 0;
        var lastPoint = new Point(_leftMargin, _topMargin + (7 * _maxCellHeight));

        while (curDay <= noOfDays)
        {
            var rtlPointsToShow = otherMonthPoints.Skip(rowCount).Take(oneRowItems).Reverse();
            weekDay = 1;

            foreach (var point in rtlPointsToShow)
            {
                DrawDay(curDay, weekDay, point, holidayItems);

                weekDay++;
                curDay++;

                if (curDay > noOfDays)
                {
                    lastPoint = point;

                    break;
                }
            }

            rowCount += oneRowItems;
        }

        PrintHolidayTexts(holidayItems, lastPoint);
    }

    private void DrawDay(int curDay, int weekDay, Point point, List<HolidayItem>? holidayItems)
    {
        var fillColor = curDay == _day ? TodayColor : (Color?)null;

        var holidayItem = GetHolidayItem(curDay);

        if (holidayItem is not null)
        {
            if (curDay != _day)
            {
                fillColor = _holidayColor;
            }

            if (ShowPastHolidays || curDay >= _day)
            {
                holidayItem.IsToday = _day == curDay;
                holidayItems?.Add(holidayItem);
            }
        }

        if (weekDay == 7 && curDay != _day)
        {
            fillColor = _holidayColor;
        }

        var dayText = curDay.ToString(CultureInfo.InvariantCulture).ToPersianNumbers();
        PrintPoint(point, dayText, _maxCellWidth, CalendarFontSize, fillColor);
    }

    private void DrawDaysOfWeek()
    {
        if (_points is null)
        {
            return;
        }

        for (var i = 6; i >= 0; i--)
        {
            var dayOfWeekAbbreviation = PersianCalendarNames.DaysOfWeek[6 - i];
            var point = _points[i];

            PrintPoint(point, dayOfWeekAbbreviation, _maxCellWidth, CalendarFontSize, fillColor: null,
                applyDarkerColor: true);
        }
    }

    private DayItem[] GetDayItems()
    {
        var arCulture = new CultureInfo(name: "ar-SA")
        {
            DateTimeFormat =
            {
                Calendar = new HijriCalendar()
            }
        };

        var now = DateTime.Now;

        return
        [
            new DayItem
            {
                Day = _day.ToString(CultureInfo.InvariantCulture),
                Text =
                    $"{PersianCalendarNames.MonthNames[_month - 1]} {_year.ToString(CultureInfo.InvariantCulture).ToPersianNumbers()}"
            },
            new DayItem
            {
                Day = FormattableString.Invariant(
                    $"{int.Parse(now.ToString(format: "dd", arCulture), CultureInfo.InvariantCulture)}"),
                Text = $"{now.ToString(format: "MMMM yyyy", arCulture)}"
            },
            new DayItem
            {
                Day = now.Day.ToString(CultureInfo.InvariantCulture),
                Text = $"{now.ToString(format: "yyyy, MMMM", CultureInfo.InvariantCulture)}"
            }
        ];
    }

    private string GetFontPath() => Path.Combine(DirUtils.GetAppPath(), path2: "fonts", CalendarFontFileName);

    private HolidayItem GetHolidayItem(int curDay)
        => Holidays.FirstOrDefault(item => item.Year == _year && item.Month == _month && item.Day == curDay);

    private SizeF GetMaxCellSize(int margin)
    {
        var sizes = new List<SizeF>();

        foreach (var weekDay in PersianCalendarNames.DaysOfWeek)
        {
            var sizeF = MeasureString(weekDay, CalendarFontSize);

            if (sizeF is not null)
            {
                sizes.Add(sizeF.Value);
            }
        }

        var measureString = MeasureString(LargestDayStringInMonth, CalendarFontSize);

        if (measureString is not null)
        {
            sizes.Add(measureString.Value);
        }

        var maxHeight = sizes.Max(x => x.Height);
        var maxWidth = sizes.Max(x => x.Width);

        return new SizeF(maxWidth + margin, maxHeight + margin);
    }

    private SizeF? MeasureString(string? text, int calendarFontSize)
    {
        if (text is null)
        {
            return null;
        }

        var fontPath = GetFontPath();

        return DrawingExtensions.MeasureString(text, calendarFontSize, FontStyle.Regular, fontPath);
    }

    private void PrintCopyright()
    {
        if (!ShowCopyright || _image is null)
        {
            return;
        }

        using var font = new Font(CopyrightFontName, CopyrightFontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        var copyrightSize = DrawingExtensions.MeasureString(CopyrightText, font);

        if (copyrightSize is null)
        {
            return;
        }

        var copyrightLeftMargin = _image.Width - (BingLogoRightMargin + copyrightSize.Value.Width + 5);

        PrintPoint(new Point((int)copyrightLeftMargin, _image.Height - 120), CopyrightText,
            (int)copyrightSize.Value.Width + 20, CopyrightFontSize, fillColor: null, isRtl: false, customFont: font);
    }

    private void PrintHeaderText()
    {
        if (_image is null)
        {
            return;
        }

        var top = _maxCellHeight + TopHeaderMargin;
        var daySize = _maxCellWidth;
        var textSize = _maxCellWidth * 6;
        var space = 0;

        foreach (var item in GetDayItems())
        {
            var dayLeftMargin = _image.Width - (RightMargin + daySize);
            var textPoint = new Point(dayLeftMargin, top + (space * _maxCellHeight));
            PrintPoint(textPoint, item.Day, daySize, HolidaysFontSize, _headerColor);

            var textLeftMargin = _image.Width - (RightMargin + textSize + daySize);
            textPoint = new Point(textLeftMargin, top + (space * _maxCellHeight));
            PrintPoint(textPoint, item.Text, textSize, HolidaysFontSize, _headerColor);

            space++;
        }

        _topMargin = (4 * _maxCellHeight) + TopHeaderMargin;
    }

    private void PrintHolidayTexts(IList<HolidayItem> items, Point lastPoint)
    {
        if (_image is null)
        {
            return;
        }

        var space = 2;
        var dayTextSize = MeasureString(LargestDayStringInMonth, HolidaysFontSize)?.Width;

        if (dayTextSize is null)
        {
            return;
        }

        foreach (var item in items)
        {
            var size = MeasureString(item.Text, HolidaysFontSize)?.Width;

            if (size is null)
            {
                return;
            }

            var dayLeftMargin = _image.Width - (RightMargin + dayTextSize + 5);

            if (!dayLeftMargin.HasValue)
            {
                return;
            }

            var textPoint = new Point((int)dayLeftMargin.Value, lastPoint.Y + (space * _maxCellHeight));

            PrintPoint(textPoint, item.Day.ToString(CultureInfo.InvariantCulture), (int)dayTextSize + 5,
                HolidaysFontSize, item.IsToday ? TodayColor : _holidayColor);

            var textLeftMargin = _image.Width - (RightMargin + size + dayTextSize + 10);
            textPoint = new Point((int)textLeftMargin.Value, lastPoint.Y + (space * _maxCellHeight));

            PrintPoint(textPoint, item.Text, (int)size + 5, HolidaysFontSize,
                item.IsToday ? TodayColor : _holidayColor);

            space++;
        }
    }

    private void PrintPoint(Point point,
        string? text,
        int cellWidth,
        int fontSize,
        Color? fillColor = null,
        bool isRtl = true,
        bool applyDarkerColor = false,
        Font? customFont = null)
    {
        var pointX = point.X;
        var pointY = point.Y;

        var textHeight = customFont is null
            ? MeasureString(text, fontSize)?.Height
            : DrawingExtensions.MeasureString(text, customFont)?.Height;

        if (!textHeight.HasValue)
        {
            return;
        }

        var cellHeight = (int)textHeight + 15;

        if (textHeight > _maxCellHeight)
        {
            pointY -= cellHeight - _maxCellHeight;
        }

        var color = fillColor == null ? _avgColor : Color.FromArgb(alpha: 110, fillColor.Value);

        if (applyDarkerColor)
        {
            color = color.ChangeColorBrightness(correctionFactor: -0.12f);
        }

        var rectangle = new Rectangle(pointX, pointY, cellWidth, cellHeight);

        if (_graphics is null)
        {
            return;
        }

        DrawingExtensions.DrawRoundedRectangle(_graphics, rectangle, cornerRadius: 15, new Pen(color)
        {
            Width = 1.1f
        }, color);

        using var stringFormat = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center,
            FormatFlags = isRtl ? StringFormatFlags.DirectionRightToLeft : StringFormatFlags.NoClip
        };

        _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        var textColor = color.Brightness() < BrightnessThreshold ? Color.White : Color.Black;

        using var solidBrush = new SolidBrush(textColor);

        if (customFont is not null)
        {
            _graphics.DrawString(text, customFont, solidBrush, rectangle, stringFormat);
        }
        else
        {
            using (var privateFontCollection = new PrivateFontCollection())
            {
                privateFontCollection.AddFontFile(GetFontPath());

                using (var fontFamily = privateFontCollection.Families[0])
                {
                    using (var calendarFont = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        _graphics.DrawString(text, calendarFont, solidBrush, rectangle, stringFormat);
                    }
                }
            }
        }
    }

    private void SetAvgColor()
    {
        if (_image is null)
        {
            return;
        }

        var rectangle = new Rectangle(_leftMargin - RightMargin, y: 0, _image.Width, _image.Height / 2);

        using var cropImage = _image.CropImage(rectangle);
        _avgColor = cropImage.CalculateAverageColor();

        _avgColor = Color.FromArgb(alpha: 190, _avgColor);

        while (_avgColor.Brightness() >= BrightnessThreshold)
        {
            _avgColor = _avgColor.ChangeColorBrightness(correctionFactor: -0.01f);
        }
    }

    private void SetColors()
    {
        SetAvgColor();
        _holidayColor = _avgColor.ChangeColorBrightness(correctionFactor: -0.7f);
        _headerColor = _avgColor.ChangeColorBrightness(correctionFactor: -0.4f);
    }

    private void SetSizes()
    {
        if (_image is null)
        {
            return;
        }

        var cellSize = GetMaxCellSize(CellMargin);
        _maxCellHeight = (int)cellSize.Height;
        _maxCellWidth = (int)cellSize.Width;
        _leftMargin = _image.Width - (RightMargin + (7 * _maxCellWidth));
    }
}
