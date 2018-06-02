using System;

using NUnit.Framework;

using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	class VisualControllerTest
	{
		[Test]
		public void TestVisualController()
		{
			var sourceImage = new CodedImage { Size = new Size(4, 3) };
			var visualController = new VisualControllerForTest(sourceImage);

			Assert.AreSame(sourceImage, visualController.SourceImage);
			Assert.AreNotSame(sourceImage, visualController.VisualImage);
		}

		[Test]
		public void TestTranslatePoint()
		{
			var visualController = new VisualControllerForTest(new CodedImage { Size = new Size(4, 3) });

			Assert.AreEqual(new Point(4, 6), visualController.TranslateExposedForTest(new Point(3, 4)), "Do simple shift (+1, +2) for test purposes.");

			visualController.OnTouched(new Point(5, 5));
			Assert.AreEqual(new Point(6, 7), visualController.LastProcessedPoint);

			visualController.OnUntouched(new Point(2, 1));
			Assert.AreEqual(new Point(3, 3), visualController.LastProcessedPoint);

			visualController.OnShift(new Point(3, 5));
			Assert.AreEqual(new Point(3, 3), visualController.LastProcessedPoint, "Previous point should have been passed to OnShiftCore method.");
			Assert.AreEqual(new Size(1, 4), visualController.LastProcessedShift, "Shift value should not be translated.");
		}

		[Test]
		public void TestIsTouching()
		{
			var visualController = new VisualControllerForTest(new CodedImage { Size = new Size(4, 3) });

			Assert.IsFalse(visualController.IsTouchingExposedForTest, "Should be false by default.");

			visualController.OnTouched(new Point(5, 5));
			Assert.IsTrue(visualController.IsTouchingExposedForTest, "Should be set to true.");

			visualController.OnTouched(new Point(3, 3));
			Assert.IsTrue(visualController.IsTouchingExposedForTest, "Should remain true.");

			visualController.OnShift(new Point(4, 4));
			Assert.IsTrue(visualController.IsTouchingExposedForTest, "Still should remain true.");

			visualController.OnUntouched(new Point(4, 5));
			Assert.IsFalse(visualController.IsTouchingExposedForTest, "Should be reset to false - no matter how many OnTouched has occurred before.");
		}

		[Test]
		public void TestShift()
		{
			var visualController = new VisualControllerForTest(new CodedImage { Size = new Size(4, 3) });

			visualController.OnTouched(new Point(3, 3));

			visualController.OnShift(new Point(4, 5));
			Assert.AreEqual(new Size(0, 0), visualController.LastProcessedShift, "Shift is not initialized for small noise.");

			visualController.OnShift(new Point(9, 8));
			Assert.AreEqual(new Size(6, 5), visualController.LastProcessedShift);

			visualController.OnShift(new Point(2, 2));
			Assert.AreEqual(new Size(-7, -6), visualController.LastProcessedShift);

			visualController.OnShift(new Point(2, 2));
			Assert.AreEqual(new Size(0, 0), visualController.LastProcessedShift);
		}

		#region Test class

		class VisualControllerForTest : VisualController
		{
			public VisualControllerForTest(IImageProvider imageProvider) : base(imageProvider) { }

			protected override void OnSourceImageChanged()
			{
			}

			protected override Point TranslatePoint(Point controllerPoint)
			{
				return new Point(controllerPoint.X + 1, controllerPoint.Y + 2);
			}

			public Point TranslateExposedForTest(Point controlPoint)
			{
				return TranslatePoint(controlPoint);
			}

			public bool IsTouchingExposedForTest => IsTouching;

			protected override void OnTouchedCore(Point imagePoint)
			{
				base.OnTouchedCore(imagePoint);
				LastProcessedPoint = imagePoint;
			}

			protected override void OnUntouchedCore(Point imagePoint)
			{
				base.OnUntouchedCore(imagePoint);
				LastProcessedPoint = imagePoint;
			}

			protected override void OnShiftCore(Point imagePoint, Size shiftSize)
			{
				base.OnShiftCore(imagePoint, shiftSize);
				LastProcessedPoint = imagePoint;
				LastProcessedShift = shiftSize;
			}

			public Point LastProcessedPoint { get; private set; }
			public Size LastProcessedShift { get; private set; }
		}

		#endregion
	}
}
