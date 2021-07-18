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

namespace PersianBingCalendar.Core
{
    public class PersianCalendarRenderer : IDisposable
    {
        private const int BingLogoRightMargin = 246;
        private const int BrightnessThreshold = 130;
        private const int CellMargin = 15;
        private const string LargestDayStringInMonth = "31";
        private const int RightMargin = 70;
        private const int TopHeaderMargin = 20;
        private readonly int _day;
        private readonly Graphics _graphics;
        private readonly Image _image;
        private readonly int _month;
        private readonly int _year;
        private Color _avgColor;
        private bool _disposed;
        private Color _headerColor;
        private Color _holidayColor;
        private int _leftMargin;
        private int _maxCellHeight;
        private int _maxCellWidth;
        private IList<Point> _points;
        private int _topMargin;

        public PersianCalendarRenderer(string imageFileName)
        {
            PersianDateHelper.GregorianToPersianDate(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                out _year, out _month, out _day);

            _image = Image.FromFile(imageFileName);
            _graphics = Graphics.FromImage(_image);
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
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
            Dispose(true);
            GC.SuppressFinalize(this);
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
            var cellUpperLeftPoints = new List<Point>();
            for (var row = 0; row < 7; row++)
            {
                for (var col = 0; col < 7; col++)
                {
                    var left = _leftMargin + col * _maxCellWidth;
                    var top = _topMargin + row * _maxCellHeight;
                    var point = new Point(left, top);

                    cellUpperLeftPoints.Add(point);
                }
            }
            return cellUpperLeftPoints;
        }

        private void drawCalendar()
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
                drawDay(curDay, weekDay, point, holidayItems);

                weekDay++;
                curDay++;
            }

