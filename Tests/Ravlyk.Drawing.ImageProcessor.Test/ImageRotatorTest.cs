using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	public class ImageRotatorTest
	{
		[Test]
		public void TestRotateCWInPlace()
		{
			var srcImage = new IndexedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var dstImage = ImageRotator.RotateCWInPlace(srcImage);

			Assert.AreSame(srcImage, dstImage);
			Assert.AreEqual(new Size(2, 3), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(3, pixels[0]);
				Assert.AreEqual(0, pixels[1]);
				Assert.AreEqual(4, pixels[2]);
				Assert.AreEqual(1, pixels[3]);
				Assert.AreEqual(5, pixels[4]);
				Assert.AreEqual(2, pixels[5]);
			}
		}

		[Test]
		public void TestRotateCCWInPlace()
		{
			var srcImage = new IndexedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var dstImage = ImageRotator.RotateCCWInPlace(srcImage);

			Assert.AreSame(srcImage, dstImage);
			Assert.AreEqual(new Size(2, 3), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(2, pixels[0]);
				Assert.AreEqual(5, pixels[1]);
				Assert.AreEqual(1, pixels[2]);
				Assert.AreEqual(4, pixels[3]);
				Assert.AreEqual(0, pixels[4]);
				Assert.AreEqual(3, pixels[5]);
			}
		}

		[Test]
		public void TestFlipHorizontallyInPlace()
		{
			var srcImage = new IndexedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var dstImage = ImageRotator.FlipHorizontallyInPlace(srcImage);

			Assert.AreSame(srcImage, dstImage);
			Assert.AreEqual(new Size(3, 2), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(2, pixels[0]);
				Assert.AreEqual(1, pixels[1]);
				Assert.AreEqual(0, pixels[2]);
				Assert.AreEqual(5, pixels[3]);
				Assert.AreEqual(4, pixels[4]);
				Assert.AreEqual(3, pixels[5]);
			}
		}

		[Test]
		public void TestFlipVerticallyInPlace()
		{
			var srcImage = new IndexedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var dstImage = ImageRotator.FlipVerticallyInPlace(srcImage);

			Assert.AreSame(srcImage, dstImage);
			Assert.AreEqual(new Size(3, 2), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(3, pixels[0]);
				Assert.AreEqual(4, pixels[1]);
				Assert.AreEqual(5, pixels[2]);
				Assert.AreEqual(0, pixels[3]);
				Assert.AreEqual(1, pixels[4]);
				Assert.AreEqual(2, pixels[5]);
			}
		}
	}
}

