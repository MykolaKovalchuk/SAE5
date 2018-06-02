using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Ravlyk.Common;

namespace Ravlyk.Drawing
{
	/// <summary>
	/// A set of <see cref="Ravlyk.Drawing.Color"/> objects.
	/// </summary>
	public class Palette : IEnumerable<Color>
	{
		#region Colors

		IDictionary<int, Color> colors = new Dictionary<int, Color>();

		/// <summary>
		/// Count of colors in palette.
		/// </summary>
		public int Count => colors.Count;

		/// <summary>
		/// Find and return <see cref="Ravlyk.Drawing.Color"/> object in palette by it's hash code if exists.
		/// </summary>
		/// <returns>Returns instance <see cref="Ravlyk.Drawing.Color"/> for specified hash.</returns>
		/// <param name="colorHash">The hash code of <see cref="Ravlyk.Drawing.Color"/> object.</param>
		public Color this[int colorHash]
		{
			get
			{
				Color color;
				return colors.TryGetValue(colorHash, out color) ? color : null;
			}
		}

		/// <summary>
		/// Add if new a <see cref="Ravlyk.Drawing.Color"/> object to palette.
		/// </summary>
		/// <param name="color"><see cref="Ravlyk.Drawing.Color"/> object to add to palette.</param>
		/// <remarks>Adds a copy of color object.</remarks>
		/// <returns>Returns a new instance of color added to palette or instance of color already in the palette.</returns>
		public Color Add(Color color)
		{
			int colorHash = color.GetHashCode();
			if (!colors.ContainsKey(colorHash)) // If it is new color object
			{
				AddColorCore(colorHash, NewColor(color));
			}
			return this[colorHash];
		}

		/// <summary>
		/// Add if new a <see cref="Ravlyk.Drawing.Color"/> object to palette.
		/// </summary>
		/// <param name="colorHash">Color's hash code.</param>
		/// <remarks>Adds a copy of color object.</remarks>
		/// <returns>Returns a new instance of color added to palette or instance of color already in the palette.</returns>
		public Color Add(int colorHash)
		{
			if (!colors.ContainsKey(colorHash)) // If it is new color object
			{
				AddColorCore(colorHash, NewColor(colorHash));
			}
			return this[colorHash];
		}

		void AddColorCore(int colorHash, Color newColor)
		{
			colors.Add(colorHash, newColor);
			OnColorAdded(newColor);
		}

		/// <summary>
		/// Removes color from palette.
		/// </summary>
		/// <param name="colorHash">The hash code of <see cref="Ravlyk.Drawing.Color"/> object.</param>
		public void Remove(int colorHash)
		{
			Color color;
			if (colors.TryGetValue(colorHash, out color))
			{
				colors.Remove(colorHash);
				OnColorRemoved(color);
			}
		}

		/// <summary>
		/// Remove all colors from palette.
		/// </summary>
		public void Clear()
		{
			colors.Clear();
		}

		#endregion

		#region Color usage occurrences

		/// <summary>
		/// Adds new color usage occurrence.
		/// </summary>
		/// <param name="colorHash">Color's hash code.</param>
		/// <param name="usageIndex">Index of pixel with color usage.</param>
		internal void AddColorOccurrence(int colorHash, int usageIndex)
		{
			var paletteColor = Add(colorHash);
			paletteColor.AddOccurrence(usageIndex);
			OnOccurrencesChanged(paletteColor);
		}

		/// <summary>
		/// Adds new color usage occurrence.
		/// </summary>
		/// <param name="color">Color object.</param>
		/// <param name="usageIndex">Index of pixel with color usage.</param>
		internal void AddColorOccurrence(Color color, int usageIndex)
		{
			var paletteColor = Add(color);
			paletteColor.AddOccurrence(usageIndex);
			OnOccurrencesChanged(paletteColor);
		}

		/// <summary>
		/// Removes color usage occurrence.
		/// </summary>
		/// <param name="colorHash">Color's hash code.</param>
		/// <param name="usageIndex">Index of pixel with color usage.</param>
		/// <remarks>If color is not used any more - it will be deleted from palette.</remarks>
		internal void RemoveColorOccurrence(int colorHash, int usageIndex)
		{
			Color color;
			if (colors.TryGetValue(colorHash, out color) && color.RemoveOccurrence(usageIndex) == 0 && !removeColorsWithoutOccurrencesLock.IsLocked())
			{
				Remove(colorHash);
			}
			if (color != null)
			{
				OnOccurrencesChanged(color);
			}
		}

