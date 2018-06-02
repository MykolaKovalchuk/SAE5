using System;
using System.Diagnostics;

using Ravlyk.Common;

namespace Ravlyk.Drawing
{
	/// <summary>
	/// Image structure with palette of colors and indexes on colors in palette from main image data.
	/// </summary>
	public class IndexedImage
	{
		#region Properties

		/// <summary>
		/// Palette of <see cref="Ravlyk.Drawing.Color"/> objects.
		/// </summary>
		public Palette Palette
		{
			get { return palette ?? (palette = NewPalette()); }
			set { palette = value; }
		}
		Palette palette;

		protected virtual Palette NewPalette()
		{
			return new Palette();
		}

		/// <summary>
		/// A size of the image.
		/// </summary>
		public Size Size
		{
			get { return size; }
			set
			{
				if (value != size)
				{
					int newLength = value.Height * value.Width;
					if (newLength != pixels.Length)
					{
						Array.Resize(ref pixels, newLength);
					}
					size = value;
				}
			}
		}
		Size size;

		/// <summary>
		/// Flat array of image's pixels.
		/// </summary>
		protected internal int[] Pixels
		{
			get { return pixels; }
			protected set { pixels = value; }
		}
		int[] pixels = new int[0];

		#endregion

		#region Accessing pixels

		/// <summary>
		/// Returns index of pixel in internal flat array by it's coordinates.
		/// </summary>
		/// <param name="x">x-coordinate of pixel.</param>
		/// <param name="y">y-coordinate of pixel.</param>
		/// <returns>Index of pixel in flat array.</returns>
		int GetPixelIndex(int x, int y)
		{
			return y * size.Width + x;
		}

		public Point GetPixelLocationFromIndex(int index)
		{
			return new Point(index % size.Width, index / size.Width);
		}

		/// <summary>
		/// Returns integer value of color in specified pixel.
		/// </summary>
		/// <param name="x">x-coordinate of pixel.</param>
		/// <param name="y">y-coordinate of pixel.</param>
		/// <returns>Integer value of pixel's color in specifie position.</returns>
		protected int GetPixel(int x, int y)
		{
			Debug.Assert((x < size.Width) && (y < size.Height), $"Wrong pixel coordinates ({x}, {y}).");

			return Pixels[GetPixelIndex(x, y)];
		}

		/// <summary>
		/// Set integer value of color for specified pixel (without changing palette.
		/// </summary>
		/// <param name="x">x-coordinate of pixel.</param>
		/// <param name="y">y-coordinate of pixel.</param>
		/// <param name="argb">32-bit (Alpha, Red, Green, Blue) color value.</param>
		protected void SetPixel(int x, int y, int argb)
		{
			Debug.Assert((x < size.Width) && (y < size.Height), $"Wrong pixel coordinates ({x}, {y}).");

			Pixels[GetPixelIndex(x, y)] = argb;
		}

		/// <summary>
		/// Indexator by image's pixels. For each image's pixel operates with <see cref="Ravlyk.Drawing.Color"/> object.
		/// </summary>
		/// <param name="x">x-coordinate of pixel.</param>
		/// <param name="y">y-coordinate of pixel.</param>
		/// <returns><see cref="Color"/> object representing color of pixel in specified position.</returns>
		/// <remarks>On changing pixel's color makes changes in palette.</remarks>
		public Color this[int x, int y]
		{
			get
			{
				Debug.Assert((x < size.Width) && (y < size.Height), $"Wrong pixel coordinates ({x}, {y}).");
				Debug.Assert(Palette.Count > 0, "There is no colors in Palette or Palette is not initialized.");

				return Palette[GetPixel(x, y)];
			}
			set
			{
				Debug.Assert((x < size.Width) && (y < size.Height), $"Wrong pixel coordinates ({x}, {y}).");

				var pixelIndex = GetPixelIndex(x, y);
				var pixel = Pixels[pixelIndex];
				var oldColor = Palette[pixel];
				if (oldColor != null)
				{
					Palette.RemoveColorOccurrence(pixel, pixelIndex);
				}
				SetPixel(x, y, value.GetHashCode());
				Palette.AddColorOccurrence(value, pixelIndex);

				OnPixelChanged(x, y, oldColor, value);
			}
		}

