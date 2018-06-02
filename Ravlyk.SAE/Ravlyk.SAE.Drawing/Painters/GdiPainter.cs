using System;
using System.Collections.Generic;
using System.Drawing;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Drawing;
using Point = Ravlyk.Common.Point;
using Size = Ravlyk.Common.Size;
using GdiPoint = System.Drawing.Point;
using GdiColor = System.Drawing.Color;

namespace Ravlyk.SAE.Drawing.Painters
{
	public class GdiPainter : BaseGdiPainter, IPainter
	{
		public GdiPainter(Graphics graphics, Func<IndexedImage, Bitmap> toBitmap, bool skipWhite)
		{
			GdiGraphics = graphics;
			ToBitmap = toBitmap;
			SkipWhite = skipWhite;
		}

		protected Graphics GdiGraphics { get; set; }
		Func<IndexedImage, Bitmap> ToBitmap { get; }
		bool SkipWhite { get; }

		bool IPainter.SupportsMultithreading => false;

		void IPainter.DrawHorizontalLine(int x, int y, int length, int argb, int width)
		{
			GdiGraphics.DrawLine(GetPen(argb, width), x + Shift.Width - 0.75f, y + Shift.Height, x + Shift.Width + length, y + Shift.Height);
		}

		void IPainter.DrawVerticalLine(int x, int y, int length, int argb, int width)
		{
			GdiGraphics.DrawLine(GetPen(argb, width), x + Shift.Width, y + Shift.Height - 0.75f, x + Shift.Width, y + Shift.Height + length);
		}

		void IPainter.FillRectangle(int x, int y, int width, int height, int argb)
		{
			if (SkipWhite && (argb & 0x00ffffff) == 0x00ffffff)
			{
				return; // Do not fill white paint onto white canvas
			}

			GdiGraphics.FillRectangle(GetBrush(argb), x + Shift.Width - 0.75f, y + Shift.Height - 0.75f, width + 1, height + 1);
		}

		void IPainter.PaintImage(IndexedImage image, Point atPoint)
		{
			using (var bitmap = ToBitmap(image))
			{
				GdiGraphics.DrawImage(bitmap, new GdiPoint(atPoint.X + Shift.Width, atPoint.Y + Shift.Height));
			}
		}

		void IPainter.PaintSymbol(char symbol, Point atPoint, int symbolSize, int symbolBoxSize, int fontRgb, int backgroundRgb)
		{
			((IPainter)this).FillRectangle(atPoint.X, atPoint.Y, symbolBoxSize, symbolBoxSize, backgroundRgb);

			if (fontRgb != backgroundRgb)
			{
				symbolSize = symbolSize * 9 / 10;

				var symbolShift = GetSymbolShift(symbol, symbolSize, symbolBoxSize);

				GdiGraphics.DrawString(
					symbol.ToString(),
					GetGdiFont(symbolSize),
					GetBrush(fontRgb),
					atPoint.X + Shift.Width + symbolShift.Width,
					atPoint.Y + Shift.Height + symbolShift.Height);
			}
		}

		void IPainter.PaintText(string text, Point atPoint, int pixelHeight, int argb, int spaceBetweenCharacters, FontBasePainter.TextDirection direction)
		{
			var font = GetGdiFont(pixelHeight);

			var stringFormat = new StringFormat();
			if (direction == FontBasePainter.TextDirection.VerticalDownward || direction == FontBasePainter.TextDirection.VerticalUpward)
			{
				stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
			}

			var verticalShift = 0;
			if (direction == FontBasePainter.TextDirection.VerticalUpward)
			{
				var textSize = GetTextSize(text, font);
				verticalShift = textSize.Width;
			}

			GdiGraphics.DrawString(text, font, GetBrush(argb), atPoint.X + Shift.Width, atPoint.Y + Shift.Height - verticalShift + 1f, stringFormat);
		}

		Size IPainter.GetTextSize(string text, int pixelHeight, int spaceBetweenCharacters)
		{
			return GetTextSize(text, GetGdiFont(pixelHeight));
		}

		Size GetTextSize(string text, Font font)
		{
			var gdiSize = GdiGraphics.MeasureString(text, font);
			return new Size((int)gdiSize.Width, (int)gdiSize.Height);
		}

		#region Internal methods

		Brush GetBrush(int argb)
		{
			argb = unchecked((int)((uint)argb | 0xff000000));
			Brush brush;
			if (!brushes.TryGetValue(argb, out brush))
			{
				brush = new SolidBrush(GdiColor.FromArgb(argb));
				brushes.Add(argb, brush);
			}
			return brush;
		}

		Pen GetPen(int argb, int width)
		{
			var key = (width << 24) | (argb & 0x00ffffff);
			Pen pen;
			if (!pens.TryGetValue(key, out pen))
			{
				argb = unchecked((int)((uint)argb | 0xff000000));
				pen = new Pen(GdiColor.FromArgb(argb), width / 2f);
				pens.Add(key, pen);
			}
			return pen;
		}

		readonly IDictionary<int, Brush> brushes = new Dictionary<int, Brush>();
		readonly IDictionary<int, Pen> pens = new Dictionary<int, Pen>();

		#endregion

		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var brush in brushes.Values)
				{
					brush.Dispose();
				}
				brushes.Clear();

				foreach (var pen in pens.Values)
				{
					pen.Dispose();
				}
				pens.Clear();
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
