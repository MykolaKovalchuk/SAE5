using System;
using NUnit.Framework;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageSizeControllerTest
	{
		[Test]
		public void TestDoesntCallManipulationsWithoutChanges()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var resizer = new ImageSizeController4Test(new ImageSizeManipulator(image));

			Assert.IsTrue(resizer.KeepAspect, "Default value");
			Assert.AreEqual(ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight, resizer.SizeLockBy, "Initial value");
			Assert.AreEqual(ImageResampler.FilterType.Lanczos3, resizer.FilterType, "Default value");
			Assert.AreEqual(200, resizer.Width, "Default value");
			Assert.AreEqual(200, resizer.Height, "Initially recalculated height value");

			Assert.IsTrue(resizer.CallManipulationsCoreFired, "CallManipulations should have been fired");
			Assert.AreEqual(new Size(5, 5), resizer.Manipulator.ManipulatedImage.Size, "Overridden CallManipulationsCore() doesn't change image.");

			resizer.CallManipulationsCoreFired = false;

			resizer.KeepAspect = false;
			Assert.IsFalse(resizer.CallManipulationsCoreFired, "Changing KeepAspect option should not call manipulations by itself.");

			resizer.SizeLockBy = ImageSizeManipulator.SizeLockType.None;
			Assert.IsFalse(resizer.CallManipulationsCoreFired, "Changing CallManipulationsCoreFired option should not call manipulations by itself.");

			resizer.Width = 200;
			Assert.IsFalse(resizer.CallManipulationsCoreFired, "Should not call manipulations when there is no changes.");
			resizer.Width = 100;
			Assert.IsTrue(resizer.CallManipulationsCoreFired, "Should call manipulations for changed width.");

			resizer.Height = 200; // Return to initial value
			resizer.CallManipulationsCoreFired = false;

			resizer.Height = 200;
			Assert.IsFalse(resizer.CallManipulationsCoreFired, "Should not call manipulations when there is no changes.");
			resizer.Height = 100;
			Assert.IsTrue(resizer.CallManipulationsCoreFired, "Should call manipulations for changed height.");

			resizer.CallManipulationsCoreFired = false;

			resizer.FilterType = ImageResampler.FilterType.Lanczos3;
			Assert.IsFalse(resizer.CallManipulationsCoreFired, "Should not call manipulations when there is no changes.");
			resizer.FilterType = ImageResampler.FilterType.Box;
			Assert.IsTrue(resizer.CallManipulationsCoreFired, "Should call manipulations for changed filter type.");
		}

		[Test]
		public void TestCallManipulations()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var resizer = new ImageSizeController(new ImageSizeManipulator(image));

			Assert.AreEqual(200, resizer.Width, "Default value");
			Assert.AreEqual(200, resizer.Height, "Initially recalculated height value");
			Assert.AreEqual(new Size(200, 200), resizer.Manipulator.ManipulatedImage.Size, "Default manipulations should be already executed.");

			resizer.Width = 100;

			Assert.AreEqual(100, resizer.Width, "New width");
			Assert.AreEqual(100, resizer.Height, "New recalculated height");
			Assert.AreEqual(new Size(100, 100), resizer.Manipulator.ManipulatedImage.Size, "Manipulated image should be resized.");
		}

		[Test]
		public void TestSetSizeInCorrectLimits()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var resizer = new ImageSizeController4Test(new ImageSizeManipulator(image)); // Use test controller as we don't need to update image
			Assert.AreEqual(200, resizer.Width, "Default value");
			Assert.AreEqual(200, resizer.Height, "Initially recalculated height value");

			resizer.Width = ImageSizeController.MinimumSize - 5;
			Assert.AreEqual(ImageSizeController.MinimumSize, resizer.Width, "Should be changed to minimum width.");
			Assert.AreEqual(ImageSizeController.MinimumSize, resizer.Height, "Height should be recalculated.");

			resizer.Width = ImageSizeController.MaximumSize + 5;
			Assert.AreEqual(ImageSizeController.MaximumSize, resizer.Width, "Should be changed to maximum width.");
			Assert.AreEqual(ImageSizeController.MaximumSize, resizer.Height, "Height should be recalculated.");

			resizer.Height = ImageSizeController.MinimumSize - 5;
			Assert.AreEqual(ImageSizeController.MinimumSize, resizer.Width, "Width should be recalculated.");
			Assert.AreEqual(ImageSizeController.MinimumSize, resizer.Height, "Should be changed to minimum width height.");

			resizer.Height = ImageSizeController.MaximumSize + 5;
			Assert.AreEqual(ImageSizeController.MaximumSize, resizer.Width, "Width should be recalculated.");
			Assert.AreEqual(ImageSizeController.MaximumSize, resizer.Height, "Should be changed to maximum width height.");
		}

		[Test]
		public void TestDefaultSettings()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var resizer = new ImageSizeController4Test(new ImageSizeManipulator(image)); // Use test controller as we don't need to update image

			using (resizer.SuspendCallManipulations())
			{
				resizer.KeepAspect = false;
				resizer.Width = 30;
				resizer.Height = 25;
				resizer.FilterType = ImageResampler.FilterType.Box;
			}

			var newResizer = new ImageSizeController4Test(new ImageSizeManipulator(image));

			// Precondition checks
			Assert.IsTrue(newResizer.KeepAspect);
			Assert.AreNotEqual(30, newResizer.Width);
			Assert.AreNotEqual(25, newResizer.Height);
			Assert.AreNotEqual(ImageResampler.FilterType.Box, newResizer.FilterType);

			resizer.SaveDefaults();

			newResizer = new ImageSizeController4Test(new ImageSizeManipulator(image));

			Assert.IsFalse(newResizer.KeepAspect);
			Assert.AreEqual(30, newResizer.Width);
			Assert.AreEqual(25, newResizer.Height);
			Assert.AreEqual(ImageResampler.FilterType.Box, newResizer.FilterType);

			SAEWizardSettings.Default.Reset(); // Restore defaults
		}

		#region Test class

		class ImageSizeController4Test : ImageSizeController
		{
			public ImageSizeController4Test(ImageSizeManipulator manipulator) : base(manipulator) { }

			protected override void CallManipulationsCore()
			{
				CallManipulationsCoreFired = true;
			}

			public bool CallManipulationsCoreFired { get; set; }

			protected override void UpdateValuesFromManipulatedImage()
			{
				// Do nothing
			}
		}

		#endregion
	}
}

