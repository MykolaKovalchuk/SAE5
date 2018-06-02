using System.Collections;
using System.Threading.Tasks;

using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor
{
	/// <summary>
	/// Rotates and flips <see cref="IndexedImage"/> object.
	/// </summary>
	/// <remarks>All operations are inplace.</remarks>
	public static class ImageRotator
	{
		#region Rotate clockwise

		/// <summary>
		/// Rotates image clockwise.
		/// </summary>
		/// <param name="image">Image for rotating.</param>
		/// <returns>The same input <see cref="IndexedImage"/> instance.</returns>
		public static IndexedImage RotateCWInPlace(IndexedImage image)
		{
			var oldWidth = image.Size.Width;
			var newWidth = image.Size.Height;
			var newHeight = image.Size.Width;

			var pixels = image.Pixels;
			var updated = new BitArray(pixels.Length);

			for (int i = 0; i < pixels.Length; i++)
			{
				if (!updated[i])
				{
					int newIndex = i;
					int newY = newIndex / newWidth;
					int newX = newIndex % newWidth;
					int oldY = newWidth - newX - 1;
					int oldX = newY;

					int oldIndex = oldY * oldWidth + oldX;
					int oldPixel = pixels[oldIndex];

					while (!updated[newIndex])
					{
						int newPixel = pixels[newIndex];
						pixels[newIndex] = oldPixel;
						oldPixel = newPixel;
						updated[newIndex] = true;

						oldIndex = newIndex;
						oldY = oldIndex / oldWidth;
						oldX = oldIndex % oldWidth;

						newY = oldX;
						newX = newWidth - oldY - 1;
						newIndex = newY * newWidth + newX;
					}
				}
			}

			image.Size = new Size(newWidth, newHeight);

			return image;
		}

		#endregion Rotate clockwise

		#region Rotate counter-clockwise

		/// <summary>
		/// Rotates image counter-clockwise.
		/// </summary>
		/// <param name="image">Image for rotating.</param>
		/// <returns>The same input <see cref="IndexedImage"/> instance.</returns>
		public static IndexedImage RotateCCWInPlace(IndexedImage image)
		{
			var oldWidth = image.Size.Width;
			var newWidth = image.Size.Height;
			var newHeight = image.Size.Width;

			var pixels = image.Pixels;
			var updated = new BitArray(pixels.Length);

			for (int i = 0; i < pixels.Length; i++)
			{
				if (!updated[i])
				{
					int newIndex = i;
					int newY = newIndex / newWidth;
					int newX = newIndex % newWidth;
					int oldY = newX;
					int oldX = newHeight - newY - 1;

					int oldIndex = oldY * oldWidth + oldX;
					int oldPixel = pixels[oldIndex];

					while (!updated[newIndex])
					{
						int newPixel = pixels[newIndex];
						pixels[newIndex] = oldPixel;
						oldPixel = newPixel;
						updated[newIndex] = true;

						oldIndex = newIndex;
						oldY = oldIndex / oldWidth;
						oldX = oldIndex % oldWidth;

						newY = newHeight - oldX - 1;
						newX = oldY;
						newIndex = newY * newWidth + newX;
					}
				}
			}

			image.Size = new Size(newWidth, newHeight);

			return image;
		}

		#endregion Rotate counter-clockwise

		#region Flip

		/// <summary>
		/// Flips image horizontally.
		/// </summary>
		/// <param name="image">Image for flipping.</param>
		/// <returns>The same input <see cref="IndexedImage"/> instance.</returns>
		public static IndexedImage FlipHorizontallyInPlace(IndexedImage image)
		{
			var pixels = image.Pixels;

			Parallel.For(0, image.Size.Height,
				y =>
				{
					for (int i1 = y * image.Size.Width, i2 = i1 + image.Size.Width - 1; i1 < i2; i1++, i2--)
					{
						int tempPixel = pixels[i2];
						pixels[i2] = pixels[i1];
						pixels[i1] = tempPixel;
					}
				});

			return image;
		}

		/// <summary>
		/// Flips image vertically.
		/// </summary>
		/// <param name="image">Image for flipping.</param>
		/// <returns>The same input <see cref="IndexedImage"/> instance.</returns>
		public static IndexedImage FlipVerticallyInPlace(IndexedImage image)
		{
			var pixels = image.Pixels;

			Parallel.For(0, image.Size.Width,
				x =>
				{
					for (int i1 = x, i2 = pixels.Length - image.Size.Width + i1; i1 < i2; i1 += image.Size.Width, i2 -= image.Size.Width)
					{
						int tempPixel = pixels[i2];
						pixels[i2] = pixels[i1];
						pixels[i1] = tempPixel;
					}
				});

			return image;
		}

		#endregion Flip
	}
}
