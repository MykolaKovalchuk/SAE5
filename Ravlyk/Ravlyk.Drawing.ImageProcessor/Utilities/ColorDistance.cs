using System;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Utilities
{
	/// <summary>
	/// Colors distance calculator.
	/// </summary>
	public static class ColorDistance
	{
		static ColorDistance()
		{
			SetCoeffs(30, 59, 11);
		}

		/// <summary>
		/// Calculated square decart distance from one color to another.
		/// </summary>
		/// <param name="c1">First color to get distance to.</param>
		/// <param name="c2">Second color to get distance to.</param>
		/// <returns>Integer value, representing square distance to specified color.</returns>
		public static int GetSquareDistance(this Color c1, Color c2)
		{
			return GetSquareDistance(c1.R, c1.G, c1.B, c2.R, c2.G, c2.B);
		}

		public static int GetSquareDistance(byte c1R, byte c1G, byte c1B, byte c2R, byte c2G, byte c2B)
		{
			int dR = c1R - c2R;
			int dG = c1G - c2G;
			int dB = c1B - c2B;

			return dR * dR + dG * dG + dB * dB;
		}

		/// <summary>
		/// Calculated visual distance from one color to another.
		/// </summary>
		/// <param name="c1">First color to get distance to.</param>
		/// <param name="c2">Second color to get distance to.</param>
		/// <returns>Integer value, representing visual distance to specified color.</returns>
		public static int GetVisualDistance(this Color c1, Color c2)
		{
			return GetVisualDistance(c1.R, c1.G, c1.B, c2.R, c2.G, c2.B);
		}

		public static int GetVisualDistance(byte c1R, byte c1G, byte c1B, byte c2R, byte c2G, byte c2B)
		{
			int dR = c1R - c2R;
			int dG = c1G - c2G;
			int dB = c1B - c2B;

			return CoeffRed * dR * dR + CoeffGreen * dG * dG + CoeffBlue * dB * dB;
		}

		/// <summary>
		/// Calculated visual distance from one color to another, including enhanced color properties (like HUE components).
		/// </summary>
		/// <param name="c1">First color to get distance to.</param>
		/// <param name="c2">Second color to get distance to.</param>
		/// <returns>Integer value, representing visual distance to specified color.</returns>
		public static int GetVisualDistanceEnhanced(this Color c1, Color c2)
		{
			int distance = GetVisualDistance(c1, c2);

			if (CoeffHue != 0 || CoeffSaturation != 0 || CoeffBrightness != 0)
			{
				int dH = c1.Hue - c2.Hue;
				int dS = c1.Saturation - c2.Saturation;
				int dB = c1.Brightness - c2.Brightness;

				distance += CoeffHue * dH * dH + CoeffSaturation * dS * dS + CoeffBrightness * dB * dB;
			}

			return distance;
		}

		#region Color component weight coeffitients

		public static IDisposable UseCoeffs(int red, int green, int blue, int hue = 0, int saturation = 0, int brightness = 0)
		{
			int originalRed = CoeffRed;
			int originalGreen = CoeffGreen;
			int originalBlue = CoeffBlue;
			int originalHue = CoeffHue;
			int originalSaturation = CoeffSaturation;
			int originalBrightness = CoeffBrightness;

			SetCoeffs(red, green, blue, hue, saturation, brightness);

			return new DisposableAction(() => SetCoeffs(originalRed, originalGreen, originalBlue, originalHue, originalSaturation, originalBrightness));
		}

		public static void SetCoeffs(int red, int green, int blue, int hue = 0, int saturation = 0, int brightness = 0)
		{
			CoeffRed = red;
			CoeffGreen = green;
			CoeffBlue = blue;
			CoeffHue = hue;
			CoeffSaturation = saturation;
			CoeffBrightness = brightness;
		}

		/// <summary>
		/// Red value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffRed { get; private set; }

		/// <summary>
		/// Green value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffGreen { get; private set; }

		/// <summary>
		/// Blue value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffBlue { get; private set; }

		/// <summary>
		/// Hue value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffHue { get; private set; }

		/// <summary>
		/// Saturation value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffSaturation { get; private set; }

		/// <summary>
		/// Brightness value coeffitient for visual distance calculation.
		/// </summary>
		public static int CoeffBrightness { get; private set; }

		#endregion
	}
}
