using System;

using NUnit.Framework;

using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	class VisualZoomControllerTest
	{
		[Test]
		public void TestVisualZoomController()
		{
			var sourceImage = CreateSourceImage();
			var visualZoomController = new VisualZoomController(sourceImage, new Size(60, 50));
			visualZoomController.ZoomPercent = 100;

			Assert.AreEqual(new Size(60, 50), visualZoomController.ImageBoxSize);
			Assert.AreEqual(100, visualZoomController.ZoomPercent, "Default zoom percent value.");
			Assert.AreEqual(ImageResampler.FilterType.Box, visualZoomController.UpscaleFilterType, "Default upscale filter.");
			Assert.AreEqual(ImageResampler.FilterType.Lanczos3, visualZoomController.DownscaleFilterType, "Default downscale filter.");
		}

		#region TestInitialZoom

		[Test]
		public void TestInitialZoom()
		{
			AssertInitialZoom(new Size(200, 300), new Size(200, 300), 100, new Size(200, 300));
			AssertInitialZoom(new Size(200, 300), new Size(200, 150), 50, new Size(100, 150));
			AssertInitialZoom(new Size(200, 300), new Size(150, 300), 75, new Size(150, 225));
			AssertInitialZoom(new Size(200, 300), new Size(400, 500), 100, new Size(200, 300));
			AssertInitialZoom(new Size(200, 300), new Size(100, 100), 33, new Size(66, 99));
		}

		void AssertInitialZoom(Size imageSize, Size imageBoxSize, int expectedZoom, Size expectedVisualSize)
		{
			var sourceImage = new CodedImage { Size = imageSize };
			var visualZoomController = new VisualZoomController(sourceImage, imageBoxSize);

			Assert.AreEqual(expectedZoom, visualZoomController.ZoomPercent);
			Assert.AreEqual(expectedVisualSize, visualZoomController.VisualImage.Size);
		}

		#endregion

		[Test]
		public void TestUpdateParameters()
		{
			var sourceImage = CreateSourceImage();
			var visualZoomController = new VisualZoomController(sourceImage, new Size(60, 50));
			visualZoomController.ZoomPercent = 100;

			Assert.AreEqual(100, visualZoomController.ZoomPercent);
			Assert.AreEqual(sourceImage.Size, visualZoomController.ZoomedImageSize);
			Assert.AreEqual(new Rectangle(0, 0, 60, 50), visualZoomController.VisualImageFrame);
			Assert.AreEqual(new Point(0, 0), visualZoomController.ImageLocation);

			visualZoomController.ZoomPercent = 25;

			Assert.AreEqual(new Size(50, 37), visualZoomController.ZoomedImageSize);
			Assert.AreEqual(new Rectangle(0, 0, 50, 37), visualZoomController.VisualImageFrame);
			Assert.AreEqual(new Point(5, 6), visualZoomController.ImageLocation);

			visualZoomController.ZoomPercent = 150;

			Assert.AreEqual(new Size(300, 225), visualZoomController.ZoomedImageSize);
			Assert.AreEqual(new Rectangle(0, 0, 60, 50), visualZoomController.VisualImageFrame);
			Assert.AreEqual(new Point(0, 0), visualZoomController.ImageLocation);
		}

		[Test]
		public void TestUpdateVisualImage()
		{
			var sourceImage = CreateSourceImage();
			var visualZoomController = new VisualZoomController(sourceImage, new Size(60, 50));
			visualZoomController.ZoomPercent = 100;

			Assert.AreEqual(100, visualZoomController.ZoomPercent);
			Assert.AreEqual(sourceImage.Size, visualZoomController.ZoomedImageSize);
			Assert.AreEqual(new Rectangle(0, 0, 60, 50), visualZoomController.VisualImageFrame);
			Assert.AreEqual(new Point(0, 0), visualZoomController.ImageLocation);

			AssertVisualImageFrameForZoom100(visualZoomController.VisualImage, visualZoomController.VisualImageFrame, sourceImage.Size);
		}

		[Test]
		public void TestGetTouchPointerStyle()
		{
			var sourceImage = CreateSourceImage();
			var visualZoomController = new VisualZoomController(sourceImage, new Size(60, 50));

			visualZoomController.ZoomPercent = 25;

			Assert.AreEqual(new Point(5, 6), visualZoomController.ImageLocation, "Precondition.");

			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(5, 6)), "Image is too small to shift.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(10, 10)), "Image is too small to shift.");

			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(0, 0)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(0, 10)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(59, 10)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(10, 0)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(10, 49)), "Outside of image.");

			visualZoomController.ZoomPercent = 100;

			Assert.IsTrue(visualZoomController.ZoomedImageSize.Width > visualZoomController.ImageBoxSize.Width, "Precondition.");
			Assert.IsTrue(visualZoomController.ZoomedImageSize.Height > visualZoomController.ImageBoxSize.Height, "Precondition.");

			Assert.AreEqual(VisualController.TouchPointerStyle.Shift, visualZoomController.GetTouchPointerStyle(new Point(0, 0)));
			Assert.AreEqual(VisualController.TouchPointerStyle.Shift, visualZoomController.GetTouchPointerStyle(new Point(10, 10)));

			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(-1, -1)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(-1, 10)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(visualZoomController.ZoomedImageSize.Width, 10)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(10, -1)), "Outside of image.");
			Assert.AreEqual(VisualController.TouchPointerStyle.None, visualZoomController.GetTouchPointerStyle(new Point(10, visualZoomController.ZoomedImageSize.Height)), "Outside of image.");
		}

		[Test]
		public void TestShift()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			int[] sourcePixels;
			using (sourceImage.LockPixels(out sourcePixels))
			{
				for (int i = 0; i < sourcePixels.Length; i++)
				{
					sourcePixels[i] = i;
				}
			}

			var visualZoomController = new VisualZoomController(sourceImage, new Size(60, 50));
			visualZoomController.ZoomPercent = 100;

			visualZoomController.OnTouched(new Point(20, 20));
			visualZoomController.OnShift(new Point(40, 30));

			Assert.AreEqual(new Rectangle(0, 0, 60, 50), visualZoomController.VisualImageFrame, "Frame should not be moved beyond left-top edges.");

			visualZoomController.OnShift(new Point(30, 25));

			Assert.AreEqual(new Rectangle(10, 5, 60, 50), visualZoomController.VisualImageFrame, "Frame should be shifted on distance (10, 5).");
			AssertVisualImageFrameForZoom100(visualZoomController.VisualImage, visualZoomController.VisualImageFrame, sourceImage.Size);

			visualZoomController.OnShift(new Point(-200, -150));

			Assert.AreEqual(new Rectangle(140, 100, 60, 50), visualZoomController.VisualImageFrame, "Frame should not be moved beyond right-bottom edges.");
			AssertVisualImageFrameForZoom100(visualZoomController.VisualImage, visualZoomController.VisualImageFrame, sourceImage.Size);
		}

		#region Implementation

		CodedImage CreateSourceImage()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			int[] sourcePixels;
			using (sourceImage.LockPixels(out sourcePixels))
			{
				for (int i = 0; i < sourcePixels.Length; i++)
				{
					sourcePixels[i] = i;
				}
			}
			return sourceImage;
		}

		void AssertVisualImageFrameForZoom100(CodedImage visualImage, Rectangle visualImageFrame, Size sourceImageSize)
		{
			int[] visualPixels;
			using (visualImage.LockPixels(out visualPixels))
			{
				for (int y = visualImageFrame.Top; y < visualImageFrame.BottomExclusive; y++)
				{
					for (int x = visualImageFrame.Left; x < visualImageFrame.RightExclusive; x++)
					{
						var sourceIndex = y * sourceImageSize.Width + x;
						var visualIndex = (y - visualImageFrame.Top) * visualImageFrame.Width + (x - visualImageFrame.Left);

						Assert.AreEqual(sourceIndex, visualPixels[visualIndex]);
					}
				}
			}
		}

		#endregion
	}
}
