using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	public class ImageCopierTest
	{
		[Test]
		public void TestCopy()
		{
			AssertCopy(
				new Size(3, 2),
				new[]
				{
					1, 2, 3,
					4, 5, 6
				},
				new Size(5, 5),
				new[]
				{
					11, 12, 13, 14, 15,
					21, 22, 23, 24, 25,
					31, 32, 33, 34, 35,
					41, 42, 43, 44, 45,
					51, 52, 53, 54, 55
				},
				new Point(3, 1),
				new[]
				{
					11, 12, 13, 14, 15,
					21, 22, 23,  1,  2,
					31, 32, 33,  4,  5,
					41, 42, 43, 44, 45,
					51, 52, 53, 54, 55
				});
		}

		void AssertCopy(Size srcSize, int[] srcPixels, Size destSize, int[] destPixels, Point location, int[] expectedResultPixels)
		{
			var scrImage = new IndexedImage { Size = srcSize };
			srcPixels.CopyTo(scrImage.Pixels, 0);

			var destImage = new IndexedImage { Size = destSize };
			destPixels.CopyTo(destImage.Pixels, 0);

			ImageCopier.Copy(scrImage, destImage, location);

			for (int i = 0; i < expectedResultPixels.Length; i++)
			{
				Assert.AreEqual(expectedResultPixels[i], destImage.Pixels[i], $"Incorrect pixel in position {i}.");
			}
		}
	}
}
