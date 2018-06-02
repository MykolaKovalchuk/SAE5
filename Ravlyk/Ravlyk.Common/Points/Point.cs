using System;

namespace Ravlyk.Common
{
	/// <summary>
	/// Represents a point in XY coordinates.
	/// </summary>
	public struct Point
	{
		/// <summary>
		/// Initializes new point.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		// Analysis disable InconsistentNaming

		/// <summary>
		/// X coordinate.
		/// </summary>
		public int X { get; }

		/// <summary>
		/// Y coordinate.
		/// </summary>
		public int Y { get; }

		// Analysis restore InconsistentNaming

		#region Overrides

		public static bool operator ==(Point s1, Point s2)
		{
			return s1.Equals(s2);
		}

		public static bool operator !=(Point s1, Point s2)
		{
			return !(s1 == s2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Point))
			{
				return false;
			}

			var otherPoint = (Point)obj;
			return X == otherPoint.X && Y == otherPoint.Y;
		}

		public override int GetHashCode()
		{
			return (X << 16) ^ Y;
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		#endregion
	}
}
