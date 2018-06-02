using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ravlyk.Drawing
{
	/// <summary>
	/// (Red, Green, Blue) color structure with advanced properties.
	/// </summary>
	public class Color
	{
		#region Value field

		/// <summary>
		/// Composition of (Alpha, Red, Green, Blue) color components.
		/// </summary>
		public readonly int Argb;

		/// <summary>
		/// Alpha color component.
		/// </summary>
		/// <value>0..255</value>
		public readonly byte A;

		/// <summary>
		/// Red color component.
		/// </summary>
		/// <value>0..255</value>
		public readonly byte R;

		/// <summary>
		/// Green color component.
		/// </summary>
		/// <value>0..255</value>
		public readonly byte G;

		/// <summary>
		/// Blue color component.
		/// </summary>
		/// <value>0..255</value>
		public readonly byte B;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Ravlyk.Drawing.Color"/> class and inits its internal <see cref="Argb"/> by combined in argb (Red, Green Blue) color components.
		/// </summary>
		/// <param name="argb">
		/// 32-bit (Alpha, Red, Green, Blue) color value.
		/// </param>
		public Color(int argb) : this(argb, argb.Alpha(), argb.Red(), argb.Green(), argb.Blue()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ravlyk.Drawing.Color"/> class and inits its internal <see cref="Argb"/> by (Red, Green, Blue) color components.
		/// </summary>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		public Color(byte r, byte g, byte b) : this(0, r, g, b) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ravlyk.Drawing.Color"/> class and inits its internal <see cref="Argb"/> by (Red, Green, Blue) color components.
		/// </summary>
		/// <param name="a">Alpha color component.</param>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		public Color(byte a, byte r, byte g, byte b) : this(ColorBytes.ToArgb(a, r, g, b), a, r, g, b) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ravlyk.Drawing.Color"/> class by copying it from source <see cref="Ravlyk.Drawing.Color"/> object.
		/// </summary>
		/// <param name="color">Source color of <see cref="Ravlyk.Drawing.Color"/> type.</param>
		public Color(Color color) : this(color.Argb, color.A, color.R, color.G, color.B) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ravlyk.Drawing.Color"/> class.
		/// </summary>
		/// <param name="argb">32-bit (Alpha, Red, Green, Blue) color value.</param>
		/// <param name="a">Alpha color component.</param>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		Color(int argb, byte a, byte r, byte g, byte b)
		{
			Debug.Assert(ColorBytes.ToArgb(a, r, g, b) == argb, "Color components should equal to combined value");

			Argb = argb;
			A = a;
			R = r;
			G = g;
			B = b;
		}

		#endregion

		#region Half tone colors

		/// <summary>
		/// Hight half tone color.
		/// </summary>
		public Color HalfToneHighColor => halfToneHighColor ?? (halfToneHighColor = new Color(Argb.HalfToneHigh()));
		Color halfToneHighColor;

		/// <summary>
		/// Low half tone color.
		/// </summary>
		public Color HalfToneLowColor => halfToneLowColor ?? (halfToneLowColor = new Color(Argb.HalfToneLow()));
		Color halfToneLowColor;

		/// <summary>
		/// Shows how dark is color.
		/// </summary>
		/// <value>0..765</value>
		public int Darkness => R + G + B;

		#endregion

		#region HSB

		/// <summary>
		/// Returns Hue value for color in HSV space.
		/// </summary>
		/// <value>0..255</value>
		public int Hue
		{
			get
			{
				if (hue < 0)
				{
					CalculateHSB();
				}
				return hue;
			}
		}
		int hue = -1;

		/// <summary>
		/// Returns Saturation value for color in HSV space.
		/// </summary>
		/// <value>0..255</value>
		public int Saturation
		{
			get
			{
				if (saturation < 0)
				{
					CalculateHSB();
				}
				return saturation;
			}
		}
		int saturation = -1;

		/// <summary>
		/// Returns Brightness value for color in HSV space.
		/// </summary>
		/// <value>0..255</value>
		public int Brightness
		{
			get
			{
				if (brightness < 0)
				{
					CalculateHSB();
				}
				return brightness;
			}
		}
		int brightness = -1;

		void CalculateHSB()
		{
			// TODO: Implement HSB calculation.
			throw new NotImplementedException();
		}

		#endregion

		#region Equals and HashCode

		/// <summary>
		/// Compares current object with <see cref="Ravlyk.Drawing.Color"/> structure on equal (Red, Green, Blue) color components.
		/// </summary>
		/// <param name="obj">The <see cref="Ravlyk.Drawing.Color"/> object to compare with the this <see cref="Ravlyk.Drawing.Color"/> object.</param>
		/// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.</returns>
		/// <remarks>Does not take into account Alpha component.</remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return ((Argb ^ (obj as Color)?.Argb) & 0x00ffffff) == 0;
		}

		/// <summary>
		/// Generates hash code.
		/// </summary>
		/// <remarks>Returns exactly internal <see cref="Argb"/> value.</remarks>
		/// <returns>A hash code for the current <see cref="System.Object"/>.</returns>
		public override int GetHashCode()
		{
			return Argb;
		}

		#endregion

		#region ToString

		public override string ToString()
		{
			return $"({R}, {G}, {B})";
		}

		#endregion

		#region Usage occurrences

		/// <summary>
		/// List of usage occurrences of color object.
		/// </summary>
		/// <remarks>Undefined after deserialization! It is needed to call ClearOccurrences() to create new List object, or get UsageOccurrences property.</remarks>
		internal List<int> UsageOccurrences => usageOccurrences ?? (usageOccurrences = new List<int>());
		List<int> usageOccurrences;

		/// <summary>
		/// Gets count of color's usage occurrences.
		/// </summary>
		public int OccurrencesCount
		{
			get { return usageOccurrences?.Count ?? occurrencesCountOverridden; }
			set { occurrencesCountOverridden = value; }
		}
		int occurrencesCountOverridden;

		/// <summary>
		/// Adds new usage occurrence of color.
		/// </summary>
		/// <param name="index">Index of pixel in bitmap where this color is used.</param>
		/// <remarks>Can create duplicates!</remarks>
		internal void AddOccurrence(int index)
		{
			UsageOccurrences.Add(index);
		}

		/// <summary>
		/// Removes usage occurrence of color.
		/// </summary>
		/// <param name="index">Index of pixel in bitmap where this color is used.</param>
		/// <returns>New color's usage occurrences count.</returns>
		/// <remarks>Removes only first found index occurrence (if there are duplicates).</remarks>
		internal int RemoveOccurrence(int index)
		{
			UsageOccurrences.Remove(index);
			return OccurrencesCount;
		}

		/// <summary>
		/// Clears all usage occurrences of color.
		/// </summary>
		public void ClearOccurrences()
		{
			usageOccurrences = null;
			OccurrencesCount = 0;
		}

		#endregion

		#region ICloneable

		public Color Clone()
		{
			return CloneCore();
		}

		protected virtual Color CloneCore()
		{
			var newColor = (Color)MemberwiseClone();
			newColor.usageOccurrences = null;
			return newColor;
		}

		#endregion
	}
}
