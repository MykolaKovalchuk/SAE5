using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Resampler of <see cref="Ravlyk.Drawing.IndexedImage"/> object image size.
	/// </summary>
	public class ImageResampler
	{
		#region Filter types

		/// <summary>
		/// Enumeration of resampling filters.
		/// </summary>
		public enum FilterType
		{
			/// <summary>
			/// Simpliest no-smoothing resampler.
			/// </summary>
			Box,

			/// <summary>
			/// Triangle filter resampler.
			/// </summary>
			Triangle,

			/// <summary>
			/// Hermite filter resampler.
			/// </summary>
			Hermite,

			/// <summary>
			/// Bell filter resampler.
			/// </summary>
			Bell,

			/// <summary>
			/// B-Spline filter resampler.
			/// </summary>
			BSpline,
			/// <summary>
			/// Lanczos resampler.
			/// </summary>
			Lanczos3,

			/// <summary>
			/// Mitchell resampler.
			/// </summary>
			Mitchell
		}

		#endregion

		#region Instrumental definitions

		const int BiasShift = 16;
		const int Bias = 1 << BiasShift;

		/// <summary>
		/// Radius for smoothing filter function.
		/// </summary>
		private double radiusSmoothing;

		/// <summary>
		/// Structure of weighted value.
		/// </summary>
		private struct WeightedValue
		{
			/// <summary>
			/// Index of real value in external array.
			/// </summary>
			public int Index;

			/// <summary>
			/// Weight of value.
			/// </summary>
			public int Weight;
		}

		/// <summary>
		/// Array of arrays of weighted values.
		/// </summary>
		private WeightedValue[][] weightedValues;

		/// <summary>
		/// Smoothing filter function.
		/// </summary>
		/// <remarks>Is initialized by <see cref="PrepareInternalParams"/>.</remarks>
		private Func<double, double> filterFunc;

		/// <summary>
		/// Preparing half iteration function.
		/// </summary>
		/// <remarks>Is initialized by <see cref="PrepareInternalParams"/>.</remarks>
		private Action<int, int, int, int> prepareWeightsFunc;

		#endregion

		/// <summary>
		/// Changes sizes of source <see cref="IndexedImage"/> object with using of specified <see cref="filter"/> and truncates with <see cref="destImageFrame"/>.
		/// </summary>
		/// <param name="sourceImage">The source <see cref="IndexedImage"/> object.</param>
		/// <param name="newSize">The new image size.</param>
		/// <param name="filter">Filter for resampling.</param>
		/// <param name="destImage">Destionation <see cref="IndexedImage"/> object or null.</param>
		/// <param name="destImageFrame"><see cref="Rectangle"/> specifying a frame within resampled image that should be copied into <see cref="destImage"/>.</param>
		/// <returns>New or updated frame of resized <see cref="IndexedImage"/> object.</returns>
		public IndexedImage Resample(IndexedImage sourceImage, Size newSize, FilterType filter, Rectangle destImageFrame = default(Rectangle), IndexedImage destImage = null)
		{
			PrepareInternalParams(filter);

			Debug.Assert(newSize.Width > 0 && newSize.Height > 0, "New size should be bigger then (0, 0).");

			Debug.Assert(
				destImageFrame.Left >= 0 &&
				destImageFrame.Top >= 0 &&
				destImageFrame.Left + destImageFrame.Width <= newSize.Width &&
				destImageFrame.Top + destImageFrame.Height <= newSize.Height,
				"Destination image frame should be inside newSize limits.");

			if (destImageFrame.Width <= 0 || destImageFrame.Height <= 0)
			{
				destImageFrame = new Rectangle(0, 0, newSize.Width, newSize.Height);
			}
			if (destImage == null)
			{
				destImage = new IndexedImage();
			}
			destImage.Size = new Size(destImageFrame.Width, destImageFrame.Height);

			int[] oldPixels, newPixels; // Arrays for source and destination pixels

			/* Resizing by width */

			// Set current old and new image sizes
			var oldImageSize = sourceImage.Size;
			var newImageSize = new Size(newSize.Width, oldImageSize.Height);
			oldPixels = sourceImage.Pixels; // Get source pixels from source image

			// If new width differs from old
			if (newImageSize.Width != oldImageSize.Width || destImageFrame.Width != newImageSize.Width)
			{
				// Calculate required vertical area
				int requiredTop;
				if (destImageFrame.Top > 0)
				{
					prepareWeightsFunc(sourceImage.Size.Height, newSize.Height, destImageFrame.Top, 1);
					requiredTop = weightedValues[0].Select(w => w.Index).Min();
				}
				else
				{
					requiredTop = 0;
				}

				int requiredBottomExclusive;
				if (destImageFrame.BottomExclusive < newSize.Height)
				{
					prepareWeightsFunc(sourceImage.Size.Height, newSize.Height, destImageFrame.BottomExclusive - 1, 1);
					requiredBottomExclusive = weightedValues[0].Select(w => w.Index).Max() + 1;
				}
				else
				{
					requiredBottomExclusive = newImageSize.Height;
				}

				newPixels = new int[destImageFrame.Width * newImageSize.Height];
				prepareWeightsFunc(oldImageSize.Width, newImageSize.Width, destImageFrame.Left, destImageFrame.Width); // Prepare weights for half iteration
				newImageSize.Width = destImageFrame.Width;
				Parallel.For(requiredTop, requiredBottomExclusive, ystart => ResampleLine(oldPixels, newPixels, ystart * oldImageSize.Width, ystart * newImageSize.Width, 1, 1));
			}
			else
			{
				// Use old pixels without changes
				newPixels = oldPixels;
			}

			/* Resizing by height */

			// Set current old and new image sizes
			oldImageSize = newImageSize;
			newImageSize = newSize;
			oldPixels = newPixels; // New pixels are now old
			newPixels = destImage.Pixels; // Get reference to destination pixels array

			// If new height differs from old
			if (newImageSize.Height != oldImageSize.Height || destImageFrame.Height != newImageSize.Height)
			{
				if (sourceImage.Size.Width != sourceImage.Size.Height || newSize.Width != newSize.Height || destImageFrame.Top != destImageFrame.Left || destImageFrame.Height != destImageFrame.Width)
				{
					prepareWeightsFunc(oldImageSize.Height, newImageSize.Height, destImageFrame.Top, destImageFrame.Height); // Prepare new weights for half iteration
				}
				newImageSize.Width = destImageFrame.Width;
				newImageSize.Height = destImageFrame.Height;
				Parallel.For(0, newImageSize.Width, xstart => ResampleLine(oldPixels, newPixels, xstart, xstart, oldImageSize.Width, newImageSize.Width));
			}
			else
			{
				oldPixels.CopyTo(newPixels, 0);
			}

			return destImage;
		}

		#region Preparing to resample

		/// <summary>
		/// Prepares internal params of calculations.
		/// </summary>
		/// <param name="filter">Filter type for resampling.</param>
		private void PrepareInternalParams(FilterType filter)
		{
			switch (filter)
			{
				case FilterType.Triangle:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterTriangle;
					radiusSmoothing = 1.0;
					break;
				case FilterType.Hermite:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterHermite;
					radiusSmoothing = 1.0;
					break;
				case FilterType.Bell:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterBell;
					radiusSmoothing = 1.5;
					break;
				case FilterType.BSpline:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterBSpline;
					radiusSmoothing = 2.0;
					break;
				case FilterType.Lanczos3:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterLanczos3;
					radiusSmoothing = 3.0;
					break;
				case FilterType.Mitchell:
					prepareWeightsFunc = PrepareWeights;
					filterFunc = FilterMitchell;
					radiusSmoothing = 2.0;
					break;
				case FilterType.Box:
				default:
					prepareWeightsFunc = PrepareWeightsBox;
					filterFunc = FilterBox;
					radiusSmoothing = 0.5;
					break;
			}
		}

		/// <summary>
		/// Precalculates weighted values for each index of point in new line.
		/// </summary>
		/// <param name="oldLength">The old line length.</param>
		/// <param name="newLength">The new line length.</param>
		/// <param name="startFrom">Starting index for weights values.</param>
		/// <param name="prepareCount">Count of weights values to prepare.</param>
		private void PrepareWeights(int oldLength, int newLength, int startFrom = 0, int prepareCount = 0)
		{
			Debug.Assert(startFrom >= 0, "startFrom should be no less than 0.");
			Debug.Assert(startFrom + prepareCount <= newLength, "prepareCount should not go over newLength.");

			if (prepareCount <= 0)
			{
				prepareCount = newLength - startFrom;
			}

			double scale = (double)newLength / (double)oldLength;
			double fscale;
			double radius;
			if (scale < 1.0)
			{
				radius = radiusSmoothing / scale;
				fscale = 1.0 / scale;
			}
			else
			{
				radius = radiusSmoothing;
				fscale = 1.0;
			}

			weightedValues = new WeightedValue[prepareCount][];

			// By all indexes in new line
			Parallel.For(startFrom, startFrom + prepareCount,
				i =>
				{
					double center = i / scale;
					int left = (int)Math.Floor(center - radius);
					int right = (int)Math.Ceiling(center + radius);

					weightedValues[i - startFrom] = new WeightedValue[right - left + 1];
					int count = 0;

					for (int j = left; j <= right; j++)
					{
						// Calculate weight of element
						int weight = (int)(filterFunc((center - j) / fscale) / fscale * Bias + 0.5);

						if (weight != 0)
						{
							// Calculate index of pixel in old array
							int index;
							if (j < 0)
							{
								index = -j;
							}
							else if (j >= oldLength)
							{
								index = oldLength * 2 - j - 1;
							}
							else
							{
								index = j;
							}

							// Save weighted value
							weightedValues[i - startFrom][count++] = new WeightedValue { Index = index, Weight = weight };
						}
					}

					Array.Resize(ref weightedValues[i - startFrom], count);
				});
		}

		/// <summary>
		/// Precalculates weighted values for each index of point in new line (Box filter).
		/// </summary>
		/// <param name="oldLength">The old line length.</param>
		/// <param name="newLength">The new line length.</param>
		/// <param name="startFrom">Starting index for weights values.</param>
		/// <param name="prepareCount">Count of weights values to prepare.</param>
		private void PrepareWeightsBox(int oldLength, int newLength, int startFrom = 0, int prepareCount = 0)
		{
			Debug.Assert(startFrom >= 0, "startFrom should be no less than 0.");
			Debug.Assert(startFrom + prepareCount <= newLength, "prepareCount should not go over newLength.");

			if (prepareCount <= 0)
			{
				prepareCount = newLength - startFrom;
			}

			double scale = (double)newLength / (double)oldLength;
			double radius = scale < 1.0 ? radiusSmoothing / scale : radiusSmoothing;

			weightedValues = new WeightedValue[prepareCount][];

			// By all indexes in new line
			Parallel.For(startFrom, startFrom + prepareCount,
				i =>
				{
					double center = i / scale;
					int left;
					int right;
					if (scale > 1.0)
					{
						left = right = (int)(center + 0.5);
					}
					else
					{
						left = (int)Math.Floor(center - radius);
						right = (int)Math.Ceiling(center + radius);
					}

					// Add correction for Box filter (radiusSmoothing == 0.5) and calculate weight of element
					int correctionSize = right - left + 1;
					int weight = (correctionSize == 1) ? Bias : (Bias + correctionSize / 2) / correctionSize;

					weightedValues[i - startFrom] = new WeightedValue[correctionSize];
					for (int j = left, count = 0; j <= right; j++, count++)
					{
						// Calculate index of pixel in old array
						int index;
						if (j < 0)
						{
							index = -j;
						}
						else if (j >= oldLength)
						{
							index = oldLength * 2 - j - 1;
						}
						else
						{
							index = j;
						}

						// Save weighted value
						weightedValues[i - startFrom][count] = new WeightedValue { Index = index, Weight = weight };
					}
				});
		}

		#endregion

		#region ResampleLine

		/// <summary>
		/// Changes sizes of one pixel's line.
		/// </summary>
		/// <param name="oldArray">Old array with pixels.</param>
		/// <param name="newArray">New array with pixels.</param>
		/// <param name="oldStartIndex">Start index in old array.</param>
		/// <param name="newStartIndex">Start index in new array.</param>
		/// <param name="oldOneStep">Distance between points in old array.</param>
		/// <param name="newOneStep">Distance between points in new array.</param>
		private void ResampleLine(int[] oldArray, int[] newArray, int oldStartIndex, int newStartIndex, int oldOneStep, int newOneStep)
		{
			// By all pixels in new array
			// weightedValues.Length == new length
			for (int i = 0, newIndexPart = newStartIndex; i < weightedValues.Length; i++, newIndexPart += newOneStep)
			{
				// Initialize new color components 0.5 is for smoother truncating (instead of Math.Round)
				int b, g, r, a;
				b = g = r = a = 0;

				WeightedValue[] weightedValuesList = weightedValues[i];
				for (int j = 0; j < weightedValuesList.Length; j++)
				{
					// Get index and weight of old element
					int weight = weightedValuesList[j].Weight;
					int oldIndex = oldStartIndex + weightedValuesList[j].Index * oldOneStep;
					if (oldIndex < 0) oldIndex = 0;
					if (oldIndex >= oldArray.Length) oldIndex = oldArray.Length - 1;

					// Update color components
					int argb = oldArray[oldIndex];
					b += weight * argb.Blue();
					g += weight * argb.Green();
					r += weight * argb.Red();
					a += weight * argb.Alpha();
				}

				// Calculate index of pixel in new array
				int newIndex = newIndexPart;
				if (newIndex < 0) newIndex = 0;
				if (newIndex >= newArray.Length) newIndex = oldArray.Length - 1;

				// Save new color
				newArray[newIndex] = ColorBytes.ToArgb(GetNormalizedValue(a), GetNormalizedValue(r), GetNormalizedValue(g), GetNormalizedValue(b));
			}
		}

		private static byte GetNormalizedValue(int value)
		{
			value /= Bias;
			if (value < 0)
			{
				return 0;
			}
			if (value > 255)
			{
				return 255;
			}
			return (byte)value;
		}

		#endregion

		#region Filter functions

		/// <summary>
		/// Box resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterBox(double value)
		{
			return (value > -0.5) && (value < 0.5) ? 1.0 : 0.0;
		}

		/// <summary>
		/// Triangle resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterTriangle(double value)
		{
			if (value < 0.0)
			{
				value = -value;
			}
			return value < 1.0 ? 1.0 - value : 0.0;
		}

		/// <summary>
		/// Hermite resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterHermite(double value)
		{
			if (value < 0.0)
			{
				value = -value;
			}
			return value < 1.0 ? (2.0 * value - 3.0) * value * value + 1.0 : 0.0;
		}

		/// <summary>
		/// Bell resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterBell(double value)
		{
			if (value < 0.0)
			{
				value = -value;
			}
			if (value < 0.5)
			{
				return 0.75 - value * value;
			}
			if (value < 1.5)
			{
				value = 1.5 - value;
				return 0.5 * value * value;
			}

			return 0.0;
		}

		/// <summary>
		/// BSpline resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterBSpline(double value)
		{
			if (value < 0.0)
			{
				value = -value;
			}
			if (value < 1.0)
			{
				double tt = value * value;
				return 0.5 * tt * value - tt + 2.0 / 3.0;
			}
			if (value < 2.0)
			{
				value = 2.0 - value;
				return 1.0 / 6.0 * value * value * value;
			}

			return 0.0;
		}

		/// <summary>
		/// Lanczos3 resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterLanczos3(double value)
		{
			if (value < 0.0)
			{
				value = -value;
			}
			return value < 3.0 ? SinC(value) * SinC(value / 3.0) : 0.0;
		}

		private static double SinC(double value)
		{
			if (value != 0.0)
			{
				value = value * Math.PI;
				return (Math.Sin(value) / value);
			}

			return 1.0;
		}

		/// <summary>
		/// Mitchell resample filter.
		/// </summary>
		/// <param name="value">Distance between pixels.</param>
		/// <returns>Weight of source pixel.</returns>
		private static double FilterMitchell(double value)
		{
			const double b = 1.0 / 3.0;
			const double c = b;

			if (value < 0.0)
			{
				value = -value;
			}
			double tt = value * value;
			if (value < 1.0)
			{
				value = ((12.0 - 9.0 * b - 6.0 * c) * (value * tt)) + ((-18.0 + 12.0 * b + 6.0 * c) * tt) + (6.0 - 2 * b);
				return value / 6.0;
			}
			if (value < 2.0)
			{
				value = ((-1.0 * b - 6.0 * c) * (value * tt)) + ((6.0 * b + 30.0 * c) * tt) + ((-12.0 * b - 48.0 * c) * value) + (8.0 * b + 24 * c);
				return value / 6.0;
			}

			return 0.0;
		}

		#endregion
	}
}
