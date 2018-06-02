using System;
using System.Diagnostics;

using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageColorsManipulator : ImageManipulator
	{
		public ImageColorsManipulator(CodedImage sourceImage) : base(sourceImage) { }
		public ImageColorsManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

		public void QuantizeColors(int colorsCount, CodedPalette destPalette = null, int ditherLevel = 0, bool ensureSpecialColors = false)
		{
			Debug.Assert(colorsCount >= 2, "colorsCount should be greater than 1.");

			lastColorsCount = colorsCount;
			lastDestPalette = destPalette;
			lastDitherLevel = ditherLevel;
			lastEnsureSpecialColors = ensureSpecialColors;

			if (IsManipulatedImageInitialized)
			{
				new NeuQuant().QuantizeColors(SourceImage, colorsCount, destPalette, 1, ditherLevel, true, ManipulatedImage, ensureSpecialColors);
				OnImageChanged();
			}
		}

		int lastColorsCount;
		CodedPalette lastDestPalette;
		int lastDitherLevel;
		bool lastEnsureSpecialColors;

		protected override void RestoreManipulationsCore()
		{
			if (lastColorsCount >= 2)
			{
				QuantizeColors(lastColorsCount, lastDestPalette, lastDitherLevel, lastEnsureSpecialColors);
			}
		}
	}
}
