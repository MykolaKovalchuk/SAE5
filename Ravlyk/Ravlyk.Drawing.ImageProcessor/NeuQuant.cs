/* NeuQuant Neural-Net Quantization Algorithm
 * ------------------------------------------
 *
 * Copyright (c) 1994 Anthony Dekker
 *
 * NEUQUANT Neural-Net quantization algorithm by Anthony Dekker, 1994.
 * See "Kohonen neural networks for optimal colour quantization"
 * in "Network: Computation in Neural Systems" Vol. 5 (1994) pp 351-367.
 * for a discussion of the algorithm.
 *
 * Any party obtaining a copy of these files from the author, directly or
 * indirectly, is granted, free of charge, a full and unrestricted irrevocable,
 * world-wide, paid up, royalty-free, nonexclusive right and license to deal
 * in this software and documentation files (the "Software"), including without
 * limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons who receive
 * copies from any such party to do so, with the only requirement being
 * that this copyright notice remain intact.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Ravlyk.Drawing.ImageProcessor.Utilities;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Exception about too small image.
	/// </summary>
	public class ImageSizeTooSmallException : InvalidOperationException
	{
		/// <summary>
		/// Base constructor.
		/// </summary>
		/// <param name="message">Error message.</param>
		public ImageSizeTooSmallException(string message) : base(message) { }
	}

	/// <summary>
	/// Makes color quantization on <see cref="IndexedImage"/> object.
	/// </summary>
	public class NeuQuant
	{
		#region Constants and global variables

		public const int MaxDitherLevel = 10;

		const int Prime1 = 499;
		const int Prime2 = 491;
		const int Prime3 = 487;
		const int Prime4 = 503;
		const int MaxPrime = Prime4;

		const int CyclesCount = 100;

		const int BlackColorIdx = 0;
		const int WhiteColorIdx = 1;

		const int NetBiasShift = 4; /* Bias for colour values */

		const int RadiusBiasShift = 6;
		const int RadiusBias = 1 << RadiusBiasShift;
		const int RadiusDec = 30;

		const int IntBiasShift = 16; /* Bias for fractions */
		const int IntBias = 1 << IntBiasShift;
		const int GammaShift = 10; /* Gamma = 1024 */
		//const int Gamma = 1 << GammaShift; /* Unused constant */
		const int BetaShift = 10;
		const int Beta = IntBias >> BetaShift; /* Beta = 1 / 1024 */
		const int BetaGamma = IntBias << (GammaShift - BetaShift);

		const int AlphaBiasShift = 10;
		const int InitAlpha = 1 << AlphaBiasShift;

		const int RadBiasShift = 8;
		const int RadBias = 1 << RadBiasShift;
		const int AlphaRadBiasShift = AlphaBiasShift + RadBiasShift;
		const int AlphaRadBias = 1 << AlphaRadBiasShift;

		private int ColorsCount;
		private int Sample = 1;

		private int[,] network;
		private int[] freq;
		private int[] bias;
		private int[] radPower;

		private int initRadius;
		private int initBiasRadius;

		int coeffR = ColorDistance.CoeffRed;
		int coeffG = ColorDistance.CoeffGreen;
		int coeffB = ColorDistance.CoeffBlue;

		int SpecialColorsCount
		{
			get { return ensureBlackAndWhiteColors ? BlackAndWhiteColorsCount : 0; }
		}
		const int BlackAndWhiteColorsCount = 2;
		bool ensureBlackAndWhiteColors = false;

		#endregion

		/// <summary>
		/// Makes color quantization on <see cref="IndexedImage"/> object.
		/// </summary>
		/// <param name="sourceImage">The source <see cref="IndexedImage"/> object.</param>
		/// <param name="colorsCount">New count for color quantization.</param>
		/// <param name="destPalette">Palette with destination colors.</param>
		/// <param name="sample">Sample of pixels for clasterization (1 = all pixels, n = 1/n pixels).</param>
		/// <param name="ditherLevel">Specifies level of image dithering (0-10).</param>
		/// <param name="orderColorsByUsage">Enable additional refining step with ordering colors by usage count before substituting into destination palette.</param>
		/// <param name="destImage">Destionation <see cref="IndexedImage"/> object or null.</param>
		/// <param name="ensureBlackAndWhiteColors">Ensures that Black and White colors are included into <see cref="colorsCount"/> selected colors.</param>
		/// <param name="randomInitColorsPercent">Percent of colors pre-initialized with random value.</param>
		/// <returns>New or updated quantized <see cref="IndexedImage"/> object.</returns>
		public IndexedImage QuantizeColors(IndexedImage sourceImage, int colorsCount, Palette destPalette = null, int sample = 1, int ditherLevel = 0, bool orderColorsByUsage = true, IndexedImage destImage = null, bool ensureBlackAndWhiteColors = false, int randomInitColorsPercent = 0)
		{
			sourceImage.CompletePalette(); // For using in colors substituting

			this.ensureBlackAndWhiteColors = ensureBlackAndWhiteColors;
			var minColorsCount = Math.Min(colorsCount, destPalette?.Count ?? -1);
			if (sourceImage.Palette.Count <= minColorsCount)
			{
				var quickImage =
					(ditherLevel > 0)
						? DitherNewImageWithPalette(sourceImage, destPalette, ditherLevel, destImage)
						: CreateNewImageWithPalette(sourceImage, destPalette, destImage);
				quickImage.CompletePalette();
				if (quickImage.Palette.Count <= minColorsCount)
				{
					return quickImage;
				}
			}

			InitilizeGlobalVariables(colorsCount, sample);
			InitializeArrays(sourceImage.Palette, randomInitColorsPercent);

			var palette = GenerateOutputPalette(sourceImage);
			if (destPalette != null)
			{
				if (orderColorsByUsage)
				{
					UpdateUsagesCounts(palette, sourceImage);
				}
				palette.SubstColorsFromPalette(destPalette);
			}

			var resultImage =
				(ditherLevel > 0)
					? DitherNewImageWithPalette(sourceImage, palette, ditherLevel, destImage)
					: CreateNewImageWithPalette(sourceImage, palette, destImage);

			resultImage.CompletePalette();

			return resultImage;
		}

		#region Initialize global variables

		private void InitilizeGlobalVariables(int colorsCount, int sample)
		{
			ColorsCount = colorsCount;
			Sample = sample;

			initRadius = (ColorsCount + 7) / 8;
			initBiasRadius = initRadius * RadiusBias;

			coeffR = ColorDistance.CoeffRed;
			coeffG = ColorDistance.CoeffGreen;
			coeffB = ColorDistance.CoeffBlue;
		}

		private void InitializeArrays(Palette sourcePalette, int randomInitColorsPercent)
		{
			int initFrequence = IntBias / ColorsCount;

			network = new int[ColorsCount, 3];
			freq = new int[ColorsCount];
			bias = new int[ColorsCount];
			radPower = new int[initRadius];

			network[BlackColorIdx, ColorBytes.RedIdx] = 0;
			network[BlackColorIdx, ColorBytes.GreenIdx] = 0;
			network[BlackColorIdx, ColorBytes.BlueIdx] = 0;
			freq[BlackColorIdx] = initFrequence;
			bias[BlackColorIdx] = 0;

			int weight = 255 << NetBiasShift;

			network[WhiteColorIdx, ColorBytes.RedIdx] = weight;
			network[WhiteColorIdx, ColorBytes.GreenIdx] = weight;
			network[WhiteColorIdx, ColorBytes.BlueIdx] = weight;
			freq[WhiteColorIdx] = initFrequence;
			bias[WhiteColorIdx] = 0;

			int randomInitColorsCount = ColorsCount * randomInitColorsPercent / 100;
			if (randomInitColorsCount > 0)
			{
				var randomColors = sourcePalette.OrderByRandom(new Random())
				.Where(c =>
					{
						var rgb = c.Argb & 0x00ffffff;
						return rgb != 0 && rgb != 0x00ffffff; // Skip black and white
					})
					.Take(randomInitColorsCount).ToArray();
				randomInitColorsCount = randomColors.Length;
				for (int i = 0, index = BlackAndWhiteColorsCount; i < randomInitColorsCount; i++, index++)
				{
					var color = randomColors[i];
					network[index, ColorBytes.RedIdx] = color.R << NetBiasShift;
					network[index, ColorBytes.GreenIdx] = color.G << NetBiasShift;
					network[index, ColorBytes.BlueIdx] = color.B << NetBiasShift;
					freq[index] = initFrequence;
					bias[index] = 0;
				}
			}

			const int workRangeStart = BlackAndWhiteColorsCount / 2;
			var definedColorsCount = BlackAndWhiteColorsCount + randomInitColorsCount;
			var spreadColorsCount = ColorsCount - definedColorsCount;
			for (int i = 0, index = definedColorsCount; i < spreadColorsCount; i++, index++)
			{
				weight = ((i + workRangeStart) * 255 << NetBiasShift) / (spreadColorsCount + BlackAndWhiteColorsCount - 1);
				network[index, ColorBytes.RedIdx] = weight;
				network[index, ColorBytes.GreenIdx] = weight;
				network[index, ColorBytes.BlueIdx] = weight;
				freq[index] = initFrequence;
				bias[index] = 0;
			}
		}

		#endregion

		#region Network learning

		private Palette GenerateOutputPalette(IndexedImage sourceImage)
		{
			int[] pixels = sourceImage.Pixels;

			int alphaDec = 30 + ((Sample - 1) / 3);
			int lengthCount = pixels.Length;
			int samplePixels = lengthCount / Sample;
			int delta = samplePixels / CyclesCount;
			int alpha = InitAlpha;
			int biasRadius = initBiasRadius;

			int rad = biasRadius >> RadiusBiasShift;
			if (rad <= 1)
			{
				rad = 0;
			}

			int step;
			if (lengthCount < MaxPrime)
			{
				step = 1;
			}
			else if ((lengthCount % Prime1) != 0)
			{
				step = Prime1;
			}
			else if ((lengthCount % Prime2) != 0)
			{
				step = Prime2;
			}
			else if ((lengthCount % Prime3) != 0)
			{
				step = Prime3;
			}
			else
			{
				step = Prime4;
			}

			for (int i = 0, pos = 0; i < samplePixels; i++)
			{
				if ((i % delta) == 0)
				{
					if (i != 0)
					{
						alpha -= alpha / alphaDec;
						biasRadius -= biasRadius / RadiusDec;
					}

					rad = biasRadius >> RadiusBiasShift;
					if (rad <= 1)
					{
						rad = 0;
					}
					for (int radIndex = 0; radIndex < rad; radIndex++)
					{
						radPower[radIndex] = alpha * (((rad * rad - radIndex * radIndex) * RadBias) / (rad * rad));
					}
				}

				int p = pixels[pos];
				int red = (p & ColorBytes.RedMask) >> ColorBytes.RedBias;
				int green = (p & ColorBytes.GreenMask) >> ColorBytes.GreenBias;
				int blue = (p & ColorBytes.BlueMask) >> ColorBytes.BlueBias;

				int r = red << NetBiasShift;
				int g = green << NetBiasShift;
				int b = blue << NetBiasShift;

				int j = SpecialFind(r, g, b);
				j = (j < 0) ? ContestFind(r, g, b) : j;

				// Don't learn for specials
				if (j >= SpecialColorsCount)
				{
					AlterSingle(alpha, j, r, g, b);
					if (rad != 0)
					{
						AlterNeighbors(rad, j, r, g, b); // Alter neighbors
					}
				}

				pos += step;
				while (pos >= lengthCount)
				{
					pos -= lengthCount;
				}
			}

			Palette palette = sourceImage.Palette.Clone();
			palette.Clear();
			for (int j = 0; j < ColorsCount; j++)
			{
				int red = network[j, ColorBytes.RedIdx] >> NetBiasShift;
				if (red < 0)
				{
					red = 0;
				}
				if (red > 255)
				{
					red = 255;
				}

				int green = network[j, ColorBytes.GreenIdx] >> NetBiasShift;
				if (green < 0)
				{
					green = 0;
				}
				if (green > 255)
				{
					green = 255;
				}

				int blue = network[j, ColorBytes.BlueIdx] >> NetBiasShift;
				if (blue < 0)
				{
					blue = 0;
				}
				if (blue > 255)
				{
					blue = 255;
				}

				palette.Add(new Color((byte)red, (byte)green, (byte)blue));
			}

			return palette;
		}

		private int ContestFind(int r, int g, int b)
		{
			// Search for biased RGB values
			// finds closest neuron (min dist) and updates freq 
			// finds best neuron (min dist-bias) and returns position 
			// for frequently chosen neurons, freq[i] is high and bias[i] is negative 
			// bias[i] = Gamma * ((1 / ColorsCount) - freq[i]) 

			int bestd = int.MaxValue;
			int bestbiasd = bestd;
			int bestpos = -1;
			int bestbiaspos = bestpos;

			int startFrom = ColorsCount > SpecialColorsCount ? SpecialColorsCount : 0;

			for (int i = startFrom; i < ColorsCount; i++)
			{
				int dR = network[i, ColorBytes.RedIdx] - r;
				int dG = network[i, ColorBytes.GreenIdx] - g;
				int dB = network[i, ColorBytes.BlueIdx] - b;
				int dist = coeffR * dR * dR + coeffG * dG * dG + coeffB * dB * dB;

				if (dist < bestd)
				{
					bestd = dist;
					bestpos = i;
				}
				int biasdist = dist - (bias[i] >> (IntBiasShift - NetBiasShift));
				if (biasdist < bestbiasd)
				{
					bestbiasd = biasdist;
					bestbiaspos = i;
				}
				freq[i] -= freq[i] >> BetaShift;
				bias[i] += freq[i] << GammaShift;
			}

			freq[bestpos] += Beta;
			bias[bestpos] -= BetaGamma;

			return bestbiaspos;
		}

		private int SpecialFind(int r, int g, int b)
		{
			for (int i = 0; i < SpecialColorsCount; i++)
			{
				if ((network[i, ColorBytes.RedIdx] == r) && (network[i, ColorBytes.GreenIdx] == g) && (network[i, ColorBytes.BlueIdx] == b))
				{
					return i;
				}
			}
			return -1;
		}

		private void AlterSingle(int alpha, int i, int r, int g, int b)
		{
			// Move neuron i towards biased (r,g,b) by factor alpha
			network[i, ColorBytes.RedIdx] -= alpha * (network[i, ColorBytes.RedIdx] - r) / InitAlpha;
			network[i, ColorBytes.GreenIdx] -= alpha * (network[i, ColorBytes.GreenIdx] - g) / InitAlpha;
			network[i, ColorBytes.BlueIdx] -= alpha * (network[i, ColorBytes.BlueIdx] - b) / InitAlpha;
		}

		private void AlterNeighbors(int rad, int i, int r, int g, int b)
		{
			int lo = i - rad;
			if (lo < (SpecialColorsCount - 1)) lo = SpecialColorsCount - 1;
			int hi = i + rad;
			if (hi > ColorsCount) hi = ColorsCount;

			int j = i + 1;
			int k = i - 1;
			int m = 1;
			while ((j < hi) || (k > lo))
			{
				int a = radPower[m++];

				if (j < hi)
				{
					network[j, ColorBytes.RedIdx] -= a * (network[j, ColorBytes.RedIdx] - r) / AlphaRadBias;
					network[j, ColorBytes.GreenIdx] -= a * (network[j, ColorBytes.GreenIdx] - g) / AlphaRadBias;
					network[j, ColorBytes.BlueIdx] -= a * (network[j, ColorBytes.BlueIdx] - b) / AlphaRadBias;
					j++;
				}
				if (k > lo)
				{
					network[k, ColorBytes.RedIdx] -= a * (network[k, ColorBytes.RedIdx] - r) / AlphaRadBias;
					network[k, ColorBytes.GreenIdx] -= a * (network[k, ColorBytes.GreenIdx] - g) / AlphaRadBias;
					network[k, ColorBytes.BlueIdx] -= a * (network[k, ColorBytes.BlueIdx] - b) / AlphaRadBias;
					k--;
				}
			}
		}

		#endregion

		#region Colors substituting

		private void UpdateUsagesCounts(Palette palette, IndexedImage sourceImage)
		{
			foreach (Color color in palette)
			{
				color.ClearOccurrences();
			}

			var searcher = new PaletteQuickColorSearcher(palette);

			foreach (Color sourceColor in sourceImage.Palette)
			{
				Color substColor = searcher.SearchSubstitute(sourceColor);
				Debug.Assert(substColor != null, "Substitute color was not found.");
				substColor.OccurrencesCount += sourceColor.OccurrencesCount;
			}
		}

		private IndexedImage CreateNewImageWithPalette(IndexedImage sourceImage, Palette palette, IndexedImage destImage)
		{
			if (destImage == null)
			{
				destImage = sourceImage.Clone(false);
			}
			destImage.Size = sourceImage.Size;
			destImage.Palette = palette.Clone();

			var destPixels = destImage.Pixels;

			var searcher = new PaletteQuickColorSearcher(palette);

			Parallel.ForEach(sourceImage.Palette,
				sourceColor =>
				{
					Color substColor = searcher.SearchSubstitute(sourceColor);
					Debug.Assert(substColor != null, "Substitute color was not found.");
					foreach (int occurrence in sourceColor.UsageOccurrences)
					{
						destPixels[occurrence] = substColor.Argb;
					}
				});

			return destImage;
		}

		public IndexedImage DitherNewImageWithPalette(IndexedImage sourceImage, Palette palette, int ditherLevel, IndexedImage destImage = null)
		{
			Debug.Assert((ditherLevel >= 1) && (ditherLevel <= MaxDitherLevel), "ditherLevel should is out of range");

			if (destImage == null)
			{
				destImage = sourceImage.Clone(false);
			}
			destImage.Size = sourceImage.Size;
			destImage.Palette = palette.Clone();

			var sourcePixels = sourceImage.Pixels;
			var destPixels = destImage.Pixels;

			const int ErrorCoeff = MaxDitherLevel * MaxDitherLevel * 16;
				
			var tempPixels = new int[sourcePixels.Length, 3];
			for (var i = 0; i < sourcePixels.Length; i++)
			{
				var colorHash = sourcePixels[i];
				tempPixels[i, ColorBytes.RedIdx] = colorHash.Red() * ErrorCoeff;
				tempPixels[i, ColorBytes.GreenIdx] = colorHash.Green() * ErrorCoeff;
				tempPixels[i, ColorBytes.BlueIdx] = colorHash.Blue() * ErrorCoeff;
			}

			var ditherLevel2 = ditherLevel * ditherLevel;
			var ditherCoeffRight = 7 * ditherLevel2;
			var ditherCoeffBottomLeft = 3 * ditherLevel2;
			var ditherCoeffBottomMiddle = 5 * ditherLevel2;
			var ditherCoeffBottomRight = 1 * ditherLevel2;

			var searcher = new PaletteQuickColorSearcher(palette);

			var sourceWidth = sourceImage.Size.Width;
			for (var i = 0; i < sourcePixels.Length; i++)
			{
				var r = tempPixels[i, ColorBytes.RedIdx] / ErrorCoeff;
				if (r < 0)
				{
					r = 0;
				}
				if (r > 255)
				{
					r = 255;
				}

				var g = tempPixels[i, ColorBytes.GreenIdx] / ErrorCoeff;
				if (g < 0)
				{
					g = 0;
				}
				if (g > 255)
				{
					g = 255;
				}

				var b = tempPixels[i, ColorBytes.BlueIdx] / ErrorCoeff;
				if (b < 0)
				{
					b = 0;
				}
				if (b > 255)
				{
					b = 255;
				}

				var colorSubs = searcher.SearchSubstitute((byte)r, (byte)g, (byte)b);
				Debug.Assert(colorSubs != null, "Substitute color was not found.");
				destPixels[i] = colorSubs.Argb;

				// Populate errors
				var colorHash = sourcePixels[i];
				var dr = colorHash.Red() - colorSubs.R;
				var dg = colorHash.Green() - colorSubs.G;
				var db = colorHash.Blue() - colorSubs.B;

				var index = i + 1; // right
				if ((index < sourcePixels.Length) && ((index % sourceWidth) != 0))
				{
					tempPixels[index, ColorBytes.RedIdx] += dr * ditherCoeffRight;
					tempPixels[index, ColorBytes.GreenIdx] += dg * ditherCoeffRight;
					tempPixels[index, ColorBytes.BlueIdx] += db * ditherCoeffRight;
				}

				index = i + sourceWidth - 1; // bottom left
				if ((index < sourcePixels.Length) && ((i % sourceWidth) != 0))
				{
					tempPixels[index, ColorBytes.RedIdx] += dr * ditherCoeffBottomLeft;
					tempPixels[index, ColorBytes.GreenIdx] += dg * ditherCoeffBottomLeft;
					tempPixels[index, ColorBytes.BlueIdx] += db * ditherCoeffBottomLeft;
				}

				index++; // bottom middle
				if (index < sourcePixels.Length)
				{
					tempPixels[index, ColorBytes.RedIdx] += dr * ditherCoeffBottomMiddle;
					tempPixels[index, ColorBytes.GreenIdx] += dg * ditherCoeffBottomMiddle;
					tempPixels[index, ColorBytes.BlueIdx] += db * ditherCoeffBottomMiddle;
				}

				index++; // bottom right
				if ((index < sourcePixels.Length) && ((index % sourceWidth) != 0))
				{
					tempPixels[index, ColorBytes.RedIdx] += dr * ditherCoeffBottomRight;
					tempPixels[index, ColorBytes.GreenIdx] += dg * ditherCoeffBottomRight;
					tempPixels[index, ColorBytes.BlueIdx] += db * ditherCoeffBottomRight;
				}
			}

			return destImage;
		}

		#endregion
	}
}
