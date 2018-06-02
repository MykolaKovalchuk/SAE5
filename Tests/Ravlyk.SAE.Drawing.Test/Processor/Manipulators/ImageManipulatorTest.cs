using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageManipulatorTest
	{
		[Test]
		public void TestConstructor()
		{
			var image = new CodedImage { Size = new Size(2, 2), Palette = new CodedPalette() };
			image.CompletePalette();
			image[0, 0] = new CodedColor(1) { ColorCode = "100" };
			image[0, 1] = new CodedColor(2) { ColorCode = "200" };
			image[1, 0] = new CodedColor(1) { ColorCode = "100" };
			image[1, 1] = new CodedColor(3) { ColorCode = "300" };

			var imageManipulator = new DummyManipulator(image);

			Assert.AreSame(image, imageManipulator.SourceImage);
			Assert.IsNotNull(imageManipulator.ManipulatedImage);
			Assert.AreNotSame(imageManipulator.SourceImage, imageManipulator.ManipulatedImage);

			Assert.AreEqual(new Size(2, 2), imageManipulator.ManipulatedImage.Size);
			Assert.AreEqual(new CodedColor(1), imageManipulator.ManipulatedImage[0, 0]);
			Assert.AreEqual(new CodedColor(2), imageManipulator.ManipulatedImage[0, 1]);
			Assert.AreEqual(new CodedColor(1), imageManipulator.ManipulatedImage[1, 0]);
			Assert.AreEqual(new CodedColor(3), imageManipulator.ManipulatedImage[1, 1]);

			Assert.AreEqual(3, imageManipulator.ManipulatedImage.Palette.Count);
			Assert.AreEqual("100", imageManipulator.ManipulatedImage.Palette[1].ColorCode);
			Assert.AreEqual("200", imageManipulator.ManipulatedImage.Palette[2].ColorCode);
			Assert.AreEqual("300", imageManipulator.ManipulatedImage.Palette[3].ColorCode);
		}

		[Test]
		public void TestParentManipulator()
		{
			var image = new CodedImage { Size = new Size(2, 2) };
			var parentManipulator = new DummyManipulator(image);
			var childManipulator = new DummyManipulator(parentManipulator);
			var grandChildManipulator = new DummyManipulator(childManipulator);

			Assert.IsNotNull(childManipulator.SourceImage);
			Assert.IsNotNull(childManipulator.ManipulatedImage);
			Assert.IsNotNull(grandChildManipulator.ManipulatedImage);
			Assert.AreNotSame(childManipulator.SourceImage, childManipulator.ManipulatedImage);
			Assert.AreSame(parentManipulator.ManipulatedImage, childManipulator.SourceImage);
			Assert.AreSame(childManipulator.ManipulatedImage, grandChildManipulator.SourceImage);

			childManipulator.RestoreManipulationsCoreFired = false;
			grandChildManipulator.RestoreManipulationsCoreFired = false;

			parentManipulator.CallOnImageChanged();

			Assert.IsTrue(childManipulator.RestoreManipulationsCoreFired);
			Assert.IsFalse(childManipulator.ResetFired);
			Assert.IsTrue(grandChildManipulator.RestoreManipulationsCoreFired);
			Assert.IsFalse(grandChildManipulator.ResetFired);
		}

		[Test]
		public void TestReset()
		{
			var image = new CodedImage { Size = new Size(2, 2), Palette = new CodedPalette() };
			image.CompletePalette();
			image[0, 0] = new CodedColor(1) { ColorCode = "100" };
			image[0, 1] = new CodedColor(1) { ColorCode = "100" };
			image[1, 0] = new CodedColor(1) { ColorCode = "100" };
			image[1, 1] = new CodedColor(1) { ColorCode = "100" };

			var imageManipulator = new DummyManipulator(image);

			Assert.AreEqual(new Size(2, 2), imageManipulator.ManipulatedImage.Size);
			Assert.AreEqual(new CodedColor(1), imageManipulator.ManipulatedImage[0, 0]);
			Assert.AreEqual("100", imageManipulator.ManipulatedImage[0, 0].ColorCode);

			imageManipulator.ManipulatedImage.Size = new Size(5, 5);
			imageManipulator.ManipulatedImage[0, 0] = new CodedColor(0) { ColorCode = "200" };

			Assert.AreEqual(new Size(5, 5), imageManipulator.ManipulatedImage.Size);
			Assert.AreEqual(new CodedColor(0), imageManipulator.ManipulatedImage[0, 0]);
			Assert.AreEqual("200", imageManipulator.ManipulatedImage[0, 0].ColorCode);

			imageManipulator.RestoreManipulationsCoreFired = false;

			var childManipulator = new DummyManipulator(imageManipulator);
			Assert.NotNull(childManipulator.ManipulatedImage);

			imageManipulator.Reset();

			Assert.AreEqual(new Size(2, 2), imageManipulator.ManipulatedImage.Size);
			Assert.AreEqual(new CodedColor(1), imageManipulator.ManipulatedImage[0, 0]);
			Assert.AreEqual("100", imageManipulator.ManipulatedImage[0, 0].ColorCode);

			Assert.IsTrue(imageManipulator.ResetFired);
			Assert.IsFalse(imageManipulator.RestoreManipulationsCoreFired);
			Assert.IsFalse(childManipulator.ResetFired);
			Assert.IsTrue(childManipulator.RestoreManipulationsCoreFired);
		}

		#region Test class

		public class DummyManipulator : ImageManipulator
		{
			public DummyManipulator(CodedImage sourceImage) : base(sourceImage) { }
			public DummyManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

			protected override void ResetCore()
			{
				base.ResetCore();
				ResetFired = true;
			}
			public bool ResetFired { get; set; }

			protected override void RestoreManipulationsCore()
			{
				base.RestoreManipulationsCore();
				RestoreManipulationsCoreFired = true;
			}

			public bool RestoreManipulationsCoreFired { get; set; }

			public void CallOnImageChanged()
			{
				OnImageChanged();
			}
		}

		#endregion
	}
}

