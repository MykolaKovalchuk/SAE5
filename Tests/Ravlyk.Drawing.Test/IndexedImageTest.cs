using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.Test
{
	[TestFixture]
	public class IndexedImageTest
	{
		[Test]
		public void TestSize()
		{
			IndexedImage image = new IndexedImage();
			Assert.AreEqual(new Size(0, 0), image.Size);
			Assert.AreEqual(0, image.Pixels.Length);

			image.Size = new Size(3, 2);
			Assert.AreEqual(new Size(3, 2), image.Size);
			Assert.AreEqual(6, image.Pixels.Length);
		}

		[Test]
		public void TestPixels()
		{
			IndexedImage image = new IndexedImage { Size = new Size(3, 2) };
			image.Pixels[0] = 1;
			image.Pixels[1] = 2;
			image.Pixels[2] = 3;
			image.Pixels[3] = 4;
			image.Pixels[4] = 5;
			image.Pixels[5] = 6;
			image.CompletePalette();

			Assert.AreEqual(1, image[0, 0].Argb);
			Assert.AreEqual(2, image[1, 0].Argb);
			Assert.AreEqual(3, image[2, 0].Argb);
			Assert.AreEqual(4, image[0, 1].Argb);
			Assert.AreEqual(5, image[1, 1].Argb);
			Assert.AreEqual(6, image[2, 1].Argb);

			image[0, 0] = new Color(100);
			image[1, 0] = new Color(200);
			image[2, 0] = new Color(400);
			image[0, 1] = new Color(800);
			image[1, 1] = new Color(1600);
			image[2, 1] = new Color(3200);

			Assert.AreEqual(100, image.Pixels[0]);
			Assert.AreEqual(200, image.Pixels[1]);
			Assert.AreEqual(400, image.Pixels[2]);
			Assert.AreEqual(800, image.Pixels[3]);
			Assert.AreEqual(1600, image.Pixels[4]);
			Assert.AreEqual(3200, image.Pixels[5]);
		}

		[Test]
		public void TestColorUsageOccurrences()
		{
			IndexedImage image = new IndexedImage { Size = new Size(3, 2) };
			image.Pixels[0] = 1;
			image.Pixels[1] = 2;
			image.Pixels[2] = 3;
			image.Pixels[3] = 1;
			image.Pixels[4] = 4;
			image.Pixels[5] = 2;
			image.CompletePalette();

			Assert.AreEqual(4, image.Palette.Count);
			PaletteTest.AssertColorInPalette(image.Palette, 1, 1, 0, 3);
			PaletteTest.AssertColorInPalette(image.Palette, 2, 2, 1, 5);
			PaletteTest.AssertColorInPalette(image.Palette, 3, 3, 2);
			PaletteTest.AssertColorInPalette(image.Palette, 4, 4, 4);

			image[1, 1] = new Color(1);

			Assert.AreEqual(3, image.Palette.Count);
			PaletteTest.AssertColorInPalette(image.Palette, 1, 1, 0, 3, 4);

			image[1, 1] = new Color(5);

			Assert.AreEqual(4, image.Palette.Count);
			PaletteTest.AssertColorInPalette(image.Palette, 1, 1, 0, 3);
			PaletteTest.AssertColorInPalette(image.Palette, 5, 5, 4);
		}

		[Test]
		public void TestLockPixels()
		{
			IndexedImage image = new IndexedImage { Size = new Size(3, 2) };

			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				Assert.IsNotNull(pixels);

				pixels[0] = 1;
				pixels[1] = 2;
				pixels[2] = 3;
				pixels[3] = 4;
				pixels[4] = 5;
				pixels[5] = 6;
			}

			Assert.AreEqual(0, image.Palette.Count);
			image.CompletePalette();
			Assert.AreEqual(6, image.Palette.Count);

			Assert.AreEqual(1, image[0, 0].Argb);
			Assert.AreEqual(2, image[1, 0].Argb);
			Assert.AreEqual(3, image[2, 0].Argb);
			Assert.AreEqual(4, image[0, 1].Argb);
			Assert.AreEqual(5, image[1, 1].Argb);
			Assert.AreEqual(6, image[2, 1].Argb);

			PaletteTest.AssertColorInPalette(image.Palette, 1, 1, 0);
			PaletteTest.AssertColorInPalette(image.Palette, 2, 2, 1);
			PaletteTest.AssertColorInPalette(image.Palette, 3, 3, 2);
			PaletteTest.AssertColorInPalette(image.Palette, 4, 4, 3);
			PaletteTest.AssertColorInPalette(image.Palette, 5, 5, 4);
			PaletteTest.AssertColorInPalette(image.Palette, 6, 6, 5);
		}

		[Test]
		public void TestClone()
		{
			IndexedImage image = new IndexedImage { Size = new Size(3, 2) };
			image.Pixels[0] = 1;
			image.Pixels[1] = 2;
			image.Pixels[2] = 3;
			image.Pixels[3] = 1;
			image.Pixels[4] = 4;
			image.Pixels[5] = 2;
			image.CompletePalette();

			IndexedImage clonedImage = image.Clone(true, true);

			Assert.AreEqual(image.Size, clonedImage.Size);

			Assert.AreEqual(image.Pixels.Length, clonedImage.Pixels.Length);
			Assert.AreNotSame(image.Pixels, clonedImage.Pixels);
			Assert.AreEqual(image.Pixels[0], clonedImage.Pixels[0]);
			Assert.AreEqual(image.Pixels[1], clonedImage.Pixels[1]);
			Assert.AreEqual(image.Pixels[2], clonedImage.Pixels[2]);
			Assert.AreEqual(image.Pixels[3], clonedImage.Pixels[3]);
			Assert.AreEqual(image.Pixels[4], clonedImage.Pixels[4]);
			Assert.AreEqual(image.Pixels[5], clonedImage.Pixels[5]);

			Assert.AreEqual(image.Palette.Count, clonedImage.Palette.Count);
			Assert.AreNotSame(image.Palette, clonedImage.Palette);
			Assert.AreEqual(image.Palette[1], clonedImage.Palette[1]);
			Assert.AreEqual(image.Palette[2], clonedImage.Palette[2]);
			Assert.AreEqual(image.Palette[3], clonedImage.Palette[3]);
			Assert.AreEqual(image.Palette[4], clonedImage.Palette[4]);

			clonedImage[0, 0] = new Color(100);

			Assert.AreEqual(100, clonedImage.Pixels[0]);
			Assert.AreEqual(1, image.Pixels[0], "Changes in cloned image should not change original");
			Assert.AreEqual(5, clonedImage.Palette.Count);
			Assert.AreEqual(4, image.Palette.Count, "Changes in cloned image should not change original");

			PaletteTest.AssertColorInPalette(clonedImage.Palette, 100, 100, 0);
			PaletteTest.AssertColorInPalette(clonedImage.Palette, 1, 1, 3);
			PaletteTest.AssertColorInPalette(image.Palette, 1, 1, 0, 3);
		}

		[Test]
		public void TestOnPixelChanged()
		{
			var image = new IndexedImage { Size = new Size(2, 3) };
			image.Palette = new Palette();
			image[1, 2] = new Color(1, 2, 3);

			bool eventFired = false;
			image.PixelChanged +=
				(sender, e) =>
				{
					eventFired = true;
					Assert.AreSame(image, sender);
					Assert.AreEqual(1, e.X);
					Assert.AreEqual(2, e.Y);
					Assert.AreEqual(new Color(1, 2, 3), e.OldColor);
					Assert.AreEqual(new Color(10, 20, 30), e.NewColor);
				};

			Assert.IsFalse(eventFired, "Precondition");
			image[1, 2] = new Color(10, 20, 30);
			Assert.IsTrue(eventFired);
		}
	}
}

