using System;
using System.Collections.Generic;

using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.Adopted.TrueTypeSharp
{
	public class CachedSymbolPainter : FontBasePainter
	{
		public CachedSymbolPainter(TrueTypeFont font, int pixelHeight, int boxHeight = 0) : base(font, pixelHeight)
		{
			BoxHeight = boxHeight > 0 ? boxHeight : pixelHeight;
		}

		public int BoxHeight { get; }

		#region Paint symbol

		public void PaintSymbol(char c, IndexedImage image, Point atPoint, Rectangle clipRect = default(Rectangle), int fontRgb = 0, int backgroundRgb = 0x00ffffff)
		{
			int[] imagePixels;
			using (image.LockPixels(out imagePixels))
			{
				PaintSymbol(c, imagePixels, image.Size, atPoint, clipRect, fontRgb, backgroundRgb);
			}
		}

		public void PaintSymbol(char c, int[] imagePixels, Size imageSize, Point atPoint, Rectangle clipRect = default(Rectangle), int fontRgb = 0, int backgroundRgb = 0x00ffffff)
		{
			if (clipRect.Width <= 0 || clipRect.Height <= 0)
			{
				clipRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
			}

			var symbolBitmap = GetSymbolBitmap(c, fontRgb, backgroundRgb);

			var localClipRect = new Rectangle(
				atPoint.X < clipRect.Left ? clipRect.Left - atPoint.X : 0,
				atPoint.Y < clipRect.Top ? clipRect.Top - atPoint.Y : 0,
				Math.Min(clipRect.RightExclusive - atPoint.X, BoxHeight),
				Math.Min(clipRect.BottomExclusive - atPoint.Y, BoxHeight));
			localClipRect.Width -= localClipRect.Left;
			localClipRect.Height -= localClipRect.Top;

			if (localClipRect.Height > 0 && localClipRect.Width > 0)
			{
				for (int y = localClipRect.Top, bitmapRow = localClipRect.Top * BoxHeight, imageRow = (localClipRect.Top + atPoint.Y) * imageSize.Width;
					y < localClipRect.BottomExclusive;
					y++, bitmapRow += BoxHeight, imageRow += imageSize.Width)
				{
					Array.Copy(symbolBitmap, bitmapRow + localClipRect.Left, imagePixels, imageRow + atPoint.X + localClipRect.Left, localClipRect.Width);
				}
			}
		}

		#endregion

		#region Symbols bitmaps

		int[] GetSymbolBitmap(char c, int fontRgb, int backgroundRgb)
		{
			int[] symbolBitmap;
			var key = new SymbolKey(c, fontRgb, backgroundRgb);

			if (!symbolCache.TryGetValue(key, out symbolBitmap))
			{
				lock (symbolCache)
				{
					if (!symbolCache.TryGetValue(key, out symbolBitmap))
					{
						symbolBitmap = new int[BoxHeight * BoxHeight];
						if (backgroundRgb != 0)
						{
							for (int i = 0; i < symbolBitmap.Length; i++)
							{
								symbolBitmap[i] = backgroundRgb;
							}
						}

						if (!char.IsWhiteSpace(c))
						{
							var characterBitmap = new CharacterBitmap();
							characterBitmap.Bitmap = Font.GetCodepointBitmap(c, Scale, Scale, out characterBitmap.Width, out characterBitmap.Height, out characterBitmap.OffsetX, out characterBitmap.OffsetY);
							if (characterBitmap.Width > 0 && characterBitmap.Height > 0)
							{
								characterBitmap.OffsetX = (BoxHeight - characterBitmap.Width) / 2;
								characterBitmap.OffsetY = (BoxHeight - characterBitmap.Height) / 2;

								PaintCharacterLeftToRight(characterBitmap, symbolBitmap, new Size(BoxHeight, BoxHeight), new Point(0, 0), new Rectangle(0, 0, BoxHeight, BoxHeight), fontRgb);
							}
						}

						symbolCache.Add(key, symbolBitmap);
					}
				}
			}

			return symbolBitmap;
		}

		readonly Dictionary<SymbolKey, int[]> symbolCache = new Dictionary<SymbolKey, int[]>();

		class SymbolKey
		{
			public SymbolKey(char c, int fontRgb, int backgroundRgb)
			{
				this.c = c;
				this.fontRgb = fontRgb;
				this.backgroundRgb = backgroundRgb;
			}

			readonly char c;
			readonly int fontRgb, backgroundRgb;

			public override int GetHashCode()
			{
				return fontRgb ^ backgroundRgb ^ c;
			}

			public override bool Equals(object obj)
			{
				var otherKey = obj as SymbolKey;
				return otherKey != null
					&& otherKey.c == c
					&& otherKey.fontRgb == fontRgb
					&& otherKey.backgroundRgb == backgroundRgb;
			}
		}

		#endregion
	}
}
