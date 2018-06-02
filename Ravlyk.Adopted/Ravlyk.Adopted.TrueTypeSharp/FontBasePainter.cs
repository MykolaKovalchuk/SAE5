using System;

using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.Adopted.TrueTypeSharp
{
	public abstract class FontBasePainter
	{
		protected FontBasePainter(TrueTypeFont font, int pixelHeight)
		{
			Font = font;

			PixelHeight = pixelHeight;
			Scale = font.GetScaleForPixelHeight(pixelHeight);
			SpaceWidth = pixelHeight / 2;
			TabWidth = pixelHeight * 4;
		}

		public TrueTypeFont Font { get; private set; }

		public int PixelHeight { get; private set; }

		protected float Scale { get; }
		protected int SpaceWidth { get; }
		protected int TabWidth { get; }

		#region Text direction

		public enum TextDirection
		{
			LeftToRight,
			VerticalUpward,
			VerticalDownward
		}

		protected struct CharacterBitmap
		{
			public byte[] Bitmap;
			public int Width, Height, OffsetX, OffsetY;
		}

		#endregion

		#region Paint character

		protected void PaintCharacter(CharacterBitmap characterBitmap, int[] imagePixels, Size imageSize, Point atPoint = default(Point), Rectangle clipRect = default(Rectangle), TextDirection direction = TextDirection.LeftToRight, int fontRgb = 0)
		{
			if (characterBitmap.Width <= 0 || characterBitmap.Height <= 0)
			{
				return;
			}

			if (clipRect.Width <= 0 || clipRect.Height <= 0)
			{
				clipRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
			}

			if (direction == TextDirection.LeftToRight)
			{
				PaintCharacterLeftToRight(characterBitmap, imagePixels, imageSize, atPoint, clipRect, fontRgb);
			}
			else
			{
				PaintCharacterRotated(characterBitmap, imagePixels, imageSize, atPoint, clipRect, direction, fontRgb);
			}
		}

		protected void PaintCharacterLeftToRight(CharacterBitmap characterBitmap, int[] imagePixels, Size imageSize, Point atPoint, Rectangle clipRect, int fontRgb = 0)
		{
			var imageX = atPoint.X + characterBitmap.OffsetX;
			var imageY = atPoint.Y + characterBitmap.OffsetY;

			var bitmapLeft = imageX < clipRect.Left ? clipRect.Left - imageX : 0;
			var bitmapTop = imageY < clipRect.Top ? clipRect.Top - imageY : 0;

			var localClipRect = new Rectangle(
				bitmapLeft,
				bitmapTop,
				Math.Min(clipRect.RightExclusive - imageX, characterBitmap.Width - bitmapLeft),
				Math.Min(clipRect.BottomExclusive - imageY, characterBitmap.Height - bitmapTop));

			if (localClipRect.Height > 0 && localClipRect.Width > 0)
			{
				for (int y = localClipRect.Top, bitmapRow = localClipRect.Top * characterBitmap.Width, imageRow = (localClipRect.Top + imageY) * imageSize.Width;
					y < localClipRect.BottomExclusive;
					y++, bitmapRow += characterBitmap.Width, imageRow += imageSize.Width)
				{
					for (int x = localClipRect.Left, bitmapIndex = bitmapRow + localClipRect.Left, imageIndex = imageRow + localClipRect.Left + imageX;
						x < localClipRect.RightExclusive;
						x++, bitmapIndex++, imageIndex++)
					{
						imagePixels[imageIndex] = ColorBytes.ComposeTwoColors(imagePixels[imageIndex], fontRgb, characterBitmap.Bitmap[bitmapIndex]);
					}
				}
			}
		}

		void PaintCharacterRotated(CharacterBitmap characterBitmap, int[] imagePixels, Size imageSize, Point atPoint, Rectangle clipRect, TextDirection direction, int fontRgb)
		{
			int imageX, imageY, imageStepDX, imageStepDY, imageNextRowDX, imageNextRowDY;
			switch (direction)
			{
				case TextDirection.VerticalUpward:
					imageX = atPoint.X + characterBitmap.OffsetY;
					imageY = atPoint.Y - characterBitmap.OffsetX;
					imageStepDX = 0;
					imageStepDY = -1;
					imageNextRowDX = 1;
					imageNextRowDY = 0;
					break;
				case TextDirection.VerticalDownward:
					imageX = atPoint.X - characterBitmap.OffsetY;
					imageY = atPoint.Y + characterBitmap.OffsetX;
					imageStepDX = 0;
					imageStepDY = 1;
					imageNextRowDX = -1;
					imageNextRowDY = 0;
					break;
				case TextDirection.LeftToRight:
				default:
					PaintCharacterLeftToRight(characterBitmap, imagePixels, imageSize, atPoint, clipRect, fontRgb);
					return;
			}
			var imageIndexStep = imageStepDY * imageSize.Width + imageStepDX;

			for (int y = 0, bitmapRow = 0;
				y < characterBitmap.Height;
				y++, bitmapRow += characterBitmap.Width, imageX += imageNextRowDX, imageY += imageNextRowDY)
			{
				for (int x = 0, bitmapIndex = bitmapRow, iX = imageX, iY = imageY, imageIndex = imageY * imageSize.Width + imageX;
					x < characterBitmap.Width;
					x++, bitmapIndex++, iX += imageStepDX, iY += imageStepDY, imageIndex += imageIndexStep)
				{
					if (iX >= clipRect.Left && iX < clipRect.RightExclusive && iY >= clipRect.Top && iY < clipRect.BottomExclusive)
					{
						imagePixels[imageIndex] = ColorBytes.ComposeTwoColors(imagePixels[imageIndex], fontRgb, characterBitmap.Bitmap[bitmapIndex]);
					}
				}
			}
		}

		#endregion
	}
}
