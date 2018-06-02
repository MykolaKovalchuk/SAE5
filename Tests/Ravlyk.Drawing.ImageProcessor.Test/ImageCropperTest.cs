using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	public class ImageCropperTest
	{
		[Test]
		public void TestCropRect()
		{
			var srcImage = new IndexedImage { Size = new Size(5, 5) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var dstImage = ImageCropper.Crop(srcImage, new Rectangle(2, 1, 2, 3), ImageCropper.CropKind.Rectangle);

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
		public void TestCropArc()
		{
			var srcImage = new IndexedImage { Size = new Size(10, 10) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = 100;
				}
			}

			var dstImage = ImageCropper.Crop(srcImage, new Rectangle(1, 1, 8, 8), ImageCropper.CropKind.Arc);
			dstImage.CompletePalette();

			Assert.AreEqual(new Size(8, 8), dstImage.Size);
			Assert.AreEqual(new Color(255, 255, 255), dstImage[0, 0]);
			Assert.AreEqual(new Color(255, 255, 255), dstImage[0, 7]);
			Assert.AreEqual(new Color(255, 255, 255), dstImage[7, 0]);
			Assert.AreEqual(new Color(255, 255, 255), dstImage[7, 7]);
			Assert.AreEqual(new Color(100), dstImage[4, 4], "Some point in the centre");
		}

		[Test]
		public void TestIsPointInsideArc()
		{
			var cropRect = new Rectangle(1, 1, 8, 8);

			Assert.IsFalse(ImageCropper.IsPointInsideArc(new Point(0, 0), cropRect));
			Assert.IsFalse(ImageCropper.IsPointInsideArc(new Point(0, 7), cropRect));
			Assert.IsFalse(ImageCropper.IsPointInsideArc(new Point(7, 0), cropRect));
			Assert.IsFalse(ImageCropper.IsPointInsideArc(new Point(7, 7), cropRect));
			Assert.IsTrue(ImageCropper.IsPointInsideArc(new Point(4, 4), cropRect), "Some point in the centre");
		}
	}
}

