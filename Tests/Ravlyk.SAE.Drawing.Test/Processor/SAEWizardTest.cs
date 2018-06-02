using System;
using NUnit.Framework;

using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class SAEWizardTest
	{
		#region Controllers

		[Test]
		public void TestControllersLinks()
		{
			var wizard = new SAEWizardForTest();

			AssertControllersLink(wizard.ImageSetter, wizard.ImageRotator);
			AssertControllersLink(wizard.ImageRotator, wizard.ImageCropper);
			AssertControllersLink(wizard.ImageCropper, wizard.ImageResizer);
			AssertControllersLink(wizard.ImageResizer, wizard.ImageColorer);
		}

		void AssertControllersLink(ImageController controller1, ImageController controller2)
		{
			Assert.AreSame(controller1.Manipulator.ManipulatedImage, controller2.Manipulator.SourceImage);
		}

		#endregion

		#region Images
		
		[Test]
		public void TestImages()
		{
			var wizard = new SAEWizardForTest();

			Assert.AreSame(wizard.ImageSetter.Manipulator.SourceImage, wizard.SourceImage);
			Assert.AreSame(wizard.ImageCropper.Manipulator.ManipulatedImage, wizard.RotatedCroppedImage);
			Assert.AreSame(wizard.ImageColorer.Manipulator.ManipulatedImage, wizard.ColoredImage);
			Assert.AreSame(wizard.ImageSymboler.Manipulator.ManipulatedImage, wizard.FinalImage);
		}

		[Test]
		public void TestSourceImageFileName()
		{
			var wizard = new SAEWizardForTest();
			Assert.IsTrue(string.IsNullOrEmpty(wizard.SourceImageFileName), "Initial image is not loaded from file.");

			wizard.LoadSourceImageFromFile("file1");
			Assert.AreEqual("file1", wizard.SourceImageFileName);

			wizard.LoadSourceImageFromFile("file2");
			Assert.AreEqual("file2", wizard.SourceImageFileName);
		}

		[Test]
		public void TestLoadSourceImageFromFile()
		{
			var wizard = new SAEWizardForTest();

			wizard.TestSize = new Size(4, 3);
			wizard.TestColor = 1;
			wizard.LoadSourceImageFromFile("file1");
			AssertLoadedImage(wizard.SourceImage, new Size(4, 3), 1);

			wizard.TestSize = new Size(5, 6);
			wizard.TestColor = 100;
			wizard.LoadSourceImageFromFile("file1");
			AssertLoadedImage(wizard.SourceImage, new Size(5, 6), 100);
		}

		void AssertLoadedImage(IndexedImage image, Size expectedSize, int expectedColor)
		{
			Assert.AreEqual(expectedSize.Width, image.Size.Width);
			Assert.AreEqual(expectedSize.Height, image.Size.Height);

			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				foreach (var pixel in pixels)
				{
					Assert.AreEqual(expectedColor, pixel);
				}
			}
		}

		#endregion

		#region Defaults

		[Test]
		public void TestSaveDefaults()
		{
			var wizard = new SAEWizardForTest();

			wizard.ImageResizer.Width = 34;
			wizard.ImageColorer.MaxColorsCount = 15;

			var newWizard = new SAEWizardForTest();

			Assert.AreNotEqual(34, newWizard.ImageResizer.Width);
			Assert.AreNotEqual(15, newWizard.ImageColorer.MaxColorsCount);

			wizard.SaveDefaults();

			newWizard = new SAEWizardForTest();

			Assert.AreEqual(34, newWizard.ImageResizer.Width);
			Assert.AreEqual(15, newWizard.ImageColorer.MaxColorsCount);

			SAEWizardSettings.Default.Reset(); // Restore defaults
		}

		#endregion

		#region Test classes

		class SAEWizardForTest : SAEWizard
		{
			protected override CodedImage LoadImageCore(string fileName)
			{
				var size = TestSize.Width > 0 && TestSize.Height > 0 ? TestSize : new Size(1, 1);
				var image = new CodedImage { Size = size };

				int[] pixels;
				using (image.LockPixels(out pixels))
				{
					for (var i = 0; i < pixels.Length; i++)
					{
						pixels[i] = TestColor;
					}
				}

				return image;
			}

			public Size TestSize { get; set; }
			public int TestColor { get; set; }
	}

		#endregion
	}
}

