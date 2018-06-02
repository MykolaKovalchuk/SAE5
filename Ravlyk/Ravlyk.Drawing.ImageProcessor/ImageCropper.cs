using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Creates cropped copies of source pictures.
	/// </summary>
	public static class ImageCropper
	{
		/// <summary>
		/// Cropping method.
		/// </summary>
		public enum CropKind
		{
			/// <summary>
			/// Croping with rectangle bounds.
			/// </summary>
			Rectangle,

			/// <summary>
			/// Cropping with arc bounds.
			/// </summary>
			Arc,

			/// <summary>
			/// No cropping.
			/// </summary>
			None
		}

		/// <summary>
		/// Creates cropped copy of source picture.
		/// </summary>
		/// <param name="sourceImage">Source picture instance.</param>
		/// <param name="cropRect">Rectange with cropping bounds.</param>
		/// <param name="cropKind">Cropping method.</param>
		/// <param name="destImage">Destionation <see cref="IndexedImage"/> object or null.</param>
		/// <returns>New or updated <see cref="IndexedImage"/> object with cropped picture.</returns>
		public static IndexedImage Crop(IndexedImage sourceImage, Rectangle cropRect, CropKind cropKind = CropKind.Rectangle, IndexedImage destImage = null)
		{
			Debug.Assert(destImage != sourceImage, "It's impossible to crop image into itself.");

			if (destImage == null)
			{
				destImage = new IndexedImage();
			}

			if (cropKind == CropKind.None)
			{
				destImage.Size = sourceImage.Size;
				sourceImage.Pixels.CopyTo(destImage.Pixels, 0);
			}
			else
			{
				destImage.Size = new Size(cropRect.Width, cropRect.Height);
				switch (cropKind)
				{
					case CropKind.Arc:
						CropArc(sourceImage.Pixels, destImage.Pixels, sourceImage.Size, cropRect);
						break;
					case CropKind.Rectangle:
					default:
						CropRect(sourceImage.Pixels, destImage.Pixels, sourceImage.Size, cropRect);
						break;
				}
			}

			return destImage;
		}

		static void CropArc(int[] sp, int[] dp, Size sourceSize, Rectangle cropRect)
		{
			double aspect = (double)cropRect.Width / (double)cropRect.Height;
			double radius2 = cropRect.Width * cropRect.Width / 4.0;
			double centerX = cropRect.Width / 2.0;
			double centerY = cropRect.Height / 2.0;

			Parallel.For(0, cropRect.Height,
				row =>
				{
					double rowF = (centerY - row) * aspect;
					double rowF2 = rowF * rowF;
					int sourceIndex = (row + cropRect.Top) * sourceSize.Width + cropRect.Left;
					int destIndex = row * cropRect.Width;
					for (int col = 0; col < cropRect.Width; col++, sourceIndex++, destIndex++)
					{
						double colF = centerX - col;
						double colF2 = colF * colF;
						double distance = rowF2 + colF2;
						dp[destIndex] = distance <= radius2 ? sp[sourceIndex] : 0x00ffffff;
					}
				});
		}

		public static bool IsPointInsideArc(Point point, Rectangle cropRect)
		{
			double aspect = (double)cropRect.Width / (double)cropRect.Height;
			double radius2 = cropRect.Width * cropRect.Width / 4.0;
			double centerX = cropRect.Width / 2.0;
			double centerY = cropRect.Height / 2.0;

			double rowF = (centerY - point.Y) * aspect;
			double rowF2 = rowF * rowF;

			double colF = centerX - point.X;
			double colF2 = colF * colF;
			double distance = rowF2 + colF2;

			return distance <= radius2;
		}

		static void CropRect(int[] sp, int[] dp, Size sourceSize, Rectangle cropRect)
		{
			Parallel.For(0, cropRect.Height,
				row =>
				{
					int sourceIndex = (row + cropRect.Top) * sourceSize.Width + cropRect.Left;
					int destIndex = row * cropRect.Width;
					Array.Copy(sp, sourceIndex, dp, destIndex, cropRect.Width);
				});
		}
	}
}
