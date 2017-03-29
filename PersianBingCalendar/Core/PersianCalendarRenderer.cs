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
        private const int CellMargin = 15;
        private const int RightMargin = 100;
        private readonly int _day;
        private readonly Graphics _graphics;
        private readonly Image _image;
        private readonly int _month;
        private readonly int _year;
        private Color _avgColor;
        private int _cellHeight;
        private int _cellWidth;
        private bool _disposed;
        private Color _holidayColor;
        private int _leftMargin;
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

        public Color TodayColor { set; get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Image DrawThisMonthsCalendar()
        {
            setColors();
            setSizes();
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
                    var left = _leftMargin + col * _cellWidth;
                    var top = _topMargin + row * _cellHeight;
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
            var lastPoint = new Point(_leftMargin, _topMargin + 7 * _cellHeight);
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

                if (curDay >= _day)
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
            printPoint(point, dayText, _cellWidth, fillColor);
        }

        private void drawDaysOfWeek()
        {
            for (var i = 6; i >= 0; i--)
            {
                var dayOfWeekAbbreviation = PersianCalendarNames.DaysOfWeek[6 - i];
                var point = _points[i];
                printPoint(point, dayOfWeekAbbreviation, _cellWidth, null, applyDarkerColor: true);
            }
        }

        private string getFontPath()
        {
            return Path.Combine(DirUtils.GetAppPath(), "fonts", CalendarFontFileName);
        }

        private string getHeaderText()
        {
            var todayText = $"{_day} {PersianCalendarNames.MonthNames[_month - 1]} {_year.ToString(CultureInfo.InvariantCulture).ToPersianNumbers()}";
            return $"{todayText} {Environment.NewLine} {DateTime.Now.ToString("d MMMM yyyy", new CultureInfo("ar-SA"))} {Environment.NewLine} {DateTime.Now.ToString("yyyy, d MMMM", CultureInfo.InvariantCulture)}";
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
                sizes.Add(measureString(weekDay));
            }

            sizes.Add(measureString("31"));

            var maxHeight = sizes.Select(x => x.Height).Max();
            var maxWidth = sizes.Select(x => x.Width).Max();

            return new SizeF(maxWidth + margin, maxHeight + margin);
        }

        private SizeF measureString(string text)
        {
            var fontPath = getFontPath();
            return DrawingExtensions.MeasureString(text, CalendarFontSize, FontStyle.Regular, fontPath);
        }
        private void printCopyright()
        {
            SizeF copyrightSize;
            using (var font = new Font(CopyrightFontName, CopyrightFontSize))
            {
                copyrightSize = DrawingExtensions.MeasureString(CopyrightText, font);
            }

            var copyrightLeftMargin = (int)(_image.Width - (BingLogoRightMargin + copyrightSize.Width + 5));
            printPoint(new Point(copyrightLeftMargin, _image.Height - 120), CopyrightText, (int)copyrightSize.Width + 10, null, isRtl: false);
        }

        private void printHeaderText()
        {
            var headerText = getHeaderText();

            var size = measureString(headerText);
            _topMargin = (int)size.Height + 100;

            var printHeaderTextPoint = new Point(_leftMargin, _topMargin - _cellHeight);
            printPoint(printHeaderTextPoint, headerText, 7 * _cellWidth);
        }

        private void printHolidayTexts(IList<HolidayItem> items, Point lastPoint)
        {
            var space = 2;
            var dayTextSize = measureString("31").Width;

            foreach (var item in items)
            {
                var size = measureString(item.Text).Width;

                var dayLeftMargin = (int)(_image.Width - (RightMargin + dayTextSize + 5));
                var textPoint = new Point(dayLeftMargin, lastPoint.Y + space * _cellHeight);
                printPoint(textPoint, item.Day.ToString(), (int)dayTextSize + 5, item.IsToday ? TodayColor : _holidayColor);

                var textLeftMargin = (int)(_image.Width - (RightMargin + size + dayTextSize + 10));
                textPoint = new Point(textLeftMargin, lastPoint.Y + space * _cellHeight);
                printPoint(textPoint, item.Text, (int)size + 5, item.IsToday ? TodayColor : _holidayColor);

                space++;
            }
        }

        private void printPoint(
            Point point, string dayText, int cellWidth, Color? fillColor = null,
            bool isRtl = true, bool applyDarkerColor = false)
        {
            var pointX = point.X;
            var pointY = point.Y;

            var cellHeight = _cellHeight;

            var textHeight = measureString(dayText).Height;

            if (textHeight > cellHeight)
            {
                cellHeight = (int)textHeight + 20;
                pointY -= cellHeight - _cellHeight;
            }

            var color = fillColor == null ?
                Color.FromArgb(alpha: 190, baseColor: _avgColor) :
                Color.FromArgb(alpha: 110, baseColor: fillColor.Value);

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
            var textColor = color.Brightness() < 140 ? Color.White : Color.Black;

            using (var privateFontCollection = new PrivateFontCollection())
            {
                privateFontCollection.AddFontFile(getFontPath());
                using (var fontFamily = privateFontCollection.Families[0])
                {
                    using (var calendarFont = new Font(fontFamily, CalendarFontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        _graphics.DrawString(dayText, calendarFont, new SolidBrush(textColor), rectangle, stringFormat);
                    }
                }
            }
        }

        private void setColors()
        {
            _avgColor = _image.CropImage(new Rectangle(_image.Width / 2, 0, _image.Width, _image.Height)).CalculateAverageColor();
            _holidayColor = _avgColor.ChangeColorBrightness(-0.7f);
        }

        private void setSizes()
        {
            var cellSize = getMaxCellSize(CellMargin);
            _cellHeight = (int)cellSize.Height;
            _cellWidth = (int)cellSize.Width;
            _leftMargin = _image.Width - (RightMargin + (7 * _cellWidth));
        }
    }
}