		/// <summary>
		/// Suppresses removing unused colors from palette.
		/// </summary>
		/// <returns><see cref="IDisposable"/> object, which should be disposed to restore removing unused colors.</returns>
		public IDisposable SuppressRemoveColorsWithoutOccurrences()
		{
			return DisposableLock.Lock(ref removeColorsWithoutOccurrencesLock);
		}
		DisposableLock removeColorsWithoutOccurrencesLock;

		#endregion

		#region Palette and Image synchronization

		/// <summary>
		/// Prepare palette by pixels' colors.
		/// </summary>
		/// <param name="image"><see cref="IndexedImage"/> object with real pixels.</param>
		internal void CompletePaletteFromImage(IndexedImage image)
		{
			Debug.Assert(image.Pixels.Length > 0, "Image does not have any pixels.");

			var newColors = Count == 0;
			var pixelColors = image.Pixels;

			foreach (var c in this)
			{
				c.ClearOccurrences();
			}

			for (int index = 0; index < pixelColors.Length; index++)
			{
				AddColorOccurrence(pixelColors[index], index);
			}

			// Remove unused colors
			if (!newColors)
			{
				foreach (var colorHash in colors.Values.Where(c => c.OccurrencesCount == 0).Select(c => c.GetHashCode()).ToArray())
				{
					Remove(colorHash);
				}
			}
		}

		#endregion

		#region ICloneable

		public Palette Clone(bool forImage = false)
		{
			return CloneCore(forImage);
		}

		protected virtual Palette CloneCore(bool forImage)
		{
			var newPalette = (Palette)MemberwiseClone();
			newPalette.ColorAdded = null;
			newPalette.ColorRemoved = null;
			newPalette.OccurrencesChanged = null;
			newPalette.colors = new Dictionary<int, Color>(Count);
			foreach (var color in this)
			{
				newPalette.Add(NewColor(color));
				if (forImage)
				{
					var colorInPalette = newPalette[color.GetHashCode()];
					colorInPalette.ClearOccurrences();
					colorInPalette.UsageOccurrences.AddRange(color.UsageOccurrences);
				}
			}
			return newPalette;
		}

		#endregion

		#region Virtual methods

		protected virtual Color NewColor(int argb)
		{
			return new Color(argb);
		}

		protected virtual Color NewColor(Color color)
		{
			return color.Clone();
		}

		#endregion

		#region Events

		void OnColorAdded(Color color)
		{
			ColorAdded?.Invoke(this, new ColorChangedEventArgs(color));
		}

		void OnColorRemoved(Color color)
		{
			ColorRemoved?.Invoke(this, new ColorChangedEventArgs(color));
		}

		void OnOccurrencesChanged(Color color)
		{
			OccurrencesChanged?.Invoke(this, new ColorChangedEventArgs(color));
		}

		/// <summary>
		/// Occurs when a new color is added to a palette.
		/// </summary>
		public event EventHandler<ColorChangedEventArgs> ColorAdded;

		/// <summary>
		/// Occurs when any color is removed from a palette.
		/// </summary>
		public event EventHandler<ColorChangedEventArgs> ColorRemoved;

		/// <summary>
		/// Occurs when any color occurrence is chaged.
		/// </summary>
		public event EventHandler<ColorChangedEventArgs> OccurrencesChanged;

		#region Events arguments

		/// <summary>
		/// Color changed event arguments.
		/// </summary>
		public class ColorChangedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class. 
			/// </summary>
			/// <param name="color">
			/// Changed color.
			/// </param>
			public ColorChangedEventArgs(Color color)
			{
				Color = color;
			}

			/// <summary>
			/// Changed color.
			/// </summary>
			public Color Color { get; private set; }
		}

		#endregion

		#endregion

		#region IEnumerable<Color>

		IEnumerator<Color> IEnumerable<Color>.GetEnumerator()
		{
			return GetColorsCore().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetColorsCore().GetEnumerator();
		}

		protected IEnumerable<Color> GetColorsCore()
		{
			return colors.Values;
		}

		#endregion
	}
}
