using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace PersianBingCalendar.Utils;

public static class DrawingExtensions
{
	public static Color Blend(Color color, Color backColor, double amount)
	{
		byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
		byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
		byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
		return Color.FromArgb(red: r, green: g, blue: b);
	}

	/// <summary>
	///     every value in the rage 128-145 will give acceptable results:
	///     Color textColor = Brightness(backgroundColor) &lt; 130 ? Colors.White : Colors.Black;
	/// </summary>
	public static int Brightness(this Color c) =>
		(int)Math.Sqrt(
			d: (c.R * c.R * .241) +
			(c.G    * c.G * .691) +
			(c.B    * c.B * .068));

	public static bool HasEnoughContrast(Color foreColor, Color backColor, double threshold = 70)
	{
		int foreBrightness = Brightness(c: foreColor);
		int backBrightness = Brightness(c: backColor);
		return Math.Abs(value: foreBrightness - backBrightness) > threshold;
	}

	public static Color CalculateAverageColor(this Bitmap bm)
	{
		int width  = bm.Width;
		int height = bm.Height;
		const int
			minDiversion =
				15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
		int    dropped = 0; // keep track of dropped pixels
		long[] totals  = [0, 0, 0];
		int bppModifier =
			bm.PixelFormat == PixelFormat.Format24bppRgb
				? 3
				: 4; // cutting corners, will fail on anything else but 32 and 24 bit images

		BitmapData srcData = bm.LockBits(rect: new(x: 0, y: 0, width: width, height: height),
			flags: ImageLockMode.ReadOnly, format: bm.PixelFormat);
		int    stride = srcData.Stride;
		IntPtr scan0  = srcData.Scan0;

		int red   = 0;
		int green = 0;
		int blue  = 0;

		unsafe
		{
			byte* p = (byte*)(void*)scan0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int idx = (y * stride) + (x * bppModifier);
					red = p[idx + 2];
					green = p[idx + 1];
					blue = p[idx];
					if ((Math.Abs(value: red - green) > minDiversion) || (Math.Abs(value: red - blue) > minDiversion) ||
						(Math.Abs(value: green - blue) > minDiversion))
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

		int count = (width * height) - dropped;
		if (count == 0)
		{
			return Color.FromArgb(red: red, green: green, blue: blue);
		}

		int avgR = (int)(totals[2] / count);
		int avgG = (int)(totals[1] / count);
		int avgB = (int)(totals[0] / count);

		return Color.FromArgb(red: avgR, green: avgG, blue: avgB);
	}

	/// <summary>
	///     Creates color with corrected brightness.
	/// </summary>
	/// <param name="color">Color to correct.</param>
	/// <param name="correctionFactor">
	///     The brightness correction factor. Must be between -1 and 1.
	///     Negative values produce darker colors.
	/// </param>
	/// <returns>
	///     Corrected <see cref="Color" /> structure.
	/// </returns>
	public static Color ChangeColorBrightness(this Color color, float correctionFactor)
	{
		float red   = color.R;
		float green = color.G;
		float blue  = color.B;

		if (correctionFactor < 0)
		{
			correctionFactor =  1 + correctionFactor;
			red              *= correctionFactor;
			green            *= correctionFactor;
			blue             *= correctionFactor;
		}
		else
		{
			red   = ((255 - red)   * correctionFactor) + red;
			green = ((255 - green) * correctionFactor) + green;
			blue  = ((255 - blue)  * correctionFactor) + blue;
		}

		return Color.FromArgb(alpha: color.A, red: (int)red, green: (int)green, blue: (int)blue);
	}

	public static Bitmap CropImage(this Image source, Rectangle crop)
	{
		Bitmap         bmp = new(width: crop.Width, height: crop.Height);
		using Graphics gr  = Graphics.FromImage(image: bmp);
		gr.DrawImage(image: source, destRect: new(x: 0, y: 0, width: bmp.Width, height: bmp.Height),
			srcRect: crop, srcUnit: GraphicsUnit.Pixel);

		return bmp;
	}

	public static Rectangle DrawRoundedRectangle(
		Graphics gfx, Rectangle bounds, int cornerRadius, Pen drawPen, Color fillColor)
	{
		int strokeOffset = Convert.ToInt32(value: Math.Ceiling(a: drawPen.Width));
		bounds         = Rectangle.Inflate(rect: bounds, x: -strokeOffset, y: -strokeOffset);
		drawPen.EndCap = drawPen.StartCap = LineCap.Round;
		GraphicsPath gfxPath              = new();
		gfxPath.AddArc(x: bounds.X, y: bounds.Y, width: cornerRadius, height: cornerRadius, startAngle: 180,
			sweepAngle: 90);
		gfxPath.AddArc(x: (bounds.X + bounds.Width) - cornerRadius, y: bounds.Y, width: cornerRadius,
			height: cornerRadius, startAngle: 270, sweepAngle: 90);
		gfxPath.AddArc(x: (bounds.X + bounds.Width) - cornerRadius, y: (bounds.Y + bounds.Height) - cornerRadius,
			width: cornerRadius, height: cornerRadius, startAngle: 0, sweepAngle: 90);
		gfxPath.AddArc(x: bounds.X, y: (bounds.Y + bounds.Height) - cornerRadius, width: cornerRadius,
			height: cornerRadius, startAngle: 90, sweepAngle: 90);
		gfxPath.CloseAllFigures();
		gfx.FillPath(brush: new SolidBrush(color: fillColor), path: gfxPath);
		gfx.DrawPath(pen: drawPen, path: gfxPath);

		return bounds;
	}

	public static Font GetPrivateFont(int fontSize, FontStyle fontStyle, string path)
	{
		PrivateFontCollection privateFontCollection = new();
		privateFontCollection.AddFontFile(filename: path);
		FontFamily fontFamily = privateFontCollection.Families[0];
		return new(family: fontFamily, emSize: fontSize, style: fontStyle, unit: GraphicsUnit.Pixel);
	}

	public static SizeF MeasureString(string text, Font f)
	{
		using Bitmap   bmp = new(width: 1, height: 1);
		using Graphics g   = Graphics.FromImage(image: bmp);
		return g.MeasureString(text: text, font: f);
	}

	public static SizeF MeasureString(string text, int fontSize, FontStyle fontStyle, string path)
	{
		using Bitmap                bmp                   = new(width: 1, height: 1);
		using Graphics              g                     = Graphics.FromImage(image: bmp);
		using PrivateFontCollection privateFontCollection = new();
		privateFontCollection.AddFontFile(filename: path);
		using FontFamily fontFamily = privateFontCollection.Families[0];
		using Font font = new(family: fontFamily, emSize: fontSize, style: fontStyle,
			unit: GraphicsUnit.Pixel);
		return g.MeasureString(text: text, font: font);
	}
}