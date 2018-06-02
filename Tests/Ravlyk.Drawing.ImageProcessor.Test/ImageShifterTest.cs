using System;

using NUnit.Framework;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	public class ImageShifterTest
	{
		[Test]
		public void TestShiftPixels()
		{
			var srcImage = new IndexedImage { Size = new Size(10, 8) };
			for (int y = 0; y < srcImage.Size.Height; y++)
			{
				for (int x = 0; x < srcImage.Size.Width; x++)
				{
					srcImage.Pixels[y * srcImage.Size.Width + x] = y * 100 + x;
				}
			}

			AssertShiftPixels(srcImage, 0, 0, true);

			AssertShiftPixels(srcImage, 0, 3, false);
			AssertShiftPixels(srcImage, 0, -3, false);
			AssertShiftPixels(srcImage, 2, 0, false);
			AssertShiftPixels(srcImage, 2, 3, false);
			AssertShiftPixels(srcImage, 2, -3, false);
			AssertShiftPixels(srcImage, -2, 0, false);
			AssertShiftPixels(srcImage, -2, 3, false);
			AssertShiftPixels(srcImage, -2, -3, false);

			AssertShiftPixels(srcImage, 10, 0, true);
			AssertShiftPixels(srcImage, -10, 0, true);
			AssertShiftPixels(srcImage, 0, 8, true);
			AssertShiftPixels(srcImage, 0, -8, true);
		}

		void AssertShiftPixels(IndexedImage image, int dx, int dy, bool shouldBeSame)
		{
			var sourcePixels = new int[image.Pixels.Length];
			image.Pixels.CopyTo(sourcePixels, 0);

			ImageShifter.ShiftPixels(image, dx, dy);

			for (int y = 0; y < image.Size.Height; y++)
			{
				for (int x = 0; x < image.Size.Width; x++)
				{
					var srcIndex = y * image.Size.Width + x;

					if (shouldBeSame)
					{
						Assert.AreEqual(sourcePixels[srcIndex], image.Pixels[srcIndex]);
					}
					else
					{
						var sy = y + dy;
						var sx = x + dx;
						if (sy >= 0 && sy < image.Size.Height && sx >= 0 && sx < image.Size.Width)
						{
							var dstIndex = sy * image.Size.Width + sx;
							Assert.AreEqual(sourcePixels[srcIndex], image.Pixels[dstIndex]);
						}
					}
				}
			}
		}
	}
}
