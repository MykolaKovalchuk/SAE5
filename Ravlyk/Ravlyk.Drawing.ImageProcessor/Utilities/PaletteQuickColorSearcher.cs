using System;
using System.Diagnostics;
using System.Linq;

namespace Ravlyk.Drawing.ImageProcessor.Utilities
{
	/// <summary>
	/// Provides functionality for quick color search in a palette.
	/// </summary>
	public class PaletteQuickColorSearcher
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PaletteQuickColorSearcher"/> class. 
		/// </summary>
		/// <param name="palette">Palette with colors to search.</param>
		public PaletteQuickColorSearcher(Palette palette)
		{
			InitializeIndexArrays(palette);
		}

		void InitializeIndexArrays(Palette palette)
		{
			orderedColorsArray = palette.OrderByGreen().ToArray();
			int previousGreenValue = 0;
			int startPos = 0;

			for (int i = 0; i < orderedColorsArray.Length; i++)
			{
				Color color = orderedColorsArray[i];
				if (color.G != previousGreenValue)
				{
					greenIndexes[previousGreenValue] = (startPos + i) / 2;
					for (int j = previousGreenValue + 1; j < color.G; j++)
					{
						greenIndexes[j] = i;
					}
					previousGreenValue = color.G;
					startPos = i;
				}
			}

			int lastColroIndex = palette.Count - 1;
			greenIndexes[previousGreenValue] = (startPos + lastColroIndex) / 2;
			for (int j = previousGreenValue + 1; j < greenIndexes.Length; j++)
			{
				greenIndexes[j] = lastColroIndex;
			}
		}

		#endregion

		#region Internals

		const int GreenIndexesCount = byte.MaxValue - byte.MinValue + 1;

		Color[] orderedColorsArray;
		readonly int[] greenIndexes = new int[GreenIndexesCount];

		#endregion

		#region Search Substitute

		/// <summary>
		/// Searches substitute color, one from palette, that is nearest to specified color.
		/// </summary>
		/// <param name="color">Color which is needed to be substitute with other color in palette.</param>
		/// <param name="unique">Specifies if found substitute color shoud not be used for other substitutes.</param>
		/// <returns>The <see cref="Color"/> object, representing the best substitute color.</returns>
		public Color SearchSubstitute(Color color, bool unique = false)
		{
			return SearchSubstitute(color.R, color.G, color.B, unique);
		}

		/// <summary>
		/// Searches substitute color, one from palette, that is nearest to specified color.
		/// </summary>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		/// <param name="unique">Specifies if found substitute color shoud not be used for other substitutes.</param>
		/// <returns>The <see cref="Color"/> object, representing the best substitute color.</returns>
		public Color SearchSubstitute(byte r, byte g, byte b, bool unique = false)
		{
			Color bestColor = null;
			int bestDist = int.MaxValue;
			int bestIndex = -1;

			int i = greenIndexes[g];
			int j = i - 1;

			int colorArrayLenght = orderedColorsArray.Length;
			while (i < colorArrayLenght || j >= 0)
			{
				if (i < colorArrayLenght)
				{
					Color nearColor = orderedColorsArray[i];
					if (nearColor != null)
					{
						int dist = TestColorSubstitute(nearColor.R, nearColor.G, nearColor.B, r, g, b, bestDist);
						if (dist < bestDist)
						{
							bestColor = nearColor;
							bestDist = dist;
							bestIndex = i;
						}
						else if (dist > bestDist)
						{
							i = colorArrayLenght;
						}
					}

					i++;
				}

				if (j >= 0)
				{
					Color nearColor = orderedColorsArray[j];
					if (nearColor != null)
					{
						int dist = TestColorSubstitute(nearColor.R, nearColor.G, nearColor.B, r, g, b, bestDist);
						if (dist < bestDist)
						{
							bestColor = nearColor;
							bestDist = dist;
							bestIndex = j;
						}
						else if (dist > bestDist)
						{
							j = -1;
						}
					}

					j--;
				}
			}

			if (unique && bestIndex >= 0)
			{
				orderedColorsArray[bestIndex] = null;
			}

			return bestColor;
		}

		/// <summary>
		/// Delegate for testing substitute color on difference to etalone one, sepcified by (r, g, b) components.
		/// </summary>
		/// <param name="pR">Red component of color to test.</param>
		/// <param name="pG">Green component of color to test.</param>
		/// <param name="pB">Blue component of color to test.</param>
		/// <param name="oR">Red component of color to be substituted.</param>
		/// <param name="oG">Green component of color to be substituted.</param>
		/// <param name="oB">Blue component of color to be substituted.</param>
		/// <param name="bestDist">Current best distance.</param>
		/// <returns>Returns:
		/// - int.MaxValue, if Green distance is too big;
		/// - bestDist, if full distance is too big;
		/// - real distance, if new distance is less then bestDist.
		/// </returns>
		static int TestColorSubstitute(byte pR, byte pG, byte pB, byte oR, byte oG, byte oB, int bestDist)
		{
			int dG = pG - oG;
			int dist = ColorDistance.CoeffGreen * dG * dG;

			if (dist > bestDist)
			{
				return int.MaxValue;
			}

			if (dist < bestDist)
			{
				int dR = pR - oR;
				dist += ColorDistance.CoeffRed * dR * dR;
				if (dist < bestDist)
				{
					int dB = pB - oB;
					dist += ColorDistance.CoeffBlue * dB * dB;
					if (dist < bestDist)
					{
						//if (ColorDistance.CoeffHue > 0 || ColorDistance.CoeffSaturation > 0 || ColorDistance.CoeffBrightness > 0)
						//{
						//    dist += (int)(ColorDistance.CoeffHue * (p.Hue - o.Hue) * (p.Hue - o.Hue) * o.Saturation / 255);
						//    if (dist < bestDist)
						//    {
						//        dist += ColorDistance.CoeffSaturation * (p.Saturation - o.Saturation) * (p.Saturation - o.Saturation) * (255 - o.Saturation) / 255;
						//        if (dist < bestDist)
						//        {
						//            dist += ColorDistance.CoeffBrightness * (p.Brightness - o.Brightness) * (p.Brightness - o.Brightness);
						//            if (dist < bestDist)
						//            {
						//                return dist;
						//            }
						//        }
						//    }
						//}
						//else
						{
							return dist;
						}
					}
				}
			}

			return bestDist;
		}

		#endregion
	}

	/// <summary>
	/// Extension mehtods for quick color search in palette.
	/// </summary>
	public static class PaletteQuickColorSearchExtensions
	{
		/// <summary>
		/// Replaces all colors in palette on its substitutes from destination palette.
		/// </summary>
		/// <param name="palette">Source palette with colors which should be substituted.</param>
		/// <param name="destPalette">Palette with destination colors.</param>
		public static void SubstColorsFromPalette(this Palette palette, Palette destPalette)
		{
			Debug.Assert(destPalette.Count >= palette.Count, "Too few colors in destination palette.");

			Color[] oldColors = palette.OrderByCount(true).ToArray();
			palette.Clear();

			PaletteQuickColorSearcher searcher = new PaletteQuickColorSearcher(destPalette);
			foreach (Color oldColor in oldColors)
			{
				Color substituteColor = searcher.SearchSubstitute(oldColor, true);
				Debug.Assert(substituteColor != null, "Substitute color was not found.");
				palette.Add(substituteColor);
			}
		}
	}
}
