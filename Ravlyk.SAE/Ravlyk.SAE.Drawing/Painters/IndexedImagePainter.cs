using System;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Painters 
{
	public class IndexedImagePainter : BasePainter, IPainter
	{
		public IndexedImage Canvas { get; set; }
		public TrueTypeFont SymbolsFont { get; set; }

		public bool SupportLineWidth { get; set; } = false;

		bool IPainter.SupportsMultithreading => true;

		void IPainter.DrawHorizontalLine(int x, int y, int length, int argb, int width)
		{
			x += Shift.Width;
			y += Shift.Height;

			if (y >= ClipRect.Top && y < ClipRect.BottomExclusive)
			{
				x = Math.Max(x, ClipRect.Left);
				length = Math.Min(length, ClipRect.RightExclusive - x);
				if (length > 0)
				{
					ImagePainter.DrawHorizontalLine(Canvas, x, y, length, argb, SupportLineWidth ? width : 1);
				}
			}
		}

		void IPainter.DrawVerticalLine(int x, int y, int length, int argb, int width)
		{
			x += Shift.Width;
			y += Shift.Height;

			if (x >= ClipRect.Left && x < ClipRect.RightExclusive)
			{
				y = Math.Max(y, ClipRect.Top);
				length = Math.Min(length, ClipRect.BottomExclusive - y);
				if (length > 0)
				{
					ImagePainter.DrawVerticalLine(Canvas, x, y, length, argb, SupportLineWidth ? width : 1);
				}
			}
		}

		void IPainter.FillRectangle(int x, int y, int width, int height, int argb)
		{
			x += Shift.Width;
			y += Shift.Height;

			x = Math.Max(x, ClipRect.Left);
			y = Math.Max(y, ClipRect.Top);
			width = Math.Min(width, ClipRect.RightExclusive - x);
			height = Math.Min(height, ClipRect.BottomExclusive - y);
			ImagePainter.FillRect(Canvas, new Rectangle(x, y, width, height), argb);
		}

		void IPainter.PaintText(string text, Point atPoint, int pixelHeight, int argb, int spaceBetweenCharacters, FontBasePainter.TextDirection direction)
		{
			atPoint = new Point(atPoint.X + Shift.Width, atPoint.Y + Shift.Height);
			GetTextPainter(pixelHeight).PaintText(text, Canvas.Pixels, Canvas.Size, atPoint, ClipRect, spaceBetweenCharacters, argb, direction);
		}

		Size IPainter.GetTextSize(string text, int pixelHeight, int spaceBetweenCharacters)
		{
			return GetTextPainter(pixelHeight).GetTextSize(text, spaceBetweenCharacters);
		}

		void IPainter.PaintSymbol(char symbol, Point atPoint, int symbolSize, int symbolBoxSize, int fontRgb, int backgroundRgb)
		{
			atPoint = new Point(atPoint.X + Shift.Width, atPoint.Y + Shift.Height);
			GetSymbolsPainter(symbolSize, symbolBoxSize).PaintSymbol(symbol, Canvas.Pixels, Canvas.Size, atPoint, ClipRect, fontRgb, backgroundRgb);
		}

		void IPainter.PaintImage(IndexedImage image, Point atPoint)
		{
			atPoint = new Point(atPoint.X + Shift.Width, atPoint.Y + Shift.Height);
			ImageCopier.Copy(image, Canvas, atPoint);
		}

		#region Internal painters

		TextPainter GetTextPainter(int pixelHeight)
		{
			if (cachedTextPainter == null || cachedTextPainter.Font != SymbolsFont || cachedTextPainter.PixelHeight != pixelHeight)
			{
				cachedTextPainter = new TextPainter(SymbolsFont, pixelHeight);
			}
			return cachedTextPainter;
		}
		TextPainter cachedTextPainter;

		CachedSymbolPainter GetSymbolsPainter(int symbolSize, int symbolBoxSize)
		{
			if (cachedSymbolsPainter == null || cachedSymbolsPainter.Font != SymbolsFont || cachedSymbolsPainter.PixelHeight != symbolSize || cachedSymbolsPainter.BoxHeight != symbolBoxSize)
			{
				cachedSymbolsPainter = new CachedSymbolPainter(SymbolsFont, symbolSize, symbolBoxSize);
			}
			return cachedSymbolsPainter;
		}
		CachedSymbolPainter cachedSymbolsPainter;

		#endregion
	}
}
