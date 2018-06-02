using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ravlyk.Drawing.ImageProcessor.Utilities.Test
{
	[TestFixture]
	public class PaletteOrderExtensionsTest
	{
		[Test]
		public void TestOrderByDarknes()
		{
			Palette palette =
				new Palette
				{
					new Color(10, 0, 0),
					new Color(0, 20, 0),
					new Color(0, 0, 5),
					new Color(4, 4, 4),
					new Color(0),
				};

			AssertPaletteOrder(palette.OrderByDarknes(), new Color(0), new Color(0, 0, 5), new Color(10, 0, 0), new Color(4, 4, 4), new Color(0, 20, 0));
			AssertPaletteOrder(palette.OrderByDarknes(true), new Color(0, 20, 0), new Color(4, 4, 4), new Color(10, 0, 0), new Color(0, 0, 5), new Color(0));
		}

		[Test]
		public void TestOrderByCount()
		{
			Palette palette =
				new Palette
				{
					new Color(1),
					new Color(2),
					new Color(3),
				};
			palette[1].UsageOccurrences.AddRange(new[] { 1, 2, 3 });
			palette[3].UsageOccurrences.AddRange(new[] { 1 });

			AssertPaletteOrder(palette.OrderByCount(), new Color(2), new Color(3), new Color(1));
			AssertPaletteOrder(palette.OrderByCount(true), new Color(1), new Color(3), new Color(2));
		}

		[Test]
		public void TestOrderByDiff()
		{
			Palette palette =
				new Palette
				{
					new Color(70, 120, 170),
					new Color(75, 0, 0),
					new Color(0, 126, 0),
					new Color(0, 0, 177),
					new Color(0),
				};

			AssertPaletteOrder(palette.OrderByDiff(new Color(75, 125, 175), false), new Color(70, 120, 170), new Color(0, 0, 177), new Color(0, 126, 0), new Color(75, 0, 0), new Color(0));
			AssertPaletteOrder(palette.OrderByDiff(new Color(75, 125, 175), false, true), new Color(0), new Color(75, 0, 0), new Color(0, 126, 0), new Color(0, 0, 177), new Color(70, 120, 170));
			AssertPaletteOrder(palette.OrderByDiff(new Color(75, 125, 175), true), new Color(70, 120, 170), new Color(0, 126, 0), new Color(0, 0, 177), new Color(75, 0, 0), new Color(0));
			AssertPaletteOrder(palette.OrderByDiff(new Color(75, 125, 175), true, true), new Color(0), new Color(75, 0, 0), new Color(0, 0, 177), new Color(0, 126, 0), new Color(70, 120, 170));
		}

		[Test]
		public void TestOrderByGreen()
		{
			Palette palette =
				new Palette
				{
					new Color(0, 1, 0),
					new Color(0, 5, 0),
					new Color(0, 126, 0),
					new Color(0, 2, 0),
					new Color(0),
				};

			AssertPaletteOrder(palette.OrderByGreen(), new Color(0), new Color(0, 1, 0), new Color(0, 2, 0), new Color(0, 5, 0), new Color(0, 126, 0));
		}

		#region Implementation

		protected void AssertPaletteOrder(IEnumerable<Color> palette, params Color[] colorsInExpectedOrder)
		{
			int index = 0;
			foreach (Color color in palette)
			{
				Assert.AreEqual(colorsInExpectedOrder[index++], color);
			}
		}

		#endregion
	}
}

