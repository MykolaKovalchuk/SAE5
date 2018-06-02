using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageRotateManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestOperationsCallsOnImageChanged()
		{
			var image = new CodedImage { Size = new Size(2, 2) };
			var rotator = new ImageRotateManipulator(image);
			var childManipulator = new ImageManipulatorTest.DummyManipulator(rotator);

			AssertActionCallsOnImageChanged(childManipulator, rotator.RotateCW);
			AssertActionCallsOnImageChanged(childManipulator, rotator.RotateCCW);
			AssertActionCallsOnImageChanged(childManipulator, rotator.FlipHorizontally);
			AssertActionCallsOnImageChanged(childManipulator, rotator.FlipVertically);
		}

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

			var rotator = new ImageRotateManipulator(srcImage);
			rotator.RotateCW();
			var dstImage = rotator.ManipulatedImage;

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

			var rotator = new ImageRotateManipulator(srcImage);
			rotator.RotateCCW();
			var dstImage = rotator.ManipulatedImage;

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

			var rotator = new ImageRotateManipulator(srcImage);
			rotator.FlipHorizontally();
			var dstImage = rotator.ManipulatedImage;

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

			var rotator = new ImageRotateManipulator(srcImage);
			rotator.FlipVertically();
			var dstImage = rotator.ManipulatedImage;

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

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			var imageSetter = new ImageSetterManipulator();
			imageSetter.SetNewImage(srcImage);

			var rotator = new ImageRotateManipulator(imageSetter);
			rotator.RotateCW();
			Assert.AreEqual(new Size(2, 3), rotator.ManipulatedImage.Size, "Manipulated image should be rotated.");

			srcImage.Size = new Size(5, 4);
			Assert.AreEqual(new Size(2, 3), rotator.ManipulatedImage.Size, "Manipulated image should not change.");

			imageSetter.SetNewImage(srcImage);
			Assert.AreEqual(new Size(5, 4), rotator.ManipulatedImage.Size, "Flip/Rotation manipulation should not be restored when called by parent manipulator.");

			rotator.RestoreManipulations("W");
			Assert.AreEqual(new Size(4, 5), rotator.ManipulatedImage.Size, "Flip/Rotation manipulation should restored when called manually.");
		}
	}
}

