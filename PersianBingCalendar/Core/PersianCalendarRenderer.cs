using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using PersianBingCalendar.Models;
using PersianBingCalendar.Utils;

namespace PersianBingCalendar.Core;

public class PersianCalendarRenderer : IDisposable
{
	private readonly int          _day;
	private readonly Graphics     _graphics;
	private readonly Image        _image;
	private readonly int          _month;
	private readonly int          _year;
	private          Color        _avgColor;
	private          bool         _disposed;
	private          Color        _headerColor;
	private          Color        _holidayColor;
	private          int          _leftMargin;
	private          int          _maxCellHeight;
	private          int          _maxCellWidth;
	private          IList<Point> _points;
	private          int          _topMargin;
	private const    int          BingLogoRightMargin     = 246;
	private const    int          BrightnessThreshold     = 130;
	private const    int          CellMargin              = 15;
	private const    string       LargestDayStringInMonth = "31";
	private const    int          RightMargin             = 70;
	private const    int          TopHeaderMargin         = 20;

	public PersianCalendarRenderer(string imageFileName)
	{
		PersianDateHelper.GregorianToPersianDate(
			inYear: DateTime.Now.Year,
			inMonth: DateTime.Now.Month,
			inDay: DateTime.Now.Day,
			outYear: out _year, outMonth: out _month, outDay: out _day);

		_image                      = Image.FromFile(filename: imageFileName);
		_graphics                   = Graphics.FromImage(image: _image);
		_graphics.SmoothingMode     = SmoothingMode.AntiAlias;
		_graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		_graphics.PixelOffsetMode   = PixelOffsetMode.HighQuality;
		_graphics.InterpolationMode = InterpolationMode.High;
		_graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
	}

	public string CalendarFontFileName { set; get; }

	public int CalendarFontSize { set; get; }

	public string CopyrightFontName { set; get; }

	public int CopyrightFontSize { set; get; }

	public string CopyrightText { set; get; }

	public IList<HolidayItem> Holidays { set; get; }

	public int HolidaysFontSize { set; get; }

	public bool ShowPastHolidays { set; get; }

	public Color TodayColor { set; get; }

	public bool ShowCopyright { set; get; }

	public void Dispose()
	{
		Dispose(disposeManagedResources: true);
		GC.SuppressFinalize(obj: this);
	}

	public Image DrawThisMonthsCalendar()
	{
		setSizes();
		setColors();
		printHeaderText();
		_points = createMonthPoints();
		drawDaysOfWeek();
		drawCalendar();
		printCopyright();

		return _image;
	}

	protected virtual void Dispose(bool disposeManagedResources)
	{
		if (_disposed) return;
		if (!disposeManagedResources) return;

		_graphics?.Dispose();
		_disposed = true;
	}

	private List<Point> createMonthPoints()
	{
		List<Point> cellUpperLeftPoints = new();
		for (int row = 0; row < 7; row++)
		{
			for (int col = 0; col < 7; col++)
			{
				int   left  = _leftMargin + (col * _maxCellWidth);
				int   top   = _topMargin  + (row * _maxCellHeight);
				Point point = new(x: left, y: top);

				cellUpperLeftPoints.Add(item: point);
			}
		}

		return cellUpperLeftPoints;
	}

	private void drawCalendar()
	{
		List<HolidayItem> holidayItems = new();

		int startDay     = PersianDateHelper.Find1StDayOfMonth(year: _year, monthIndex: _month);
		int noOfDays     = _month <= 6 ? 31 : 30;
		int currentMonth = _month - 1;
		if (currentMonth == 11)
		{
			noOfDays = PersianDateHelper.IsLeapYear(year: _year) ? 30 : 29;
		}

		int       curDay      = 1;
		const int oneRowItems = 7;
		int       weekDay     = startDay + 1;
		List<Point> firstRowPoints =
			_points.Skip(count: oneRowItems).Take(count: oneRowItems - startDay).Reverse().ToList();
		foreach (Point point in firstRowPoints)
		{
			drawDay(curDay: curDay, weekDay: weekDay, point: point, holidayItems: holidayItems);

			weekDay++;
			curDay++;
		}

		const int   first2Rows       = oneRowItems + oneRowItems;
		List<Point> otherMonthPoints = _points.Skip(count: first2Rows).ToList();
		int         rowCount         = 0;
		Point       lastPoint        = new(x: _leftMargin, y: _topMargin + (7 * _maxCellHeight));
		while (curDay <= noOfDays)
		{
			IEnumerable<Point> rtlPointsToShow =
				otherMonthPoints.Skip(count: rowCount).Take(count: oneRowItems).Reverse();
			weekDay = 1;
			foreach (Point point in rtlPointsToShow)
			{
				drawDay(curDay: curDay, weekDay: weekDay, point: point, holidayItems: holidayItems);

				weekDay++;
				curDay++;
				if (curDay <= noOfDays) continue;
				lastPoint = point;
				break;
			}

			rowCount += oneRowItems;
		}

		printHolidayTexts(items: holidayItems, lastPoint: lastPoint);
	}

