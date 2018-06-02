using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageRotateControllerTest
	{
		[Test]
		public void TestRotateCW()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var rotator = new ImageRotateController(new ImageRotateManipulator(srcImage));
			rotator.RotateCW();
			var dstImage = rotator.Manipulator.ManipulatedImage;

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
		public void TestRotateCCW()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var rotator = new ImageRotateController(new ImageRotateManipulator(srcImage));
			rotator.RotateCCW();
			var dstImage = rotator.Manipulator.ManipulatedImage;

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
		public void TestFlipHorizontally()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var rotator = new ImageRotateController(new ImageRotateManipulator(srcImage));
			rotator.FlipHorizontally();
			var dstImage = rotator.Manipulator.ManipulatedImage;

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
		public void TestFlipVertically()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var rotator = new ImageRotateController(new ImageRotateManipulator(srcImage));
			rotator.FlipVertically();
			var dstImage = rotator.Manipulator.ManipulatedImage;

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

