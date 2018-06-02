using System;
using NUnit.Framework;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageCropControllerTest
	{
		[Test]
		public void TestDoesntCallManipulationsWithoutChanges()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var cropper = new ImageCropController4Test(new ImageCropManipulator(image));

			Assert.AreEqual(ImageCropper.CropKind.None, cropper.CropKind, "Prerequisits");
			Assert.AreEqual(new Rectangle(0, 0, 5, 5), cropper.CropRect, "Prerequisits");
			Assert.IsFalse(cropper.CallManipulationsCoreFired, "Prerequisits");

			cropper.CropKind = ImageCropper.CropKind.Rectangle;
			Assert.IsTrue(cropper.CallManipulationsCoreFired, "Should call manipulations for initial crop kind change.");

			cropper.CallManipulationsCoreFired = false;

			cropper.CropKind = ImageCropper.CropKind.Rectangle;
			Assert.IsFalse(cropper.CallManipulationsCoreFired, "Should not call manipulations because there are no changes.");

			cropper.CropKind = ImageCropper.CropKind.Arc;
			Assert.IsTrue(cropper.CallManipulationsCoreFired, "Should call manipulations after changed parameters.");

			cropper.CallManipulationsCoreFired = false;

			cropper.CropRect = new Rectangle(0, 0, 5, 5);
			Assert.IsFalse(cropper.CallManipulationsCoreFired, "Should not call manipulations because there are no changes.");

			cropper.CropRect = new Rectangle(1, 1, 3, 2);
			Assert.IsTrue(cropper.CallManipulationsCoreFired, "Should call manipulations after changed parameters.");
		}

		[Test]
		public void TestCallManipulations()
		{
			var srcImage = new CodedImage { Size = new Size(10, 10) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i;
				}
			}

			var cropper = new ImageCropController(new ImageCropManipulator(srcImage));
			cropper.CropKind = ImageCropper.CropKind.Rectangle;
			cropper.CropRect = new Rectangle(1, 2, 8, 6);
			var dstImage = cropper.Manipulator.ManipulatedImage;

			Assert.AreEqual(new Size(8, 6), dstImage.Size);
			using (dstImage.LockPixels(out pixels))
			{
				Assert.AreEqual(cropper.CropRect.Width * cropper.CropRect.Height, pixels.Length);

				var i = 0;
				for (int y = cropper.CropRect.Top; y < cropper.CropRect.Top + cropper.CropRect.Height; y++)
				{
					for (int x = cropper.CropRect.Left; x < cropper.CropRect.Left + cropper.CropRect.Width; x++)
					{
						Assert.AreEqual(y * srcImage.Size.Width + x, pixels[i], string.Format("Pixels in position {0} should be same.", i));
						i++;
					}
				}
			}

			cropper.CropKind = ImageCropper.CropKind.Arc;
			dstImage.CompletePalette();
			Assert.AreEqual(new Size(8, 6), dstImage.Size);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[0, 0]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[0, 5]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[7, 0]);
			Assert.AreEqual(new CodedColor(255, 255, 255), dstImage[7, 5]);
			Assert.AreNotEqual(new CodedColor(255, 255, 255), dstImage[4, 3], "Some point in the centre");
		}

		[Test]
		public void TestCropRectCorrectLimits()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var cropper = new ImageCropController4Test(new ImageCropManipulator(image));

			cropper.CropRect = new Rectangle(-2, -3, 3, 2);
			Assert.AreEqual(new Rectangle(0, 0, 3, 2), cropper.CropRect, "Should move to 0 from outside of left/top borders.");

			cropper.CropRect = new Rectangle(2, 3, 5, 5);
			Assert.AreEqual(new Rectangle(2, 3, 3, 2), cropper.CropRect, "Should trim width/height to source image borders.");
		}

		#region Test class

		class ImageCropController4Test : ImageCropController
		{
			public ImageCropController4Test(ImageCropManipulator manipulator) : base(manipulator) { }

			protected override void CallManipulationsCore()
			{
				CallManipulationsCoreFired = true;
			}

			public bool CallManipulationsCoreFired { get; set; }
		}

		#endregion
	}
}