            const int first2Rows = oneRowItems + oneRowItems;
            var otherMonthPoints = _points.Skip(first2Rows).ToList();
            var rowCount = 0;
            var lastPoint = new Point(_leftMargin, _topMargin + 7 * _maxCellHeight);
            while (curDay <= noOfDays)
            {
                var rtlPointsToShow = otherMonthPoints.Skip(rowCount).Take(oneRowItems).Reverse();
                weekDay = 1;
                foreach (var point in rtlPointsToShow)
                {
                    drawDay(curDay, weekDay, point, holidayItems);

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

            printHolidayTexts(holidayItems, lastPoint);
        }

        private void drawDay(int curDay, int weekDay, Point point, IList<HolidayItem> holidayItems)
        {
            var fillColor = curDay == _day ? TodayColor : (Color?)null;

            var holidayItem = getHolidayItem(curDay);
            if (holidayItem != null)
            {
                if (curDay != _day)
                {
                    fillColor = _holidayColor;
                }

                if (ShowPastHolidays || curDay >= _day)
                {
                    holidayItem.IsToday = _day == curDay;
                    holidayItems.Add(holidayItem);
                }
            }

            if (weekDay == 7 && curDay != _day)
            {
                fillColor = _holidayColor;
            }

            var dayText = curDay.ToString(CultureInfo.InvariantCulture).ToPersianNumbers();
            printPoint(point, dayText, _maxCellWidth, CalendarFontSize, fillColor);
        }

        private void drawDaysOfWeek()
        {
            for (var i = 6; i >= 0; i--)
            {
                var dayOfWeekAbbreviation = PersianCalendarNames.DaysOfWeek[6 - i];
                var point = _points[i];
                printPoint(point, dayOfWeekAbbreviation, _maxCellWidth, CalendarFontSize, null, applyDarkerColor: true);
            }
        }

        private IEnumerable<DayItem> getDayItems()
        {
            var arCulture = new CultureInfo("ar-SA")
            {
                DateTimeFormat = { Calendar = new HijriCalendar() }
            };
            var now = DateTime.Now;
            return
                new[]
                {
                    new DayItem
                    {
                        Day = _day.ToString(),
                        Text = $"{PersianCalendarNames.MonthNames[_month - 1]} {_year.ToString(CultureInfo.InvariantCulture).ToPersianNumbers()}"
                    },
                    new DayItem
                    {
                        Day = $"{int.Parse(now.ToString("dd", arCulture))}",
                        Text = $"{now.ToString("MMMM yyyy", arCulture)}"
                    },
                    new DayItem
                    {
                        Day = now.Day.ToString(),
                        Text = $"{now.ToString("yyyy, MMMM", CultureInfo.InvariantCulture)}"
                    }
                };
        }

        private string getFontPath()
        {
            return Path.Combine(DirUtils.GetAppPath(), "fonts", CalendarFontFileName);
        }

        private HolidayItem getHolidayItem(int curDay)
        {
            return Holidays.FirstOrDefault(item => item.Year == _year && item.Month == _month && item.Day == curDay);
        }

        SizeF getMaxCellSize(int margin)
        {
            var sizes = new List<SizeF>();

            foreach (var weekDay in PersianCalendarNames.DaysOfWeek)
            {
                sizes.Add(measureString(weekDay, CalendarFontSize));
            }

            sizes.Add(measureString(LargestDayStringInMonth, CalendarFontSize));

            var maxHeight = sizes.Select(x => x.Height).Max();
            var maxWidth = sizes.Select(x => x.Width).Max();

            return new SizeF(maxWidth + margin, maxHeight + margin);
        }

        private SizeF measureString(string text, int calendarFontSize)
        {
            var fontPath = getFontPath();
            return DrawingExtensions.MeasureString(text, calendarFontSize, FontStyle.Regular, fontPath);
        }

        private void printCopyright()
        {
            if(!ShowCopyright)
            {
               return;
            }

            using (var font = new Font(CopyrightFontName, CopyrightFontSize, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                var copyrightSize = DrawingExtensions.MeasureString(CopyrightText, font);
                var copyrightLeftMargin = (int)(_image.Width - (BingLogoRightMargin + copyrightSize.Width + 5));
                printPoint(new Point(copyrightLeftMargin, _image.Height - 120), CopyrightText, (int)copyrightSize.Width + 20, CopyrightFontSize, null, isRtl: false, customFont: font);
            }
        }

        private void printHeaderText()
        {
            var top =  _maxCellHeight + TopHeaderMargin;
            var daySize = _maxCellWidth;
            var textSize = _maxCellWidth * 6;
            var space = 0;

            foreach (var item in getDayItems())
            {
                var dayLeftMargin = _image.Width - (RightMargin + daySize);
                var textPoint = new Point(dayLeftMargin, top + space * _maxCellHeight);
                printPoint(textPoint, item.Day, daySize, HolidaysFontSize, _headerColor);

                var textLeftMargin = _image.Width - (RightMargin + textSize + daySize);
                textPoint = new Point(textLeftMargin, top + space * _maxCellHeight);
                printPoint(textPoint, item.Text, textSize, HolidaysFontSize, _headerColor);

                space++;
            }

            _topMargin = 4 * _maxCellHeight + TopHeaderMargin;
        }

        private void printHolidayTexts(IList<HolidayItem> items, Point lastPoint)
        {
            var space = 2;
            var dayTextSize = measureString(LargestDayStringInMonth, HolidaysFontSize).Width;

            foreach (var item in items)
            {
                var size = measureString(item.Text, HolidaysFontSize).Width;

                var dayLeftMargin = (int)(_image.Width - (RightMargin + dayTextSize + 5));
                var textPoint = new Point(dayLeftMargin, lastPoint.Y + space * _maxCellHeight);
                printPoint(textPoint, item.Day.ToString(), (int)dayTextSize + 5, HolidaysFontSize, item.IsToday ? TodayColor : _holidayColor);

                var textLeftMargin = (int)(_image.Width - (RightMargin + size + dayTextSize + 10));
                textPoint = new Point(textLeftMargin, lastPoint.Y + space * _maxCellHeight);
                printPoint(textPoint, item.Text, (int)size + 5, HolidaysFontSize, item.IsToday ? TodayColor : _holidayColor);

                space++;
            }
        }

        private void printPoint(
            Point point, string text, int cellWidth, int fontSize, Color? fillColor = null,
            bool isRtl = true, bool applyDarkerColor = false, Font customFont = null)
        {
            var pointX = point.X;
            var pointY = point.Y;

            var textHeight = customFont == null ? measureString(text, fontSize).Height : DrawingExtensions.MeasureString(text, customFont).Height;
            var cellHeight = (int)textHeight + 15;
            if (textHeight > _maxCellHeight)
            {
                pointY -= cellHeight - _maxCellHeight;
            }

            var color = fillColor == null ? _avgColor : Color.FromArgb(alpha: 110, baseColor: fillColor.Value);

            if (applyDarkerColor)
            {
                color = color.ChangeColorBrightness(-0.12f);
            }

            var rectangle = new Rectangle(pointX, pointY, cellWidth, cellHeight);
            DrawingExtensions.DrawRoundedRectangle(_graphics, rectangle, 15, new Pen(color) { Width = 1.1f }, color);

            var stringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center,
                FormatFlags = isRtl ? StringFormatFlags.DirectionRightToLeft : StringFormatFlags.NoClip
            };

            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            var textColor = color.Brightness() < BrightnessThreshold ? Color.White : Color.Black;

            if (customFont != null)
            {
                _graphics.DrawString(text, customFont, new SolidBrush(textColor), rectangle, stringFormat);
            }
            else
            {
                using (var privateFontCollection = new PrivateFontCollection())
                {
                    privateFontCollection.AddFontFile(getFontPath());
                    using (var fontFamily = privateFontCollection.Families[0])
                    {
                        using (var calendarFont = new Font(fontFamily, fontSize, FontStyle.Regular,
                            GraphicsUnit.Pixel))
                        {
                            _graphics.DrawString(text, calendarFont, new SolidBrush(textColor), rectangle,
                                stringFormat);
                        }
                    }
                }
            }
        }

        private void setAvgColor()
        {
            _avgColor = _image.CropImage(new Rectangle(_leftMargin - RightMargin, 0, _image.Width, _image.Height / 2))
                              .CalculateAverageColor();
            _avgColor = Color.FromArgb(alpha: 190, baseColor: _avgColor);
            while (_avgColor.Brightness() >= BrightnessThreshold)
            {
                _avgColor = _avgColor.ChangeColorBrightness(-0.01f);
            }
        }

        private void setColors()
        {
            setAvgColor();
            _holidayColor = _avgColor.ChangeColorBrightness(-0.7f);
            _headerColor = _avgColor.ChangeColorBrightness(-0.4f);
        }

        private void setSizes()
        {
            var cellSize = getMaxCellSize(CellMargin);
            _maxCellHeight = (int)cellSize.Height;
            _maxCellWidth = (int)cellSize.Width;
            _leftMargin = _image.Width - (RightMargin + (7 * _maxCellWidth));
        }
    }
}