using System;

namespace Ravlyk.Drawing
{
	/// <summary>
	/// Color's 4 bytes indexes, masks, and operations on <see cref="int"/> value.
	/// </summary>
	public static class ColorBytes
	{
		#region Components index constants

		/// <summary>
		/// Index of alpha byte in int ARGB value.
		/// </summary>
		public const int AlphaIdx = 3;
		/// <summary>
		/// Index of red byte in int ARGB value.
		/// </summary>
		public const int RedIdx = 2;
		/// <summary>
		/// Index of green byte in int ARGB value.
		/// </summary>
		public const int GreenIdx = 1;
		/// <summary>
		/// Index of blue byte in int ARGB value.
		/// </summary>
		public const int BlueIdx = 0;

		/// <summary>
		/// Index of first alpha bit in 32-bit ARGB value.
		/// </summary>
		public const int AlphaBias = AlphaIdx * 8;
		/// <summary>
		/// Index of first red bit in 32-bit ARGB value.
		/// </summary>
		public const int RedBias = RedIdx * 8;
		/// <summary>
		/// Index of first green bit in 32-bit ARGB value.
		/// </summary>
		public const int GreenBias = GreenIdx * 8;
		/// <summary>
		/// Index of first blue bit in 32-bit ARGB value.
		/// </summary>
		public const int BlueBias = BlueIdx * 8;

		/// <summary>
		/// Base FF mask.
		/// </summary>
		private const int FFMask = 0xff;
		/// <summary>
		/// Mask of alpha bits in 32-bit ARGB value.
		/// </summary>
		public const int AlphaMask = FFMask << AlphaBias;
		/// <summary>
		/// Mask of red bits in 32-bit ARGB value.
		/// </summary>
		public const int RedMask = FFMask << RedBias;
		/// <summary>
		/// Mask of green bits in 32-bit ARGB value.
		/// </summary>
		public const int GreenMask = FFMask << GreenBias;
		/// <summary>
		/// Mask of blue bits in 32-bit ARGB value.
		/// </summary>
		public const int BlueMask = FFMask << BlueBias;

		#endregion

		#region Components extraction

		/// <summary>
		/// Extracts Alpha color component
		/// </summary>
		/// <param name="argb">Composite 32-bit integer color value.</param>
		/// <returns>Alpha color component.</returns>
		/// <value>0..255</value>
		public static byte Alpha(this int argb)
		{
			return (byte)((argb & AlphaMask) >> AlphaBias);
		}

		/// <summary>
		/// Extracts Red color component
		/// </summary>
		/// <param name="argb">Composite 32-bit integer color value.</param>
		/// <returns>Red color component.</returns>
		/// <value>0..255</value>
		public static byte Red(this int argb)
		{
			return (byte)((argb & RedMask) >> RedBias);
		}

		/// <summary>
		/// Extracts Green color component
		/// </summary>
		/// <param name="argb">Composite 32-bit integer color value.</param>
		/// <returns>Green color component.</returns>
		/// <value>0..255</value>
		public static byte Green(this int argb)
		{
			return (byte)((argb & GreenMask) >> GreenBias);
		}

		/// <summary>
		/// Extracts Blue color component
		/// </summary>
		/// <param name="argb">Composite 32-bit integer color value.</param>
		/// <returns>Blue color component.</returns>
		/// <value>0..255</value>
		public static byte Blue(this int argb)
		{
			return (byte)((argb & BlueMask) >> BlueBias);
		}

		#endregion

		#region Components composition

		/// <summary>
		/// Combines color components into single 32-bit integer value.
		/// </summary>
		/// <param name="alpha">Alpha color component.</param>
		/// <param name="red">Red color component.</param>
		/// <param name="green">Green color component.</param>
		/// <param name="blue">Blue color component.</param>
		/// <returns>32-bit integer value of color combined from 4 8-bit components.</returns>
		public static int ToArgb(byte alpha, byte red, byte green, byte blue)
		{
			return (alpha << AlphaBias) | (red << RedBias) | (green << GreenBias) | (blue << BlueBias);
		}

		#endregion

		#region Sub-colors

		/// <summary>
		/// Half dark color darkness.
		/// </summary>
		public const int HalfDark = 127 * 3;

		/// <summary>
		/// High half tone color value.
		/// </summary>
		/// <param name="argb">Source color.</param>
		/// <returns>Color in half tone higher from given.</returns>
		/// <remarks>Alpha component is half tone low for 0..127, and half tone hight for 128..255.</remarks>
		public static int HalfToneHigh(this int argb)
		{
			return unchecked((int)((uint)(argb >> 1) | 0x00808080));
		}

		/// <summary>
		/// High 3/4 tone color value.
		/// </summary>
		/// <param name="argb">Source color.</param>
		/// <returns>Color in 3/4 tone higher from given.</returns>
		public static int ThreeQuarterToneHigh(this int argb)
		{
			return unchecked((int)((uint)(argb >> 2) | 0x00C0C0C0));
		}

		/// <summary>
		/// Low half tone color value.
		/// </summary>
		/// <param name="argb">Source color.</param>
		/// <returns>Color in half tone lower from given.</returns>
		public static int HalfToneLow(this int argb)
		{
			return unchecked((int)((uint)(argb >> 1) & 0x7f7f7f7f));
		}

		#endregion

		#region Composing

		/// <summary>
		/// Composes two colors using alpha value specified for second color.
		/// </summary>
		/// <param name="colorA">First color.</param>
		/// <param name="colorB">Second color.</param>
		/// <param name="opacityB">Opacity of second color.</param>
		/// <returns>Argb value of composed colors.</returns>
		/// <remarks>Keeps alpha value of first color.</remarks>
		public static int ComposeTwoColors(int colorA, int colorB, byte opacityB)
		{
			var intensityB = (int)opacityB;
			var intensityA = 255 - intensityB;

			var a = colorA.Alpha();
			var r = (byte)((intensityA * colorA.Red() + intensityB * colorB.Red() + 127) / 255);
			var g = (byte)((intensityA * colorA.Green() + intensityB * colorB.Green() + 127) / 255);
			var b = (byte)((intensityA * colorA.Blue() + intensityB * colorB.Blue() + 127) / 255);

			return ToArgb(a, r, g, b);
		}

		/// <summary>
		/// Adjusts brightnes of source color with shade color.
		/// </summary>
		/// <param name="sourceColor">Source color.</param>
		/// <param name="shadeColor">Shade reference color.</param>
		/// <returns>Argb value of shaded color.</returns>
		/// <remarks>Keepo alpha of source color.</remarks>
		public static int ShadeColor(int sourceColor, int shadeColor)
		{
			var a = sourceColor.Alpha();
			var r = (byte)((sourceColor.Red() * shadeColor.Red() + 127) / 255);
			var g = (byte)((sourceColor.Green() * shadeColor.Green() + 127) / 255);
			var b = (byte)((sourceColor.Blue() * shadeColor.Blue() + 127) / 255);

			return ToArgb(a, r, g, b);
		}

		#endregion

		#region Inverting

		/// <summary>
		/// Inverts the bytes order in specified color integer (ARGB - BGRA).
		/// </summary>
		/// <param name="color">Int32 value representing color.</param>
		/// <returns>Int32 with bytes in opposite order.</returns>
		public static int InvertBytes(this int color)
		{
			return
				(color << 24) |
				((color << 8) & 0x00ff0000) |
				((color >> 8) & 0x0000ff00) |
				(color >> 24);
		}

		#endregion
	}
}
