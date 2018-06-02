using System;
using System.Linq;
using NUnit.Framework;

namespace Ravlyk.Drawing.ImageProcessor.Utilities.Test
{
	[TestFixture]
	public class PaletteQuickColorSearcherTest
	{
		[Test]
		public void TestSearchSubstiture()
		{
			var palette = new Palette
				{
					new Color(0),
					new Color(250, 250, 250),
					new Color(100, 0, 0),
					new Color(0, 100, 0),
					new Color(0, 0, 100),
					new Color(0, 0, 101)
				};

			var quickColorSearcher = new PaletteQuickColorSearcher(palette);

			using (ColorDistance.UseCoeffs(1, 1, 1))
			{
				Assert.AreEqual(new Color(0), quickColorSearcher.SearchSubstitute(new Color(0)));
				Assert.AreEqual(new Color(250, 250, 250), quickColorSearcher.SearchSubstitute(new Color(254, 254, 254)));
				Assert.AreEqual(new Color(0, 0, 100), quickColorSearcher.SearchSubstitute(new Color(98, 98, 99)));
				Assert.AreEqual(new Color(0, 100, 0), quickColorSearcher.SearchSubstitute(new Color(98, 99, 98)));
				Assert.AreEqual(new Color(100, 0, 0), quickColorSearcher.SearchSubstitute(new Color(99, 98, 98)));

				Assert.AreEqual(new Color(0, 0, 100), quickColorSearcher.SearchSubstitute(new Color(0, 0, 99), true));
				Assert.AreEqual(new Color(0, 0, 101), quickColorSearcher.SearchSubstitute(new Color(0, 0, 99), true));
			}
		}

		[Test]
		public void TestSubstColorsFromPalette()
		{
			var destPalette = new Palette
				{
					new Color(0),
					new Color(250, 250, 250),
					new Color(100, 100, 100),
					new Color(100, 0, 0),
					new Color(0, 100, 0),
					new Color(0, 0, 100),
					new Color(0, 0, 101)
				};

			var palette = new Palette
				{
					new Color(0, 0, 99),
					new Color(0, 0, 100),
					new Color(1, 2, 3),
					new Color(100, 10, 10),
				};

			palette.SubstColorsFromPalette(destPalette);

			Assert.AreEqual(4, palette.Count);
			Assert.IsTrue(palette.Contains(new Color(0, 0, 100)));
			Assert.IsTrue(palette.Contains(new Color(0, 0, 101)));
			Assert.IsTrue(palette.Contains(new Color(0)));
			Assert.IsTrue(palette.Contains(new Color(100, 0, 0)));
		}
	}
}

