using System;
using System.Diagnostics;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Quickly shifts image pixels horizontally and vertically.
	/// Content of vacated areas can be overridden with some hidden areas or remain original - it should not be dependent on.
	/// </summary>
	public static class ImageShifter
	{
		/// <summary>
		/// Quickly shifts image pixels horizontally and vertically.
		/// Content of vacated areas can be overridden with some hidden areas or remain original - it should not be dependent on.
		/// </summary>
		/// <param name="image">Image object which pixels should be shifter.</param>
		/// <param name="dx">Horizontal shift distance.</param>
		/// <param name="dy">Vertical shift distance.</param>
		public static void ShiftPixels(IndexedImage image, int dx, int dy)
		{
			var startPoint = new Point(dx < 0 ? -dx : 0, dy < 0 ? -dy : 0);
			if ((dx == 0 && dy == 0) ||
				dx >= image.Size.Width || dy >= image.Size.Width ||
				startPoint.X >= image.Size.Width || startPoint.Y >= image.Size.Width)
			{
				return;
			}

			var startIndex = startPoint.Y * image.Size.Width + startPoint.X;
			var distance = dy * image.Size.Width + dx;
			var movingLength = image.Pixels.Length - startIndex - (distance > 0 ? distance : 0);

			Debug.Assert(movingLength >= 0);
			Debug.Assert(startIndex >= 0);
			Debug.Assert(startIndex + movingLength <= image.Pixels.Length);
			Debug.Assert(startIndex + distance >= 0);
			Debug.Assert(startIndex + distance + movingLength <= image.Pixels.Length);

			Array.Copy(image.Pixels, startIndex, image.Pixels, startIndex + distance, movingLength);
		}
	}
}
