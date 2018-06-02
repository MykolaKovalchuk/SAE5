using System;
using System.Diagnostics;
using System.Drawing;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// WinForms-related extensions for <see cref="IndexedImage"/>
	/// </summary>
	public static class IndexedImageExtensions
	{
		/// <summary>
		/// Converts <see cref="Ravlyk.Drawing.IndexedImage"/> object to <see cref="System.Drawing.Bitmap"/> one with explicit operation.
		/// </summary>
		/// <param name="image">Source <see cref="Ravlyk.Drawing.IndexedImage"/> object.</param>
		/// <returns>New <see cref="System.Drawing.Bitmap"/> object with pixels copied from source image.</returns>
		public static Bitmap ToBitmap(this IndexedImage image)
		{
			Bitmap bitmap = new Bitmap(image.Size.Width, image.Size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			UpdateBitmap(image, bitmap);
			return bitmap;
		}

		/// <summary>
		/// Updates pixels in specified <see cref="System.Drawing.Bitmap"/> from this <see cref="IndexedImage"/> instance.
		/// </summary>
		/// <param name="image">Source <see cref="Ravlyk.Drawing.IndexedImage"/> object.</param>
		/// <param name="bitmap">Target <see cref="System.Drawing.Bitmap"/> to update.</param>
		public static void UpdateBitmap(this IndexedImage image, Bitmap bitmap)
		{
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			IntPtr ptr = bmpData.Scan0;
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				Debug.Assert(pixels != null, "Cannot lock pixels on the image.");
				System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length);
			}
			bitmap.UnlockBits(bmpData);
		}

		/// <summary>
		/// Converts <see cref="System.Drawing.Bitmap"/> object to <see cref="Ravlyk.Drawing.IndexedImage"/> one with explicit operation.
		/// </summary>
		/// <param name="bitmap">The source <see cref="T:System.Drawing.Bitmap"/> object.</param>
		/// <returns>New <see cref="Ravlyk.Drawing.IndexedImage"/> object initialized from source bitmap.</returns>
		public static IndexedImage FromBitmap(Bitmap bitmap)
		{
			IndexedImage image = new IndexedImage { Size = new Ravlyk.Common.Size(bitmap.Width, bitmap.Height) };
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			IntPtr ptr = bmpData.Scan0;
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				Debug.Assert(pixels != null, "Cannot lock pixels on the image.");
				System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, pixels.Length);
			}
			bitmap.UnlockBits(bmpData);
			return image;
		}
	}
}
