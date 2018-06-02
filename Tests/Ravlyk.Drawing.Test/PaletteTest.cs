using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.Test
{
	[TestFixture]
	public class PaletteTest
	{
		[Test]
		public void TestAddAndRemove()
		{
			Palette palette = new Palette();
			Assert.AreEqual(0, palette.Count);

			palette.Add(new Color(1));
			Assert.AreEqual(1, palette.Count);

			palette.Add(new Color(255));
			Assert.AreEqual(2, palette.Count);

			palette.Add(new Color(1));
			Assert.AreEqual(2, palette.Count);

			AssertColorInPalette(palette, 1, 1);
			AssertColorInPalette(palette, 255, 255);

			palette.Remove(100);
			Assert.AreEqual(2, palette.Count, "No error on removing non-existing color");

			palette.Remove(1);
			Assert.AreEqual(1, palette.Count);

			AssertColorInPalette(palette, 255, 255);

			palette.Clear();
			Assert.AreEqual(0, palette.Count);
		}

		[Test]
		public void TestAddCreatesNewInstance()
		{
			Color color = new Color(1, 2, 3, 4);
			Palette palette = new Palette();
			palette.Add(color);
			Color paletteColor = palette[color.GetHashCode()];

			Assert.IsNotNull(paletteColor);
			Assert.AreEqual(color, paletteColor);
			Assert.AreNotSame(color, paletteColor);
		}

		[Test]
		public void TestAddRemoveEvents()
		{
			int addedCount = 0;
			int removedCount = 0;
			Color lastAddedRemovedColor = null;

			var palette = new Palette();
			palette.ColorAdded +=
				(sender, e) =>
				{
					addedCount++;
					lastAddedRemovedColor = e.Color;
					Assert.AreSame(palette, sender);
				};
			palette.ColorRemoved +=
				(sender, e) =>
				{
					removedCount++;
					lastAddedRemovedColor = e.Color;
					Assert.AreSame(palette, sender);
				};

			Action<int, int, Color> doAsserts =
				(expectAdded, expectRemoved, expectColor) =>
				{
					Assert.AreEqual(expectAdded, addedCount);
					Assert.AreEqual(expectRemoved, removedCount);
					Assert.AreEqual(expectColor, lastAddedRemovedColor);
				};

			palette.Add(new Color(1));
			doAsserts(1, 0, new Color(1));

			palette.Add(new Color(2));
			doAsserts(2, 0, new Color(2));

			palette.Add(new Color(1));
			doAsserts(2, 0, new Color(2));

			palette.Remove(1);
			doAsserts(2, 1, new Color(1));

			palette.Remove(3);
			doAsserts(2, 1, new Color(1));
		}

		[Test]
		public void TestIndexer()
		{
			var palette = new Palette();
			palette.Add(new Color(1));
			palette.Add(new Color(2));

			Assert.AreEqual(new Color(1), palette[1]);
			Assert.AreEqual(new Color(2), palette[2]);
			Assert.IsNull(palette[3]);
		}

		[Test]
		public void TestColorOccurrences()
		{
			Palette palette = new Palette();
			palette.AddColorOccurrence(1, 0);
			palette.AddColorOccurrence(2, 1);
			palette.AddColorOccurrence(1, 2);
			palette.AddColorOccurrence(3, 3);
			palette.AddColorOccurrence(new Color(1), 4);
			palette.AddColorOccurrence(2, 5);
			palette.AddColorOccurrence(4, 6);

			Assert.AreEqual(4, palette.Count);
			AssertColorInPalette(palette, 1, 1, 0, 2, 4);
			AssertColorInPalette(palette, 2, 2, 1, 5);
			AssertColorInPalette(palette, 3, 3, 3);
			AssertColorInPalette(palette, 4, 4, 6);

			palette.RemoveColorOccurrence(1, 2);
			using (palette.SuppressRemoveColorsWithoutOccurrences())
			{
				palette.RemoveColorOccurrence(3, 3);
			}
			palette.RemoveColorOccurrence(4, 6);

			Assert.AreEqual(3, palette.Count);
			AssertColorInPalette(palette, 1, 1, 0, 4);
			AssertColorInPalette(palette, 2, 2, 1, 5);
			AssertColorInPalette(palette, 3, 3);
		}

		[Test]
		public void TestCompletePaletteFromImage()
		{
			IndexedImage image = new IndexedImage { Size = new Size(2, 2) };
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				pixels[0] = 0;
				pixels[1] = 255;
				pixels[2] = 1;
				pixels[3] = 255;
			}

			Palette palette = new Palette();
			Assert.AreEqual(0, palette.Count);
			palette.CompletePaletteFromImage(image);

			Assert.AreEqual(3, palette.Count);
			AssertColorInPalette(palette, 0, 0, 0);
			AssertColorInPalette(palette, 1, 1, 2);
			AssertColorInPalette(palette, 255, 255, 1, 3);

			using (image.LockPixels(out pixels))
			{
				pixels[0] = 100;
				pixels[2] = 100;
			}
			palette.CompletePaletteFromImage(image);

			Assert.AreEqual(2, palette.Count);
			AssertColorInPalette(palette, 100, 100, 0, 2);
			AssertColorInPalette(palette, 255, 255, 1, 3);
		}

		[Test]
		public void TestClone()
		{
			Palette palette1 = new Palette { new Color(1), new Color(2) };
			Palette palette2 = palette1.Clone();

			Assert.AreEqual(2, palette1.Count);
			Assert.AreEqual(2, palette2.Count);

			AssertColorInPalette(palette1, 1, 1);
			AssertColorInPalette(palette1, 2, 2);
			AssertColorInPalette(palette2, 1, 1);
			AssertColorInPalette(palette2, 2, 2);

			Assert.AreEqual(palette1[1], palette2[1]);
			Assert.AreNotSame(palette1[1], palette2[1], "There should be different instances");

			Assert.AreEqual(palette1[2], palette2[2]);
			Assert.AreNotSame(palette1[2], palette2[2], "There should be different instances");

			palette2.Remove(2);

			Assert.AreEqual(2, palette1.Count, "Changes in clonned palette should not affect original palette");
			Assert.AreEqual(1, palette2.Count);
		}

		#region Implementation

		internal static void AssertColorInPalette(Palette palette, int colorHash, int argb, params int[] occurrences)
		{
			Color color = palette[colorHash];

			Assert.IsNotNull(color, "Color {0} not found in palette", colorHash);
			Assert.AreEqual(argb, color.Argb);
			Assert.AreEqual(occurrences.Length, color.OccurrencesCount);

			foreach (var occurrence in occurrences)
			{
				Assert.IsTrue(color.UsageOccurrences.Contains(occurrence));
			}
		}

		#endregion
	}
}

