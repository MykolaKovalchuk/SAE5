using System;
using System.Threading.Tasks;
using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Copies an image to other image.
	/// </summary>
	public static class ImageCopier
	{
		/// <summary>
		/// Copies pixels of source image into destinaiton image at specified location.
		/// </summary>
		/// <param name="sourceImage">Source image.</param>
		/// <param name="destImage">Destination image.</param>
		/// <param name="location">Coordinates in destination image where source image should be copied to (left top corner).</param>
		public static void Copy(IndexedImage sourceImage, IndexedImage destImage, Point location)
		{
			Parallel.For(0, sourceImage.Size.Height,
				y =>
				{
					var destY = y + location.Y;
					if (destY >= 0 && destY < destImage.Size.Height)
					{
						var xPosition = location.X < 0 ? 0 : location.X;
						var xCorrection = location.X < 0 ? -location.X : 0;
						var length = Math.Min(sourceImage.Size.Width, destImage.Size.Width - xPosition - xCorrection);
						if (length > 0)
						{
							Array.Copy(sourceImage.Pixels, y * sourceImage.Size.Width + xCorrection, destImage.Pixels, destY * destImage.Size.Width + xPosition, length);
						}
					}
				});
		}

		/// <summary>
		/// Copies pixels of source image into destinaiton image at specified location.
		/// </summary>
		/// <param name="sourceImage">Source image.</param>
		/// <param name="destImage">Destination image.</param>
		/// <param name="location">Coordinates in destination image where source image should be copied to (left top corner).</param>
		public static void CopyWithPalette(IndexedImage sourceImage, IndexedImage destImage, Point location)
		{
			for (int y1 = 0, y2 = location.Y; y1 < sourceImage.Size.Height && y2 < destImage.Size.Height; y1++, y2++)
			{
				for (int x1 = 0, x2 = location.X; x1 < sourceImage.Size.Width && x2 < destImage.Size.Width; x1++, x2++)
				{
					destImage[x2, y2] = sourceImage[x1, y1];
				}
			}
		}
	}
}
