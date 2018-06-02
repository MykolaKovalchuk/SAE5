using System;
using System.Collections.Generic;
using System.Drawing;
using Ravlyk.Adopted.PdfSharp.Drawing;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Drawing;
using Point = Ravlyk.Common.Point;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE.Drawing.Painters
{
	public class PdfSharpPainter : BaseGdiPainter, IPainter
	{
		public PdfSharpPainter(Func<IndexedImage, Bitmap> toBitmap)
		{
			ToBitmap = toBitmap;
		}

		public XGraphics PdfGraphics { get; set; }
		Func<IndexedImage, Bitmap> ToBitmap { get; }

		public string FontName { get; set; }

		bool IPainter.SupportsMultithreading => false;

		void IPainter.DrawHorizontalLine(int x, int y, int length, int argb, int width)
		{
			PdfGraphics.DrawLine(GetPen(argb, width), x + Shift.Width - 0.75f, y + Shift.Height, x + Shift.Width + length, y + Shift.Height);
		}

		void IPainter.DrawVerticalLine(int x, int y, int length, int argb, int width)
		{
			PdfGraphics.DrawLine(GetPen(argb, width), x + Shift.Width, y + Shift.Height - 0.75f, x + Shift.Width, y + Shift.Height + length);
		}

		void IPainter.FillRectangle(int x, int y, int width, int height, int argb)
		{
			if ((argb & 0x00ffffff) == 0x00ffffff)
			{
				return; // Do not fill white paint onto white canvas
			}

			PdfGraphics.DrawRectangle(GetBrush(argb), x + Shift.Width - 0.75f, y + Shift.Height - 0.75f, width + 1, height + 1);
		}

		Size IPainter.GetTextSize(string text, int pixelHeight, int spaceBetweenCharacters)
		{
			return GetTextSize(text, GetPdfFont(pixelHeight));
		}

		Size GetTextSize(string text, XFont font)
		{
			var gdiSize = PdfGraphics.MeasureString(text, font);
			return new Size((int)gdiSize.Width, (int)gdiSize.Height);
		}

		void IPainter.PaintImage(IndexedImage image, Point atPoint)
		{
			var bitmap = ToBitmap(image);
			var xImage = XImage.FromGdiPlusImage(bitmap);
			PdfGraphics.DrawImage(xImage, new System.Drawing.Point(atPoint.X + Shift.Width, atPoint.Y + Shift.Height));
		}

		protected override Font CreateNewGdiFont(int pixelHeight)
		{
			return new Font(FontFamily, pixelHeight * 72f / 100f, FontStyle.Regular, GraphicsUnit.Point);
		}

		void IPainter.PaintSymbol(char symbol, Point atPoint, int symbolSize, int symbolBoxSize, int fontRgb, int backgroundRgb)
		{
			((IPainter)this).FillRectangle(atPoint.X, atPoint.Y, symbolBoxSize, symbolBoxSize, backgroundRgb);

			if (fontRgb != backgroundRgb)
			{
				symbolSize = symbolSize * 9 / 10;

				var symbolShift = GetSymbolShift(symbol, symbolSize, symbolBoxSize);

				PdfGraphics.DrawString(
					symbol.ToString(),
					GetPdfFont(symbolSize),
					GetBrush(fontRgb),
					atPoint.X + Shift.Width + symbolShift.Width + 0.5,
					atPoint.Y + Shift.Height + symbolShift.Height + symbolSize * 72.0 / 100.0);
			}
		}

		void IPainter.PaintText(string text, Point atPoint, int pixelHeight, int argb, int spaceBetweenCharacters, FontBasePainter.TextDirection direction)
		{
			var state = PdfGraphics.Save();

			var font = GetPdfFont(pixelHeight);

			if (direction == FontBasePainter.TextDirection.VerticalUpward)
			{
				PdfGraphics.RotateAtTransform(-90, new XPoint(atPoint.X + Shift.Width, atPoint.Y + Shift.Height));
			}

			PdfGraphics.DrawString(text ?? "", font, GetBrush(argb), atPoint.X + Shift.Width, atPoint.Y + Shift.Height + pixelHeight);

			PdfGraphics.Restore(state);
		}

		#region Internal methods

		XBrush GetBrush(int argb)
		{
			argb = unchecked((int)((uint)argb | 0xff000000));
			XBrush brush;
			if (!brushes.TryGetValue(argb, out brush))
			{
				brush = new XSolidBrush(XColor.FromArgb(argb));
				brushes.Add(argb, brush);
			}
			return brush;
		}

		XPen GetPen(int argb, int width)
		{
			var key = (width << 24) | (argb & 0x00ffffff);
			XPen pen;
			if (!pens.TryGetValue(key, out pen))
			{
				argb = unchecked((int)((uint)argb | 0xff000000));
				pen = new XPen(XColor.FromArgb(argb), width / 2.0);
				pens.Add(key, pen);
			}
			return pen;
		}

		XFont GetPdfFont(int emSize)
		{
			XFont font;
			if (!pdfFonts.TryGetValue(emSize, out font))
			{
				font = new XFont(FontName, emSize);
				pdfFonts.Add(emSize, font);
			}
			return font;
		}

		readonly IDictionary<int, XBrush> brushes = new Dictionary<int, XBrush>();
		readonly IDictionary<int, XPen> pens = new Dictionary<int, XPen>();
		readonly IDictionary<int, XFont> pdfFonts = new Dictionary<int, XFont>();

		#endregion
	}
}
