using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.Adopted.TrueTypeSharp
{
	public class TextPainter : FontBasePainter
	{
		public TextPainter(TrueTypeFont font, int pixelHeight) : base(font, pixelHeight) { }

		#region Paint text

		struct LocatedCharacterBitmap
		{
			public CharacterBitmap Bitmap;
			public Point Location;
		}

		public Size GetTextSize(string text, int spaceBetweenCharacters = 1)
		{
			var width = spaceBetweenCharacters;
			var height = 0;

			foreach (var c in text)
			{
				var characterBitmap = GetCharacterBitmap(c);
				width += characterBitmap.Width + spaceBetweenCharacters;
				height = Math.Max(height, characterBitmap.OffsetY + characterBitmap.Height);
			}

			return new Size(width, height);
		}

		public void PaintText(string text, IndexedImage image, Point startPoint, Rectangle clipRect = default(Rectangle), int spaceBetweenCharacters = 1, int fontRgb = 0, TextDirection direction = TextDirection.LeftToRight, bool multithread = false)
		{
			int[] imagePixels;
			using (image.LockPixels(out imagePixels))
			{
				PaintText(text, imagePixels, image.Size, startPoint, clipRect, spaceBetweenCharacters, fontRgb, direction, multithread);
			}
		}

		public void PaintText(string text, int[] imagePixels, Size imageSize, Point startPoint, Rectangle clipRect = default(Rectangle), int spaceBetweenCharacters = 1, int fontRgb = 0, TextDirection direction = TextDirection.LeftToRight, bool multithread = false)
		{
			var locatedCharBitmaps = new List<LocatedCharacterBitmap>(text.Length);
			var currentPoint = AdvancePosition(startPoint, spaceBetweenCharacters, direction);
			foreach (var c in text)
			{
				var locatedCharBitmap = new LocatedCharacterBitmap { Bitmap = GetCharacterBitmap(c), Location = currentPoint };
				locatedCharBitmaps.Add(locatedCharBitmap);

				currentPoint = AdvancePosition(currentPoint, locatedCharBitmap.Bitmap.Width + spaceBetweenCharacters, direction);
			}

			if (multithread)
			{
				Parallel.ForEach(locatedCharBitmaps, locatedCharBitmap => PaintCharacter(locatedCharBitmap.Bitmap, imagePixels, imageSize, locatedCharBitmap.Location, clipRect, direction, fontRgb));
			}
			else
			{
				locatedCharBitmaps.ForEach(locatedCharBitmap => PaintCharacter(locatedCharBitmap.Bitmap, imagePixels, imageSize, locatedCharBitmap.Location, clipRect, direction, fontRgb));
			}
		}

		static Point AdvancePosition(Point startPoint, int distance, TextDirection direction)
		{
			switch (direction)
			{
				case TextDirection.VerticalUpward:
					return new Point(startPoint.X, startPoint.Y - distance);
				case TextDirection.VerticalDownward:
					return new Point(startPoint.X, startPoint.Y + distance);
				case TextDirection.LeftToRight:
				default:
					return new Point(startPoint.X + distance, startPoint.Y);
			}
		}

		#endregion

		#region Character bitmaps

		CharacterBitmap GetCharacterBitmap(char c)
		{
			CharacterBitmap bitmap;

			if (!bitmapCache.TryGetValue(c, out bitmap))
			{
				lock (bitmapCache)
				{
					if (!bitmapCache.TryGetValue(c, out bitmap))
					{
						if (char.IsWhiteSpace(c))
						{
							bitmap = new CharacterBitmap { Bitmap = null, Width = c == '\t' ? TabWidth : SpaceWidth, Height = 0, OffsetX = 0, OffsetY = 0 };
						}
						else
						{
							bitmap = new CharacterBitmap();
							bitmap.Bitmap = Font.GetCodepointBitmap(c, Scale, Scale, out bitmap.Width, out bitmap.Height, out bitmap.OffsetX, out bitmap.OffsetY);
							bitmap.OffsetX = 0; // Do simple paint without overlapping or extra space
							bitmap.OffsetY += PixelHeight; // Counts from bottom with negative shift up
							if (bitmap.Width < 0) { bitmap.Width = 0; } // To prevent overlapping previous characters
						}

						bitmapCache.Add(c, bitmap);
					}
				}
			}
			return bitmap;
		}

		readonly Dictionary<char, CharacterBitmap> bitmapCache = new Dictionary<char, CharacterBitmap>();

		#endregion
	}
}
