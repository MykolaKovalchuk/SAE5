using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageCropManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestOperationsCallsOnImageChanged()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var cropper = new ImageCropManipulator(image);
			var childManipulator = new ImageManipulatorTest.DummyManipulator(cropper);

			AssertActionCallsOnImageChanged(childManipulator, () => cropper.CropRect(new Rectangle(1, 1, 3, 3)));
			AssertActionCallsOnImageChanged(childManipulator, () => cropper.CropArc(new Rectangle(1, 1, 3, 3)));
		}

		[Test]
		public void TestCropRect()
		{
			var srcImage = new CodedImage { Size = new Size(5, 5) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var cropper = new ImageCropManipulator(srcImage);
			cropper.CropRect(new Rectangle(2, 1, 2, 3));
			var dstImage = cropper.ManipulatedImage;

			Assert.AreEqual(new Size(2, 3), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(6, pixels.Length);
				Assert.AreEqual(7, pixels[0]);
				Assert.AreEqual(8, pixels[1]);
				Assert.AreEqual(12, pixels[2]);
				Assert.AreEqual(13, pixels[3]);
				Assert.AreEqual(17, pixels[4]);
				Assert.AreEqual(18, pixels[5]);
			}
		}

		[Test]
		public void TestCropArt()
		{
			var srcImage = new CodedImage { Size = new Size(10, 10) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = 100;
				}
			}

			var cropper = new ImageCropManipulator(srcImage);
			cropper.CropArc(new Rectangle(1, 1, 8, 8));
			var dstImage = cropper.ManipulatedImage;
			dstImage.CompletePalette();

			Assert.AreEqual(new Size(8, 8), dstImage.Size);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[0, 0]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[0, 7]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[7, 0]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[7, 7]);
			Assert.AreEqual(new CodedColor(100), dstImage[4, 4], "Some point in the centre");
		}

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			var srcImage = new CodedImage { Size = new Size(5, 5) };

			var cropper = new ImageCropManipulator(srcImage);
			cropper.CropRect(new Rectangle(2, 1, 2, 3));
			var dstImage = cropper.ManipulatedImage;

			Assert.AreEqual(new Size(2, 3), dstImage.Size);

			srcImage.Size = new Size(8, 6);
			Assert.AreEqual(new Size(2, 3), dstImage.Size, "Manipulated image should not be changed yet.");

			cropper.RestoreManipulations();
			Assert.AreEqual(new Size(8, 6), dstImage.Size, "Image crop should not be saved and restored - just copy source image.");
		}
	}
}

