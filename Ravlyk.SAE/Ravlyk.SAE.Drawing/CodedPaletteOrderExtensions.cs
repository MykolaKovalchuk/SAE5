using System;
using System.Collections.Generic;
using System.Linq;

namespace Ravlyk.SAE.Drawing
{
	/// <summary>
	/// Extensions for <see cref="CodedPalette"/> colors ordering.
	/// </summary>
	public static class CodedPaletteOrderExtensions
	{
		/// <summary>
		/// Returns array of ordered by code colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name="desc">Order in descending.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<CodedColor> OrderByCode(this CodedPalette palette, bool desc = false)
		{
			return desc
				? ((IEnumerable<CodedColor>)palette).OrderByDescending(c => c.ColorCode, new ColorCodeComparer())
				: ((IEnumerable<CodedColor>)palette).OrderBy(c => c.ColorCode, new ColorCodeComparer());
		}

		/// <summary>
		/// Returns array of ordered by symbol colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name="desc">Order in descending.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<CodedColor> OrderBySymbol(this CodedPalette palette, bool desc = false)
		{
			return desc ? ((IEnumerable<CodedColor>)palette).OrderByDescending(c => c.SymbolChar) : ((IEnumerable<CodedColor>)palette).OrderBy(c => c.SymbolChar);
		}

		class ColorCodeComparer : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				int a, b;
				if (int.TryParse(x, out a) && int.TryParse(y, out b))
				{
					return a.CompareTo(b);
				}
				else
				{
					return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
				}
			}
		}
	}
}
