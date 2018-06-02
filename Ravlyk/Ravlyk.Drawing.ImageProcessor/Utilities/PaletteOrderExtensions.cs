using System;
using System.Collections.Generic;
using System.Linq;

namespace Ravlyk.Drawing.ImageProcessor.Utilities
{
	/// <summary>
	/// Extensions for <see cref="Palette"/> colors ordering.
	/// </summary>
	public static class PaletteOrderExtensions
	{
		/// <summary>
		/// Returns array of ordered by darkness colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name="desc">Order in descending.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<Color> OrderByDarknes(this Palette palette, bool desc = false)
		{
			return desc ? palette.OrderBy(c => -c.Darkness) : palette.OrderBy(c => c.Darkness);
		}

		/// <summary>
		/// Returns array of ordered by count colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name="desc">Order in descending.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<Color> OrderByCount(this Palette palette, bool desc = false)
		{
			return desc ? palette.OrderBy(c => -c.OccurrencesCount) : palette.OrderBy(c => c.OccurrencesCount);
		}

		/// <summary>
		/// Returns array of ordered by difference colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name="color">Etalone color with which a difference is calculated.</param>
		/// <param name="useVisualDistance">Specifies if it is needed to use visual distance for colors difference.</param>
		/// <param name="desc">Order in descending.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<Color> OrderByDiff(this Palette palette, Color color, bool useVisualDistance, bool desc = false)
		{
			return palette.OrderBy(c => (desc ? -1 : 1) * (useVisualDistance ? c.GetVisualDistance(color) : c.GetSquareDistance(color)));
		}

		/// <summary>
		/// Returns array of ordered by green colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <returns>Ordered enumeration of colors.</returns>
		public static IEnumerable<Color> OrderByGreen(this Palette palette)
		{
			return palette.OrderBy(c => c.G);
		}

		/// <summary>
		/// Returns array of ordered randomly colors.
		/// </summary>
		/// <param name="palette">Palette with colors to order.</param>
		/// <param name = "rng">Random number generator.</param>
		/// <returns>Randomly ordered enumeration of colors.</returns>
		public static IEnumerable<Color> OrderByRandom(this Palette palette, Random rng)
		{
			var elements = palette.ToArray();

			// Note i > 0 to avoid final pointless iteration
			for (int i = elements.Length - 1; i > 0; i--)
			{
				// Swap element "i" with a random earlier element it (or itself)
				int swapIndex = rng.Next(i + 1);
				yield return elements[swapIndex];
				elements[swapIndex] = elements[i];
				// We don't actually perform the swap, we can forget about the swapped element because we already returned it.
			}

			// There is one item remaining that was not returned - we return it now
			yield return elements[0];
		}
	}
}
