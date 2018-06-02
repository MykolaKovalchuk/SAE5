using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	public static class ImagePainter
	{
		public static void FillRect(IndexedImage image, Rectangle rect, int argb)
		{
			rect = CorrectRect(image, rect);

			if (rect.Width <= 0 || rect.Height <= 0)
			{
				return;
			}

			Parallel.For(rect.Top, rect.BottomExclusive,
				y =>
				{
					for (int x = rect.Left, index = y * image.Size.Width + rect.Left; x < rect.RightExclusive; x++, index++)
					{
						image.Pixels[index] = argb;
					}
				});
		}

		public static void FillRect(IndexedImage image, Rectangle rect, Color toColor)
		{
			rect = CorrectRect(image, rect);

			if (rect.Width <= 0 || rect.Height <= 0)
			{
				return;
			}

			for (int y = rect.Top; y < rect.BottomExclusive; y++)
			{
				for (int x = rect.Left; x < rect.RightExclusive; x++)
				{
					image[x, y] = toColor;
				}
			}
		}

		static Rectangle CorrectRect(IndexedImage image, Rectangle rect)
		{
			if (rect.Left < 0)
			{
				rect.Width += rect.Left;
				rect.Left = 0;
			}
			if (rect.Top < 0)
			{
				rect.Height += rect.Top;
				rect.Top = 0;
			}

			var maxWidth = image.Size.Width - rect.Left;
			if (rect.Width > maxWidth)
			{
				rect.Width = maxWidth;
			}

			var maxHeight = image.Size.Height - rect.Top;
			if (rect.Height > maxHeight)
			{
				rect.Height = maxHeight;
			}

			return rect;
		}

		public static void DrawHorizontalLine(IndexedImage image, int x, int y, int length, int argb, int width = 1)
		{
			if (x < 0)
			{
				length += x;
				x = 0;
			}

			var maxLength = image.Size.Width - x;
			if (length >= maxLength)
			{
				length = maxLength;
			}

			var maxWidth = image.Size.Height - y;
			if (width > maxWidth)
			{
				width = maxWidth;
			}

			if (length <= 0 || y < 0 || y >= image.Size.Height)
			{
				return;
			}

			for (int i = 0, startIndex = y * image.Size.Width + x; i < width; i++, startIndex += image.Size.Width)
			{
				DrawLine(image, startIndex, startIndex + length, 1, argb);
			}
		}

		public static void DrawVerticalLine(IndexedImage image, int x, int y, int length, int argb, int width = 1)
		{
			if (y < 0)
			{
				length += y;
				y = 0;
			}

			var maxLength = image.Size.Height - y;
			if (length >= maxLength)
			{
				length = maxLength;
			}

			var maxWidth = image.Size.Width - x;
			if (width > maxWidth)
			{
				width = maxWidth;
			}

			if (length <= 0 || x < 0 || x >= image.Size.Width)
			{
				return;
			}

			for (int i = 0, startIndex = y * image.Size.Width + x, lastIndexExclusive = startIndex + length * image.Size.Width; i < width; i++, startIndex++, lastIndexExclusive++)
			{
				DrawLine(image, startIndex, lastIndexExclusive, image.Size.Width, argb);
			}
		}

		static void DrawLine(IndexedImage image, int startIndex, int lastIndexExclusive, int step, int argb)
		{
			for (int index = startIndex; index < lastIndexExclusive; index += step)
			{
				image.Pixels[index] = argb;
			}
		}

		public static void ShadeImage(IndexedImage image, int argb, IndexedImage maskImage = null)
		{
			Debug.Assert(maskImage == null || maskImage.Size.Equals(image.Size), "maskImage should have same size as image");

			for (int i = 0; i < image.Pixels.Length; i++)
			{
				var shadeArgb = maskImage != null ? ColorBytes.ShadeColor(argb, maskImage.Pixels[i]) : argb;
				image.Pixels[i] = ColorBytes.ShadeColor(image.Pixels[i], shadeArgb);
			}
		}

		/// <summary>
		/// Fill region of one color with specified color.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>Causes <see cref="IndexedImage.PixelChanged"/> event for each changed pixel.</remarks>
		public static void Fill(IndexedImage image, Color color, int x, int y)
		{
			var fromColor = image[x, y];
			if (fromColor.Equals(color))
			{
				return;
			}

			image.Palette.Add(color);
			FloodFill(image, fromColor, color, x, y);
		}

		static void FloodFill(IndexedImage image, Color fromColor, Color toColor, int x, int y)
		{
			if (fromColor.Equals(toColor))
			{
				return;
			}

			var width = image.Size.Width;
			var height = image.Size.Height;
			var startingX = x;

			var steps = new List<int>();

			while (x >= 0 && image[x, y].Equals(fromColor))
			{
				image[x, y] = toColor;
				steps.Add(x);
				x--;
			}

			x = startingX + 1;
			while (x < width && image[x, y].Equals(fromColor))
			{
				image[x, y] = toColor;
				steps.Add(x);
				x++;
			}

			foreach (var step in steps)
			{
				if (y > 0 && image[step, y - 1].Equals(fromColor))
				{
					FloodFill(image, fromColor, toColor, step, y - 1);
				}
				if (y < height - 1 && image[step, y + 1].Equals(fromColor))
				{
					FloodFill(image, fromColor, toColor, step, y + 1);
				}
			}
		}
	}
}
