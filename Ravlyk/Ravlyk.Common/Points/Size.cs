using System;

namespace Ravlyk.Common
{
	/// <summary>
	/// Represents a size of rectangular object.
	/// </summary>
	public struct Size
	{
		/// <summary>
		/// Initializes new rectangular size record.
		/// </summary>
		/// <param name="width">Width value.</param>
		/// <param name="height">Height value.</param>
		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		// Analysis disable InconsistentNaming

		/// <summary>
		/// Width value.
		/// </summary>
		public int Width;

		/// <summary>
		/// Height value.
		/// </summary>
		public int Height;

		// Analysis restore InconsistentNaming

		#region Overrides

		public static bool operator ==(Size s1, Size s2)
		{
			return s1.Equals(s2);
		}

		public static bool operator !=(Size s1, Size s2)
		{
			return !(s1 == s2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Size))
			{
				return false;
			}

			var otherSize = (Size)obj;
			return Width == otherSize.Width && Height == otherSize.Height;
		}

		public override int GetHashCode()
		{
			return (Width << 16) ^ Height;
		}

		public override string ToString()
		{
			return $"({Width}, {Height})";
		}

		#endregion
	}
}
