using System;
using System.Diagnostics;

using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSizeManipulator : ImageManipulator
	{
		public enum SizeLockType
		{
			None,
			KeepWidthAndHeight,
			KeepWidthScaleHeight,
			ScaleWidthKeepHeight
		}

		public ImageSizeManipulator(CodedImage sourceImage) : base(sourceImage) { }
		public ImageSizeManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

		public void Resize(Size newSize, ImageResampler.FilterType filter, SizeLockType sizeLockBy)
		{
			Debug.Assert(newSize.Width > 0 && newSize.Height > 0, "newSize should not have zero values.");

			lastSize = newSize;
			lastFilter = filter;
			lastSizeLockBy = sizeLockBy;

			if (IsManipulatedImageInitialized)
			{
				new ImageResampler().Resample(SourceImage, newSize, filter, destImage: ManipulatedImage);
				OnImageChanged();
			}
		}

		SizeLockType lastSizeLockBy = SizeLockType.None;
		Size lastSize;
		ImageResampler.FilterType lastFilter;

		protected override void RestoreManipulationsCore()
		{
			if (lastSizeLockBy != SizeLockType.None && lastSize.Width > 0 && lastSize.Height > 0)
			{
				Size newSize = lastSize;
				if (lastSizeLockBy == SizeLockType.KeepWidthScaleHeight)
				{
					newSize.Height = SourceImage.Size.Height * lastSize.Width / SourceImage.Size.Width;
					if (newSize.Height == 0)
					{
						newSize.Height = 1;
					}
				}
				else if (lastSizeLockBy == SizeLockType.ScaleWidthKeepHeight)
				{
					newSize.Width = SourceImage.Size.Width * lastSize.Height / SourceImage.Size.Height;
					if (newSize.Width == 0)
					{
						newSize.Width = 1;
					}
				}
				Resize(newSize, lastFilter, lastSizeLockBy);
			}
		}

		protected override void ResetCore()
		{
			base.ResetCore();
			lastSize = ManipulatedImage.Size;
		}
	}
}
