using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageColorsManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestOperationsCallsOnImageChanged()
		{
			var image = new CodedImage { Size = new Size(30, 20) };
			var colorer = new ImageColorsManipulator(image);
			var childManipulator = new ImageManipulatorTest.DummyManipulator(colorer);

			AssertActionCallsOnImageChanged(childManipulator, () => colorer.QuantizeColors(3));
		}

		[Test]
		public void TestQuantizeColors()
		{
			var image = new CodedImage { Size = new Size(30, 20), Palette = new CodedPalette() };
			image.CompletePalette();
			for (int x = 0; x < 30; x++)
			{
				for (int y = 0; y < 20; y++)
				{
					image[x, y] = new CodedColor((byte)(x * 8), (byte)(y * 12), (byte)((x + y) * 5));
				}
			}

			var destPalette =
				new CodedPalette
				{
					new CodedColor(0, 0, 0) { ColorCode = "100" },
					new CodedColor(255, 255, 255) { ColorCode = "200" },
					new CodedColor(127, 127, 127) { ColorCode = "300" }
				};

			var colorer = new ImageColorsManipulator(image);
			Assert.AreEqual(600, colorer.ManipulatedImage.Palette.Count, "Precondition");

			colorer.QuantizeColors(3, destPalette);
			Assert.AreEqual(3, colorer.ManipulatedImage.Palette.Count);
			Assert.AreEqual("100", colorer.ManipulatedImage.Palette[0].ColorCode);
			Assert.AreEqual("200", colorer.ManipulatedImage.Palette[new CodedColor(255, 255, 255).GetHashCode()].ColorCode);
			Assert.AreEqual("300", colorer.ManipulatedImage.Palette[new CodedColor(127, 127, 127).GetHashCode()].ColorCode);
		}

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			var image = new CodedImage { Size = new Size(30, 20), Palette = new CodedPalette() };
			image.CompletePalette();
			for (int x = 0; x < 30; x++)
			{
				for (int y = 0; y < 20; y++)
				{
					image[x, y] = new CodedColor((byte)(x * 8), (byte)(y * 12), (byte)((x + y) * 5));
				}
			}

			var dummyManipulator = new ImageManipulatorTest.DummyManipulator(image);
			var colorer = new ImageColorsManipulator(dummyManipulator);
			var childManipulator = new ImageManipulatorTest.DummyManipulator(colorer);

			colorer.QuantizeColors(3);
			Assert.AreEqual(3, colorer.ManipulatedImage.Palette.Count);

			colorer.ManipulatedImage.Palette.Add(new CodedColor(254, 254, 254));
			childManipulator.RestoreManipulationsCoreFired = false;

			Assert.AreEqual(4, colorer.ManipulatedImage.Palette.Count, "Precondition");
			Assert.IsFalse(childManipulator.RestoreManipulationsCoreFired);

			dummyManipulator.CallOnImageChanged();

			Assert.AreEqual(3, colorer.ManipulatedImage.Palette.Count);
			Assert.IsFalse(childManipulator.RestoreManipulationsCoreFired);
			Assert.NotNull(childManipulator.ManipulatedImage);
			Assert.IsTrue(childManipulator.RestoreManipulationsCoreFired);
		}
	}
}

