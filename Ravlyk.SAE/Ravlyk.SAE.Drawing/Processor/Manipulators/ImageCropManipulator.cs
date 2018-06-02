using System;

using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageCropManipulator : ImageManipulator
	{
		public ImageCropManipulator(CodedImage sourceImage) : base(sourceImage) { }
		public ImageCropManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

		public void CropRect(Rectangle cropRect)
		{
			ImageCropper.Crop(SourceImage, cropRect, ImageCropper.CropKind.Rectangle, ManipulatedImage);
			OnImageChanged();
		}

		public void CropArc(Rectangle cropRect)
		{
			ImageCropper.Crop(SourceImage, cropRect, ImageCropper.CropKind.Arc, ManipulatedImage);
			OnImageChanged();
		}
	}
}
