using System;

using NUnit.Framework;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	class ImagePainterTest
	{
		[Test]
		public void TestFillRect()
		{
			var image = new IndexedImage { Size = new Size(5, 5) };
			ImagePainter.FillRect(image, new Rectangle(2, 1, 4, 2), 1);
			AssertImage(
				new[]
				{
					0, 0, 0, 0, 0,
					0, 0, 1, 1, 1,
					0, 0, 1, 1, 1,
					0, 0, 0, 0, 0,
					0, 0, 0, 0, 0
				},
				image);

			ImagePainter.FillRect(image, new Rectangle(1, 2, 2, 2), 2);
			AssertImage(
				new[]
				{
					0, 0, 0, 0, 0,
					0, 0, 1, 1, 1,
					0, 2, 2, 1, 1,
					0, 2, 2, 0, 0,
					0, 0, 0, 0, 0
				},
				image);
		}

		[Test]
		public void TestDrawHorizontalLine()
		{
			var image = new IndexedImage { Size = new Size(5, 5) };
			ImagePainter.DrawHorizontalLine(image, 1, 1, 5, 1, 2);
			AssertImage(
				new[]
				{
					0, 0, 0, 0, 0,
					0, 1, 1, 1, 1,
					0, 1, 1, 1, 1,
					0, 0, 0, 0, 0,
					0, 0, 0, 0, 0
				},
				image);

			ImagePainter.DrawHorizontalLine(image, 0, 3, 4, 2, 1);
			AssertImage(
				new[]
				{
					0, 0, 0, 0, 0,
					0, 1, 1, 1, 1,
					0, 1, 1, 1, 1,
					2, 2, 2, 2, 0,
					0, 0, 0, 0, 0
				},
				image);
		}

		[Test]
		public void TestDrawVerticalLine()
		{
			var image = new IndexedImage { Size = new Size(5, 5) };
			ImagePainter.DrawVerticalLine(image, 1, 1, 5, 1, 2);
			AssertImage(
				new[]
				{
					0, 0, 0, 0, 0,
					0, 1, 1, 0, 0,
					0, 1, 1, 0, 0,
					0, 1, 1, 0, 0,
					0, 1, 1, 0, 0
				},
				image);

			ImagePainter.DrawVerticalLine(image, 3, 0, 4, 2, 1);
			AssertImage(
				new[]
				{
					0, 0, 0, 2, 0,
					0, 1, 1, 2, 0,
					0, 1, 1, 2, 0,
					0, 1, 1, 2, 0,
					0, 1, 1, 0, 0
				},
				image);
		}

		void AssertImage(int[] expectedPixels, IndexedImage image)
		{
			Assert.AreEqual(expectedPixels.Length, image.Pixels.Length);
			for (int i = 0; i < expectedPixels.Length; i++)
			{
				Assert.AreEqual(expectedPixels[i], image.Pixels[i], "Pixel in position " + i);
			}
		}
	}
}
