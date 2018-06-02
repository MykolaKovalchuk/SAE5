using System;
using System.Collections.Generic;

namespace Ravlyk.Common
{
	/// <summary>
	/// Represents a rectangular object in XY space.
	/// </summary>
	public struct Rectangle
	{
		/// <summary>
		/// Initializes new rectangle.
		/// </summary>
		/// <param name="left">X coordinate of left border.</param>
		/// <param name="top">Y coordinate of top border.</param>
		/// <param name="width">Width of rectangle.</param>
		/// <param name="height">Height of rectangle.</param>
		public Rectangle(int left, int top, int width, int height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		// Analysis disable InconsistentNaming

		/// <summary>
		/// X coordinate of left border.
		/// </summary>
		public int Left;

		/// <summary>
		/// Y coordinate of top border.
		/// </summary>
		public int Top;

		/// <summary>
		/// Width of rectangle.
		/// </summary>
		public int Width;

		/// <summary>
		/// Height of rectangle.
		/// </summary>
		public int Height;

		// Analysis restore InconsistentNaming

		/// <summary>
		/// Calculates right edge of rectangle + 1.
		/// </summary>
		public int RightExclusive => Left + Width;

		/// <summary>
		/// Calculates bottom edge of rectangle + 1.
		/// </summary>
		public int BottomExclusive => Top + Height;

		/// <summary>
		/// Checks if specified <see cref="Point"/> lies inside this rectangle including borders.
		/// </summary>
		/// <param name="p">Point to check.</param>
		/// <returns>True if specifie point lies inside or on border of this rectangle. False otherwise.</returns>
		public bool ContainsPoint(Point p)
		{
			return p.X >= Left && p.X < RightExclusive && p.Y >= Top && p.Y < BottomExclusive;
		}

		#region Overrides

		public static bool operator ==(Rectangle r1, Rectangle r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(Rectangle r1, Rectangle r2)
		{
			return !(r1 == r2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Rectangle))
			{
				return false;
			}

			var otherRect = (Rectangle)obj;
			return Left == otherRect.Left && Top == otherRect.Top && Width == otherRect.Width && Height == otherRect.Height;
		}

		public override int GetHashCode()
		{
			return (((((Left << 4) ^ Top) << 4) ^ Width) << 4) ^ Height;
		}

		public override string ToString()
		{
			return $"({Left}, {Top}, {Width}, {Height})";
		}

		#endregion

		#region Geometry operations

		/// <summary>
		/// Calculates XOR-intersection of 2 rectangles.
		/// </summary>
		/// <param name="rect2">Second rectangle te check intersection withs.</param>
		/// <returns>Enumeration of rectangles representing XOR-intersection area.</returns>
		public IEnumerable<Rectangle> Xor(Rectangle rect2)
		{
			if (this != rect2)
			{
				foreach (var rect in AMinusB(this, rect2))
				{
					yield return rect;
				}
				foreach (var rect in AMinusB(rect2, this))
				{
					yield return rect;
				}
			}
		}

		internal static IEnumerable<Rectangle> AMinusB(Rectangle a, Rectangle b)
		{
			if (b.RightExclusive <= a.Left || b.Left >= a.RightExclusive || b.BottomExclusive <= a.Top || b.Top >= a.BottomExclusive)
			{
				yield return a;
				yield break;
			}

			if (b.Left > a.Left && b.Left < a.RightExclusive)
			{
				yield return new Rectangle(a.Left, a.Top, b.Left - a.Left, a.Height);
				a = new Rectangle(b.Left, a.Top, a.RightExclusive - b.Left, a.Height);
			}
			if (b.RightExclusive > a.Left && b.RightExclusive < a.RightExclusive)
			{
				yield return new Rectangle(b.RightExclusive, a.Top, a.RightExclusive - b.RightExclusive, a.Height);
				a = new Rectangle(a.Left, a.Top, b.RightExclusive - a.Left, a.Height);
			}
			if (b.Top > a.Top && b.Top < a.BottomExclusive)
			{
				yield return new Rectangle(a.Left, a.Top, a.Width, b.Top - a.Top);
				a = new Rectangle(a.Left, b.Top, a.Width, a.BottomExclusive - b.Top);
			}
			if (b.BottomExclusive > a.Top && b.BottomExclusive < a.BottomExclusive)
			{
				yield return new Rectangle(a.Left, b.BottomExclusive, a.Width, a.BottomExclusive - b.BottomExclusive);
				//a = new Rectangle(a.Left, a.Top, a.Width, b.BottomExclusive - a.Top);
			}
		}

		#endregion
	}
}
