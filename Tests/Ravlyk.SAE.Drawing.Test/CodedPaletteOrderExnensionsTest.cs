using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.Test
{
	[TestFixture]
	public class CodedPaletteOrderExnensionsTest
	{
		[Test]
		public void TestOrderByCode()
		{
			var palette = new CodedPalette
				{
					new CodedColor(1) { ColorCode = "2" },
					new CodedColor(2) { ColorCode = "120" },
					new CodedColor(3) { ColorCode = "0999" },
					new CodedColor(4) { ColorCode = "A" },
					new CodedColor(5) { ColorCode = "998" },
				};

			AssertPaletteOrder(palette.OrderByCode(), new CodedColor(1), new CodedColor(2), new CodedColor(5), new CodedColor(3), new CodedColor(4));
			AssertPaletteOrder(palette.OrderByCode(true), new CodedColor(4), new CodedColor(3), new CodedColor(5), new CodedColor(2), new CodedColor(1));
		}

		[Test]
		public void TestOrderBySymbol()
		{
			var palette = new CodedPalette
				{
					new CodedColor(1) { SymbolChar = 'Z' },
					new CodedColor(2) { SymbolChar = 'B' },
					new CodedColor(3) { SymbolChar = 'W' },
					new CodedColor(4) { SymbolChar = 'C' },
					new CodedColor(5) { SymbolChar = 'A' },
				};

			AssertPaletteOrder(palette.OrderBySymbol(), new CodedColor(5), new CodedColor(2), new CodedColor(4), new CodedColor(3), new CodedColor(1));
			AssertPaletteOrder(palette.OrderBySymbol(true), new CodedColor(1), new CodedColor(3), new CodedColor(4), new CodedColor(2), new CodedColor(5));
		}

		#region Implementation

		protected void AssertPaletteOrder(IEnumerable<CodedColor> palette, params CodedColor[] colorsInExpectedOrder)
		{
			int index = 0;
			foreach (var color in palette)
			{
				Assert.AreEqual(colorsInExpectedOrder[index++], color);
			}
		}

		#endregion
	}
}

