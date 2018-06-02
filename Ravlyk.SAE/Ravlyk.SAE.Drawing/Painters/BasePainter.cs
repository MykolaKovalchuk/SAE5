using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Ravlyk.Common;
using Rectangle = Ravlyk.Common.Rectangle;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE.Drawing.Painters
{
	public abstract class BasePainter
	{
		#region Shift

		public IDisposable TranslateCoordinates(Size shift)
		{
			Shift = new Size(Shift.Width + shift.Width, Shift.Height + shift.Height);
			return new DisposableAction(() => { Shift = new Size(Shift.Width - shift.Width, Shift.Height - shift.Height); });
		}

		protected Size Shift { get; set; }

		#endregion

		#region Clip

		public IDisposable Clip(Rectangle clipRect)
		{
			var originalClipRect = ClipRect;
			ClipRect = clipRect;
			return new DisposableAction(() => { ClipRect = originalClipRect; });
		}

		public Rectangle ClipRect { get; private set; }

		#endregion
	}

	public abstract class BaseGdiPainter : BasePainter, IDisposable
	{
		public FontFamily FontFamily { get; set; }

		protected Font GetGdiFont(int pixelHeight)
		{
			Font font;
			if (!gdiFonts.TryGetValue(pixelHeight, out font))
			{
				font = CreateNewGdiFont(pixelHeight);
				gdiFonts.Add(pixelHeight, font);
			}
			return font;
		}

		protected virtual Font CreateNewGdiFont(int pixelHeight)
		{
			return new Font(FontFamily, pixelHeight, FontStyle.Regular, GraphicsUnit.Pixel);
		}

		protected SizeF GetSymbolShift(char symbol, int symbolSize, int symbolBoxSize)
		{
			var key = (symbolSize << 16) | symbol;
			SizeF shift;
			if (!symbolShifts.TryGetValue(key, out shift))
			{
				const int scale = 4;
				var tempBitmapWidth = symbolBoxSize * 3 * scale;
				using (var bitmap = new Bitmap(tempBitmapWidth, tempBitmapWidth, PixelFormat.Format32bppArgb))
				{
					bitmap.SetResolution(100, 100);
					using (var bmpGraphics = Graphics.FromImage(bitmap))
					{
						bmpGraphics.Clear(Color.White);
						bmpGraphics.DrawString(symbol.ToString(), GetGdiFont(symbolSize * scale), Brushes.Black, symbolBoxSize * scale, symbolBoxSize * scale);
					}

					var minX = tempBitmapWidth;
					var minY = tempBitmapWidth;
					var maxX = 0;
					var maxY = 0;
					for (int y = 0; y < tempBitmapWidth; y++)
					{
						for (int x = 0; x < tempBitmapWidth; x++)
						{
							var pixelColor = bitmap.GetPixel(x, y);
							if (pixelColor.R + pixelColor.G + pixelColor.B < 250 * 3)
							{
								if (x < minX)
								{
									minX = x;
								}
								if (y < minY)
								{
									minY = y;
								}
								if (x > maxX)
								{
									maxX = x;
								}
								if (y > maxY)
								{
									maxY = y;
								}
							}
						}
					}
					var width = maxX - minX + 1;
					var height = maxY - minY + 1;

					shift = new SizeF(
						((tempBitmapWidth - width) / 2.0f - minX) / scale,
						((tempBitmapWidth - height) / 2.0f - minY) / scale);
				}

				symbolShifts.Add(key, shift);
			}
			return shift;
		}

		readonly IDictionary<int, SizeF> symbolShifts = new Dictionary<int, SizeF>();
		readonly IDictionary<int, Font> gdiFonts = new Dictionary<int, Font>();

		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var font in gdiFonts.Values)
				{
					font.Dispose();
				}
				gdiFonts.Clear();
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
