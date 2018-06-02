using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Grid
{
	class CrossPainter
	{
		public CrossPainter(int pixelHeight, IndexedImage whiteCrosses, Palette allColors)
		{
			Debug.Assert(whiteCrosses != null, "whiteCrosses should not be null");

			PixelHeight = pixelHeight;
			WhiteCrosses = new ImageResampler().Resample(whiteCrosses, new Size(CrossesPerWhiteImageGridSize * pixelHeight, CrossesPerWhiteImageGridSize * pixelHeight), ImageResampler.FilterType.Lanczos3);

			InitializeCrossesForAllColors(allColors);
		}

		public void InitializeCrossesForAllColors(Palette allColors)
		{
			Parallel.ForEach(allColors, color => GetCross(color.Argb));
		}

		public int PixelHeight { get; }

		public void PaintCross(int color, IndexedImage image, Size imageSize, Point atPoint, Rectangle clipRect = default(Rectangle))
		{
			ImageCopier.Copy(GetCross(color), image, atPoint);
		}

		#region Crosses

		internal IndexedImage GetCross(int color)
		{
			var index = random.Next(CrossesPerWhiteImage);
			var row = index / CrossesPerWhiteImageGridSize;
			var col = index % CrossesPerWhiteImageGridSize;
			var crossImage = ImageCropper.Crop(GetCrossesImage(color), new Rectangle(col * PixelHeight, row * PixelHeight, PixelHeight, PixelHeight));
			return crossImage;
		}

		IndexedImage GetCrossesImage(int color)
		{
			IndexedImage image;
			if (!crosses.TryGetValue(color, out image))
			{
				image = WhiteCrosses.Clone(false);
				ImagePainter.ShadeImage(image, GetShadeColor(color));

				lock (crossesMutex)
				{
					crosses.Add(color, image);
				}
			}
			return image;
		}

		int GetShadeColor(int color)
		{
			const int coeff = 32;

			var r = AdjustComponent(color.Red(), coeff);
			var g = AdjustComponent(color.Green(), coeff);
			var b = AdjustComponent(color.Blue(), coeff);

			return ColorBytes.ToArgb(color.Alpha(), r, g, b);
		}

		byte AdjustComponent(int component, int coeff)
		{
			const int maxValue = byte.MaxValue;
			var remainder = maxValue - coeff;

			var newValue = component * remainder / maxValue + coeff;
			if (newValue > maxValue)
			{
				newValue = maxValue;
			}

			return (byte)newValue;
		}

		readonly Dictionary<int, IndexedImage> crosses = new Dictionary<int, IndexedImage>();
		readonly object crossesMutex = new object();
		readonly Random random = new Random(Environment.TickCount);
		const int CrossesPerWhiteImageGridSize = 4;
		const int CrossesPerWhiteImage = CrossesPerWhiteImageGridSize * CrossesPerWhiteImageGridSize;

		IndexedImage WhiteCrosses { get; }

		#endregion
	}
}