		/// <summary>
		/// Occurs when any pixel's color is changed.
		/// </summary>
		/// <param name="x">x-coordinate of pixel.</param>
		/// <param name="y">y-coordinate of pixel.</param>
		/// <param name="oldColor">The old <see cref="Color"/>.</param>
		/// <param name="newColor">The new <see cref="Color"/>.</param>
		protected virtual void OnPixelChanged(int x, int y, Color oldColor, Color newColor)
		{
			PixelChanged?.Invoke(this, new PixelChangedEventArgs(x, y, oldColor, newColor));
		}

		/// <summary>
		/// Occurs when any pixel's color is changed.
		/// </summary>
		public event EventHandler<PixelChangedEventArgs> PixelChanged;

		#endregion

		#region Palette and Image synchronization

		/// <summary>
		/// Completes image by updating colors palette from pixels.
		/// </summary>
		public void CompletePalette()
		{
			Palette.CompletePaletteFromImage(this);
		}

		#endregion

		#region Locking

		/// <summary>
		/// Locks image objects for bulk read/write of pixels.
		/// </summary>
		/// <param name="outPixels">Output reference to pixels array.</param>
		/// <returns>
		/// Disposable object that should be disposed when pixels are not needed any more.
		/// Returns pixels array in out parameter outPixels. Returns null in outPixels if image is already locked.
		/// </returns>
		public IDisposable LockPixels(out int[] outPixels)
		{
			Debug.Assert(!isLocked, "Image pixels are already locked.");

			if (!isLocked)
			{
				isLocked = true;
				outPixels = Pixels;
				return new DisposableAction(() => { isLocked = false; });
			}
			else
			{
				outPixels = null;
				return null;
			}
		}

		bool isLocked;

		#endregion

		#region ICloneable

		/// <summary>
		/// Creates copy of this image.
		/// </summary>
		/// <param name="withPalette">Specifies, if it is needed to clone palette colors objects.</param>
		/// <param name="doComplete">If true then image's colors palette will be updated from image's pixels.</param>
		/// <returns>New <see cref="IndexedImage"/> instance with same picture.</returns>
		public IndexedImage Clone(bool withPalette, bool doComplete = false)
		{
			var newImage = CloneCore();

			if (withPalette && palette != null)
			{
				newImage.Palette = Palette.Clone(true);
			}

			if (doComplete)
			{
				newImage.CompletePalette();
			}

			return newImage;
		}

		protected virtual IndexedImage CloneCore()
		{
			var newImage = (IndexedImage)MemberwiseClone();
			newImage.PixelChanged = null;
			newImage.palette = null;
			newImage.pixels = new int[pixels.Length];
			pixels.CopyTo(newImage.pixels, 0);
			return newImage;
		}

		#endregion
	}

	#region Events stuff

	/// <summary>
	/// PixelChanged event arguments.
	/// </summary>
	public class PixelChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelChangedEventArgs"/> class. 
		/// </summary>
		/// <param name="x">Pixel's X coordinate.</param>
		/// <param name="y">Pixel's Y coordinate.</param>
		/// <param name="oldColor">Pixel's old color.</param>
		/// <param name="newColor">Pixel's new color.</param>
		public PixelChangedEventArgs(int x, int y, Color oldColor, Color newColor)
		{
			X = x;
			Y = y;
			OldColor = oldColor;
			NewColor = newColor;
		}

		/// <summary>
		/// Pixel's X coordinate.
		/// </summary>
		public int X { get; }

		/// <summary>
		/// Pixel's Y coordinate.
		/// </summary>
		public int Y { get; }

		/// <summary>
		/// Pixel's old color.
		/// </summary>
		public Color OldColor { get; }

		/// <summary>
		/// Pixel's new color.
		/// </summary>
		public Color NewColor { get; }
	}

	#endregion
}
