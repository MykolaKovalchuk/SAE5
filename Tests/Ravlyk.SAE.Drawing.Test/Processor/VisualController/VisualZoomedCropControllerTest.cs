using System;

using NUnit.Framework;

using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	class VisualZoomedCropControllerTest
	{
		[Test]
		public void TestVisualZoomedCropController()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			var cropController = new ImageCropController(new ImageCropManipulator(sourceImage));
			var visualZoomedCropController = new VisualZoomCropController(cropController, new Size(60, 50));
			visualZoomedCropController.ZoomPercent = 100;

			Assert.AreSame(sourceImage, visualZoomedCropController.SourceImage);
			Assert.AreSame(cropController, visualZoomedCropController.Controller);
			Assert.AreEqual(100, visualZoomedCropController.ZoomPercent, "ZoomPercent inherited from VisualZoomController.");
			Assert.AreEqual(cropController.CropRect, visualZoomedCropController.CropRect);
		}

		[Test]
		public void TestCropRect()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			var cropController = new ImageCropController(new ImageCropManipulator(sourceImage));
			var visualZoomedCropController = new VisualZoomCropController(cropController, new Size(60, 50));
			visualZoomedCropController.ZoomPercent = 100;

			Assert.AreEqual(100, visualZoomedCropController.ZoomPercent, "Precondition.");

			visualZoomedCropController.SetCropRectForTest(new Rectangle(40, 50, 60, 70));
			Assert.AreEqual(new Rectangle(40, 50, 60, 70), cropController.CropRect);
			Assert.AreEqual(new Rectangle(40, 50, 60, 70), visualZoomedCropController.CropRect);

			visualZoomedCropController.ZoomPercent = 150;
			Assert.AreEqual(new Rectangle(40, 50, 60, 70), cropController.CropRect);
			Assert.AreEqual(new Rectangle(60, 75, 90, 105), visualZoomedCropController.CropRect, "Visual crop rect should be recalculated.");

			visualZoomedCropController.SetCropRectForTest(new Rectangle(30, 45, 60, 75));
			Assert.AreEqual(new Rectangle(20, 30, 40, 50), cropController.CropRect);
			Assert.AreEqual(new Rectangle(30, 45, 60, 75), visualZoomedCropController.CropRect);

			visualZoomedCropController.ZoomPercent = 50;
			Assert.AreEqual(new Rectangle(20, 30, 40, 50), cropController.CropRect);
			Assert.AreEqual(new Rectangle(10, 15, 20, 25), visualZoomedCropController.CropRect, "Visual crop rect should be recalculated.");
		}

		[Test]
		public void TestGetTouchPointerStyle()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			var cropController = new ImageCropController(new ImageCropManipulator(sourceImage));
			var visualZoomedCropController = new VisualZoomCropController(cropController, new Size(60, 50));
			visualZoomedCropController.ZoomPercent = 100;

			visualZoomedCropController.CropKind = ImageCropper.CropKind.Rectangle;
			visualZoomedCropController.SetCropRectForTest(new Rectangle(20, 10, 160, 130));
			visualZoomedCropController.ZoomPercent = 50;
			Assert.AreEqual(new Rectangle(10, 5, 80, 65), visualZoomedCropController.CropRect);

			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.Shift, false, new Point(0, 0), new Point(99, 74));

			visualZoomedCropController.OnTouched(new Point(0, 0));
			visualZoomedCropController.OnShift(new Point(-20, -10)); // Shift on distance (-20, -10)
			visualZoomedCropController.OnUntouched(new Point(0, 0));
			Assert.AreEqual(new Rectangle(20, 10, 60, 50), visualZoomedCropController.VisualImageFrame);

			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.ResizeAll, false, new Point(0, 0), new Point(20, 30));

			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.ResizeLeftTop_RightBottom, true, new Point(-10, -5), new Point(70, 60));
			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.ResizeRightTop_LeftBottom, true, new Point(70, -5), new Point(-10, 60));
			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.ResizeHorizontal, true, new Point(-10, 27), new Point(70, 27));
			AssertGetTouchPointerStyle(visualZoomedCropController, VisualController.TouchPointerStyle.ResizeVertical, true, new Point(30, -5), new Point(30, 60));
		}

		[Test]
		public void TestShift()
		{
			var sourceImage = new CodedImage { Size = new Size(200, 150) };
			var cropController = new ImageCropController(new ImageCropManipulator(sourceImage));
			var visualZoomedCropController = new VisualZoomCropController(cropController, new Size(60, 50));

			using (visualZoomedCropController.SuspendUpdateVisualImage())
			{
				visualZoomedCropController.ZoomPercent = 50;
				visualZoomedCropController.CropKind = ImageCropper.CropKind.Rectangle;
				visualZoomedCropController.SetCropRectForTest(new Rectangle(0, 0, 100, 75));
			}

			visualZoomedCropController.OnTouched(new Point(20, 20));
			visualZoomedCropController.OnShift(new Point(0, 10)); // Shift on distance (-20, -10)
			visualZoomedCropController.OnUntouched(new Point(0, 10));

			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(-10, -5), new Size(5, 10), new Rectangle(15, 15, 45, 30)); // Left-top
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(40, -5), new Size(5, 10), new Rectangle(10, 15, 55, 30)); // Right-top
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(-10, 35), new Size(5, 10), new Rectangle(15, 5, 45, 50)); // Left-bottom
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(40, 35), new Size(5, 10), new Rectangle(10, 5, 55, 50)); // Right-bottom
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(-10, 15), new Size(5, 10), new Rectangle(15, 5, 45, 40)); // Left
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(15, -5), new Size(5, 10), new Rectangle(10, 15, 50, 30)); // Top
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(40, 15), new Size(5, 10), new Rectangle(10, 5, 55, 40)); // Right
			AssertShift(visualZoomedCropController, new Rectangle(10, 5, 50, 40), new Point(15, 35), new Size(5, 10), new Rectangle(10, 5, 50, 50)); // Bottom
		}

		[Test]
		public void TestUpdateVisualImageCore()
		{
			var sourceImage = new CodedImage { Size = new Size(80, 60) };
			int[] sourcePixels;
			using (sourceImage.LockPixels(out sourcePixels))
			{
				for (int i = 0; i < sourcePixels.Length; i++)
				{
					sourcePixels[i] = 0x00FF7700;
				}
			}

			var cropController = new ImageCropController(new ImageCropManipulator(sourceImage));
			var visualZoomedCropController = new VisualZoomCropController(cropController, sourceImage.Size); // To keep simple without points translation

			Assert.AreEqual(100, visualZoomedCropController.ZoomPercent, "Precondition.");

			using (visualZoomedCropController.SuspendUpdateVisualImage())
			{
				visualZoomedCropController.CropKind = ImageCropper.CropKind.Rectangle;
				visualZoomedCropController.SetCropRectForTest(new Rectangle(10, 10, 55, 35));
				visualZoomedCropController.UpdateVisualImage();
			}

			AssertVisualColor(visualZoomedCropController, 0x00FF7700, "Should be source color.", new Point(30, 20), new Point(40, 30));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700.HalfToneLow(), "Should be darkened color on sizers.", new Point(10, 10), new Point(35, 10), new Point(65, 10), new Point(10, 25), new Point(65, 10), new Point(10, 45), new Point(35, 45), new Point(65, 45));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700.ThreeQuarterToneHigh(), "Should be faded color outside crop region.", new Point(0, 0), new Point(70, 50));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700, "Should be source color in corner of rectangle.", new Point(16, 10), new Point(10, 16));

			using (visualZoomedCropController.SuspendUpdateVisualImage())
			{
				visualZoomedCropController.CropKind = ImageCropper.CropKind.Arc;
				visualZoomedCropController.UpdateVisualImage();
			}

			AssertVisualColor(visualZoomedCropController, 0x00FF7700, "Should be source color.", new Point(30, 20), new Point(40, 30));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700.HalfToneLow(), "Should be darkened color on sizers.", new Point(10, 10), new Point(35, 10), new Point(65, 10), new Point(10, 25), new Point(65, 25), new Point(10, 45), new Point(35, 45), new Point(65, 45));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700.ThreeQuarterToneHigh(), "Should be faded color outside crop region.", new Point(0, 0), new Point(70, 50));
			AssertVisualColor(visualZoomedCropController, 0x00FF7700.ThreeQuarterToneHigh(), "Should be faded color in corner of arc.", new Point(16, 10), new Point(10, 16));
		}

		#region Implementation

		void AssertGetTouchPointerStyle(VisualController visualController, VisualController.TouchPointerStyle expectedPointerStyle, bool checkNearby, params Point[] points)
		{
			foreach (var point in points)
			{
				Assert.AreEqual(expectedPointerStyle, visualController.GetTouchPointerStyle(point), "Should match pointer style at the point.");
				if (checkNearby)
				{
					Assert.AreEqual(expectedPointerStyle, visualController.GetTouchPointerStyle(new Point(point.X - 2, point.Y - 2)), "Should match pointer style on small distance from point.");
					Assert.AreEqual(expectedPointerStyle, visualController.GetTouchPointerStyle(new Point(point.X + 2, point.Y + 2)), "Should match pointer style on small distance from point.");
					Assert.AreNotEqual(expectedPointerStyle, visualController.GetTouchPointerStyle(new Point(point.X - 8, point.Y - 8)), "Should not match pointer style on big distance from point.");
					Assert.AreNotEqual(expectedPointerStyle, visualController.GetTouchPointerStyle(new Point(point.X + 8, point.Y + 8)), "Should not match pointer style on big distance from point.");
				}
			}
		}

		void AssertShift(VisualZoomCropController visualZoomCropController, Rectangle initialCropRect, Point initialPoint, Size shiftSize, Rectangle expectedCropRect)
		{
			visualZoomCropController.SetCropRectForTest(initialCropRect);

			visualZoomCropController.OnTouched(initialPoint);
			var newPoint = new Point(initialPoint.X + shiftSize.Width, initialPoint.Y + shiftSize.Height);
			visualZoomCropController.OnShift(newPoint);

			Assert.AreEqual(expectedCropRect, visualZoomCropController.CropRect);

			visualZoomCropController.OnUntouched(newPoint); // To prevent further unexpected shifting
		}

		void AssertVisualColor(VisualController visualController, int expectedColor, string message, params Point[] points)
		{
			int[] visualPixels;
			using (visualController.VisualImage.LockPixels(out visualPixels))
			{
				foreach (var point in points)
				{
					Assert.AreEqual(expectedColor, visualPixels[point.Y * visualController.VisualImage.Size.Width + point.X], message + string.Format(" ({0}, {1})", point.X, point.Y));
				}
			}
		}

		#endregion
	}
}