	private void drawDay(int curDay, int weekDay, Point point, IList<HolidayItem> holidayItems)
	{
		Color? fillColor = curDay == _day ? TodayColor : null;

		HolidayItem holidayItem = getHolidayItem(curDay: curDay);
		if (holidayItem != null)
		{
			if (curDay != _day)
			{
				fillColor = _holidayColor;
			}

			if (ShowPastHolidays || (curDay >= _day))
			{
				holidayItem.IsToday = _day == curDay;
				holidayItems.Add(item: holidayItem);
			}
		}

		if ((weekDay == 7) && (curDay != _day))
		{
			fillColor = _holidayColor;
		}

		string dayText = curDay.ToString(provider: CultureInfo.InvariantCulture).ToPersianNumbers();
		printPoint(point: point, text: dayText, cellWidth: _maxCellWidth, fontSize: CalendarFontSize,
			fillColor: fillColor);
	}

	private void drawDaysOfWeek()
	{
		for (int i = 6; i >= 0; i--)
		{
			string dayOfWeekAbbreviation = PersianCalendarNames.DaysOfWeek[6 - i];
			Point  point                 = _points[index: i];
			printPoint(point: point, text: dayOfWeekAbbreviation, cellWidth: _maxCellWidth, fontSize: CalendarFontSize,
				applyDarkerColor: true);
		}
	}

	private IEnumerable<DayItem> getDayItems()
	{
		CultureInfo arCulture = new(name: "ar-SA")
		{
			DateTimeFormat = { Calendar = new HijriCalendar() }
		};
		DateTime now = DateTime.Now;
		return
			new[]
			{
				new DayItem
				{
					Day = _day.ToString(),
					Text =
						$"{PersianCalendarNames.MonthNames[_month - 1]} {_year.ToString(provider: CultureInfo.InvariantCulture).ToPersianNumbers()}"
				},
				new DayItem
				{
					Day  = $"{int.Parse(s: now.ToString(format: "dd", provider: arCulture))}",
					Text = $"{now.ToString(format: "MMMM yyyy", provider: arCulture)}"
				},
				new DayItem
				{
					Day  = now.Day.ToString(),
					Text = $"{now.ToString(format: "yyyy, MMMM", provider: CultureInfo.InvariantCulture)}"
				}
			};
	}

	private string getFontPath() =>
		Path.Combine(path1: DirUtils.GetAppPath(), path2: "fonts", path3: CalendarFontFileName);

	private HolidayItem getHolidayItem(int curDay)
	{
		return Holidays.FirstOrDefault(item => (item.Year == _year) && (item.Month == _month) && (item.Day == curDay));
	}

	private SizeF getMaxCellSize(int margin)
	{
		List<SizeF> sizes = PersianCalendarNames.DaysOfWeek
			.Select(weekDay => measureString(text: weekDay, calendarFontSize: CalendarFontSize)).ToList();

		sizes.Add(item: measureString(text: LargestDayStringInMonth, calendarFontSize: CalendarFontSize));

		float maxHeight = sizes.Select(x => x.Height).Max();
		float maxWidth  = sizes.Select(x => x.Width).Max();

		return new(width: maxWidth + margin, height: maxHeight + margin);
	}

	private SizeF measureString(string text, int calendarFontSize)
	{
		string fontPath = getFontPath();
		return DrawingExtensions.MeasureString(text: text, fontSize: calendarFontSize, fontStyle: FontStyle.Regular,
			path: fontPath);
	}

	private void printCopyright()
	{
		if (!ShowCopyright)
		{
			return;
		}

		using Font font = new(familyName: CopyrightFontName, emSize: CopyrightFontSize, style: FontStyle.Regular,
			unit: GraphicsUnit.Pixel);
		SizeF copyrightSize = DrawingExtensions.MeasureString(text: CopyrightText, f: font);
		int copyrightLeftMargin = (int)(_image.Width - (BingLogoRightMargin + copyrightSize.Width + 5));
		printPoint(point: new(x: copyrightLeftMargin, y: _image.Height - 120), text: CopyrightText,
			cellWidth: (int)copyrightSize.Width + 20, fontSize: CopyrightFontSize, isRtl: false, customFont: font);
	}

	private void printHeaderText()
	{
		int top      = _maxCellHeight + TopHeaderMargin;
		int daySize  = _maxCellWidth;
		int textSize = _maxCellWidth * 6;
		int space    = 0;

		foreach (DayItem item in getDayItems())
		{
			int   dayLeftMargin = _image.Width - (RightMargin + daySize);
			Point textPoint     = new(x: dayLeftMargin, y: top + (space * _maxCellHeight));
			printPoint(point: textPoint, text: item.Day, cellWidth: daySize, fontSize: HolidaysFontSize,
				fillColor: _headerColor);

			int textLeftMargin = _image.Width - (RightMargin + textSize + daySize);
			textPoint = new(x: textLeftMargin, y: top + (space * _maxCellHeight));
			printPoint(point: textPoint, text: item.Text, cellWidth: textSize, fontSize: HolidaysFontSize,
				fillColor: _headerColor);

			space++;
		}

		_topMargin = (4 * _maxCellHeight) + TopHeaderMargin;
	}

