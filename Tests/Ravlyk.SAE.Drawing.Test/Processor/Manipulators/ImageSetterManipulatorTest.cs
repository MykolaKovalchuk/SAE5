using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageSetterManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestOperationsCallsOnImageChanged()
		{
			var imageSetter = new ImageSetterManipulator();
			var childManipulator = new ImageManipulatorTest.DummyManipulator(imageSetter);

			var newImage = new CodedImage { Size = new Size(5, 5) };

			AssertActionCallsOnImageChanged(childManipulator, () => imageSetter.SetNewImage(newImage));
		}

		[Test]
		public void TestDefaults()
		{
			var imageSetter = new ImageSetterManipulator();

			Assert.NotNull(imageSetter.SourceImage);
			Assert.NotNull(imageSetter.ManipulatedImage);
			Assert.AreSame(imageSetter.SourceImage, imageSetter.ManipulatedImage, "Manipulated and source images should be same instance.");
			Assert.AreEqual(1, imageSetter.ManipulatedImage.Size.Width, "Default image should have width 1.");
			Assert.AreEqual(1, imageSetter.ManipulatedImage.Size.Height, "Default image should have height 1.");
			Assert.AreEqual(new CodedColor(255, 255, 255), imageSetter.ManipulatedImage[0, 0], "Default image should have white pixel.");
		}

		[Test]
		public void TestSetNewImage()
		{
			var imageSetter = new ImageSetterManipulator();
			var originalImage = imageSetter.ManipulatedImage;

			var newImage = new CodedImage { Size = new Size(5, 5) };
			int[] newPixels, manipulatedPixels;
			using (newImage.LockPixels(out newPixels))
			{
				for (int i = 0; i < newPixels.Length; i++)
				{
					newPixels[i] = i;
				}
			}

			imageSetter.SetNewImage(newImage);

			Assert.AreSame(originalImage, imageSetter.SourceImage, "Source image still should be same original object.");
			Assert.AreSame(originalImage, imageSetter.ManipulatedImage, "Manipulated image still should be same original object.");
			Assert.AreEqual(newImage.Size, imageSetter.ManipulatedImage.Size);

			using (newImage.LockPixels(out newPixels))
			using (imageSetter.ManipulatedImage.LockPixels(out manipulatedPixels))
			{
				Assert.AreEqual(newPixels.Length, manipulatedPixels.Length);
				for (int i = 0; i < newPixels.Length; i++)
				{
					Assert.AreEqual(newPixels[i], manipulatedPixels[i], string.Format("Pixels in position {0} should be same", i));
				}
			}
		}

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			var imageSetter = new ImageSetterManipulator();
			var originalImage = imageSetter.ManipulatedImage;

			var newImage = new CodedImage { Size = new Size(3, 2) };
			imageSetter.SetNewImage(newImage);
			Assert.AreSame(originalImage, imageSetter.ManipulatedImage, "Precondition: manipulated image should be same original object.");

			imageSetter.RestoreManipulations();
			Assert.AreSame(originalImage, imageSetter.ManipulatedImage, "Manipulated image still should be same original object.");

			imageSetter.Reset();
			Assert.AreSame(originalImage, imageSetter.ManipulatedImage, "Manipulated image still should be same original object.");
		}
	}
}

