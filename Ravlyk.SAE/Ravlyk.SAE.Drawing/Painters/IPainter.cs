using System;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Painters
{
	public interface IPainter
	{
		void DrawHorizontalLine(int x, int y, int length, int argb, int width = 1);

		void DrawVerticalLine(int x, int y, int length, int argb, int width = 1);

		void FillRectangle(int x, int y, int width, int height, int argb);

		void PaintText(string text, Point atPoint, int pixelHeight, int argb = 0, int spaceBetweenCharacters = 1, FontBasePainter.TextDirection direction = FontBasePainter.TextDirection.LeftToRight);

		Size GetTextSize(string text, int pixelHeight, int spaceBetweenCharacters = 1);

		void PaintSymbol(char symbol, Point atPoint, int symbolSize, int symbolBoxSize, int fontRgb = 0, int backgroundRgb = 0x00ffffff);

		void PaintImage(IndexedImage image, Point atPoint);

		IDisposable TranslateCoordinates(Size shift);

		IDisposable Clip(Rectangle rect);
		Rectangle ClipRect { get; }

		bool SupportsMultithreading { get; }
	}
}