	private void printHolidayTexts(IList<HolidayItem> items, Point lastPoint)
	{
		int   space       = 2;
		float dayTextSize = measureString(text: LargestDayStringInMonth, calendarFontSize: HolidaysFontSize).Width;

		foreach (HolidayItem item in items)
		{
			float size = measureString(text: item.Text, calendarFontSize: HolidaysFontSize).Width;

			int   dayLeftMargin = (int)(_image.Width - (RightMargin + dayTextSize + 5));
			Point textPoint     = new(x: dayLeftMargin, y: lastPoint.Y + (space * _maxCellHeight));
			printPoint(point: textPoint,    text: item.Day.ToString(), cellWidth: (int)dayTextSize + 5,
				fontSize: HolidaysFontSize, fillColor: item.IsToday ? TodayColor : _holidayColor);

			int textLeftMargin = (int)(_image.Width - (RightMargin + size + dayTextSize + 10));
			textPoint = new(x: textLeftMargin, y: lastPoint.Y + (space * _maxCellHeight));
			printPoint(point: textPoint, text: item.Text, cellWidth: (int)size + 5, fontSize: HolidaysFontSize,
				fillColor: item.IsToday ? TodayColor : _holidayColor);

			space++;
		}
	}

	private void printPoint(
		Point point,        string text,                     int  cellWidth, int fontSize, Color? fillColor = null,
		bool  isRtl = true, bool   applyDarkerColor = false, Font customFont = null)
	{
		int pointX = point.X;
		int pointY = point.Y;

		float textHeight = customFont == null
			? measureString(text: text, calendarFontSize: fontSize).Height
			: DrawingExtensions.MeasureString(text: text, f: customFont).Height;
		int cellHeight = (int)textHeight + 15;
		if (textHeight > _maxCellHeight)
		{
			pointY -= cellHeight - _maxCellHeight;
		}

		Color color = fillColor == null ? _avgColor : Color.FromArgb(alpha: 110, baseColor: fillColor.Value);

		if (applyDarkerColor)
		{
			color = color.ChangeColorBrightness(correctionFactor: -0.12f);
		}

		Rectangle rectangle = new(x: pointX, y: pointY, width: cellWidth, height: cellHeight);
		DrawingExtensions.DrawRoundedRectangle(gfx: _graphics, bounds: rectangle, cornerRadius: 15,
			drawPen: new(color: color) { Width = 1.1f }, fillColor: color);

		StringFormat stringFormat = new()
		{
			LineAlignment = StringAlignment.Center,
			Alignment     = StringAlignment.Center,
			FormatFlags   = isRtl ? StringFormatFlags.DirectionRightToLeft : StringFormatFlags.NoClip
		};

		_graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
		Color textColor = color.Brightness() < BrightnessThreshold ? Color.White : Color.Black;

		if (customFont != null)
		{
			_graphics.DrawString(s: text, font: customFont, brush: new SolidBrush(color: textColor),
				layoutRectangle: rectangle, format: stringFormat);
		}
		else
		{
			using PrivateFontCollection privateFontCollection = new();
			privateFontCollection.AddFontFile(filename: getFontPath());
			using FontFamily fontFamily = privateFontCollection.Families[0];
			using Font calendarFont = new(family: fontFamily, emSize: fontSize, style: FontStyle.Regular,
				unit: GraphicsUnit.Pixel);
			_graphics.DrawString(s: text, font: calendarFont, brush: new SolidBrush(color: textColor),
				layoutRectangle: rectangle,
				format: stringFormat);
		}
	}

	private void setAvgColor()
	{
		_avgColor = _image.CropImage(crop: new(x: _leftMargin - RightMargin, y: 0, width: _image.Width,
				height: _image.Height / 2))
			.CalculateAverageColor();
		_avgColor = Color.FromArgb(alpha: 190, baseColor: _avgColor);
		while (_avgColor.Brightness() >= BrightnessThreshold)
		{
			_avgColor = _avgColor.ChangeColorBrightness(correctionFactor: -0.01f);
		}
	}

	private void setColors()
	{
		setAvgColor();
		_holidayColor = _avgColor.ChangeColorBrightness(correctionFactor: -0.7f);
		_headerColor  = _avgColor.ChangeColorBrightness(correctionFactor: -0.4f);
	}

	private void setSizes()
	{
		SizeF cellSize = getMaxCellSize(margin: CellMargin);
		_maxCellHeight = (int)cellSize.Height;
		_maxCellWidth  = (int)cellSize.Width;
		_leftMargin    = _image.Width - (RightMargin + (7 * _maxCellWidth));
	}
}