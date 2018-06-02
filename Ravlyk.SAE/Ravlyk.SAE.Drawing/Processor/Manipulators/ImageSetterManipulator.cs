using System;
using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSetterManipulator : ImageInplaceManipulator
	{
		public ImageSetterManipulator() : base(new CodedImage { Size = new Size(1, 1) })
		{
			SourceImage.Pixels[0] = ColorBytes.ToArgb(0, 255, 255, 255);
			SourceImage.CompletePalette();
		}

		public void SetNewImage(CodedImage newImage)
		{
			ManipulatedImage.Size = newImage.Size;
			int[] sourcePixels, manipulatedPixels;
			using (newImage.LockPixels(out sourcePixels))
			using (ManipulatedImage.LockPixels(out manipulatedPixels))
			{
				sourcePixels.CopyTo(manipulatedPixels, 0);
			}
			ManipulatedImage.SourceImageFileName = newImage.SourceImageFileName;
			OnImageChanged();
		}

		protected override void CopySourceImageCore()
		{
			// Do nothing - source and manipulated images are same instance
		}
	}
}

