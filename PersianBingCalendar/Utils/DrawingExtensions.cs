using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace PersianBingCalendar.Utils
{
    public static class DrawingExtensions
    {
        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// every value in the rage 128-145 will give acceptable results:
        /// Color textColor = Brightness(backgroundColor) &lt; 130 ? Colors.White : Colors.Black;
        /// </summary>
        public static int Brightness(this Color c)
        {
            return (int)Math.Sqrt(
               c.R * c.R * .241 +
               c.G * c.G * .691 +
               c.B * c.B * .068);
        }

        public static bool HasEnoughContrast(Color foreColor, Color backColor, double threshold = 70)
        {
            var foreBrightness = Brightness(foreColor);
            var backBrightness = Brightness(backColor);
            return Math.Abs(foreBrightness - backBrightness) > threshold;
        }

        public static Color CalculateAverageColor(this Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr scan0 = srcData.Scan0;

            int red = 0;
            int green = 0;
            int blue = 0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            int count = width * height - dropped;
            if (count == 0)
            {
                return Color.FromArgb(red, green, blue);
            }

            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return Color.FromArgb(avgR, avgG, avgB);
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1.
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static Bitmap CropImage(this Image source, Rectangle crop)
        {
            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public static Rectangle DrawRoundedRectangle(
            Graphics gfx, Rectangle bounds, int cornerRadius, Pen drawPen, Color fillColor)
        {
            int strokeOffset = Convert.ToInt32(Math.Ceiling(drawPen.Width));
            bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);
            drawPen.EndCap = drawPen.StartCap = LineCap.Round;
            GraphicsPath gfxPath = new GraphicsPath();
            gfxPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
            gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
            gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            gfxPath.CloseAllFigures();
            gfx.FillPath(new SolidBrush(fillColor), gfxPath);
            gfx.DrawPath(drawPen, gfxPath);

            return bounds;
        }

        public static Font GetPrivateFont(int fontSize, FontStyle fontStyle, string path)
        {
            var privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(path);
            var fontFamily = privateFontCollection.Families[0];
            return new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);
        }

        public static SizeF MeasureString(string text, Font f)
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    return g.MeasureString(text, f);
                }
            }
        }

        public static SizeF MeasureString(string text, int fontSize, FontStyle fontStyle, string path)
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    using (var privateFontCollection = new PrivateFontCollection())
                    {
                        privateFontCollection.AddFontFile(path);
                        using (var fontFamily = privateFontCollection.Families[0])
                        {
                            using (var font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel))
                            {
                                return g.MeasureString(text, font);
                            }
                        }
                    }
                }
            }
        }
    }
}