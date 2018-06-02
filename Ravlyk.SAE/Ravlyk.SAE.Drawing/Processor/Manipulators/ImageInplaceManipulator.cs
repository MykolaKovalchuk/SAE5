using System;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class ImageInplaceManipulator : ImageManipulator
	{
		protected ImageInplaceManipulator(CodedImage sourceImage) : base(sourceImage) { }
		protected ImageInplaceManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

		protected override CodedImage CreateManipulatedImage(CodedImage sourceImage)
		{
			return sourceImage;
		}
	}
}
