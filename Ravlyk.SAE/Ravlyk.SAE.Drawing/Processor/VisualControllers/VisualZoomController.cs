using System;

using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class VisualZoomController : VisualBoxedController
	{
		public VisualZoomController(IImageProvider imageProvider, Size imageBoxSize = default(Size)) : base(imageProvider, imageBoxSize)
		{
			FitImage();
		}

		public ImageResampler.FilterType UpscaleFilterType { get; set; } = ImageResampler.FilterType.Box;

		public ImageResampler.FilterType DownscaleFilterType { get; set; } = ImageResampler.FilterType.Lanczos3;

		#region Zooming

		public void FitImage()
		{
			using (SuspendUpdateVisualImage())
			{
				if (SourceImage.Size.Width > ImageBoxSize.Width || SourceImage.Size.Height > ImageBoxSize.Height)
				{
					var widthZoom = SourceImage.Size.Width != 0 ? ImageBoxSize.Width * 100 / SourceImage.Size.Width : 100;
					var heightZoom = SourceImage.Size.Height != 0 ? ImageBoxSize.Height * 100 / SourceImage.Size.Height : 100;
					ZoomPercent = Math.Min(widthZoom, heightZoom);
				}
				else
				{
					ZoomPercent = 100;
				}
			}
		}

		const int MinimumZoom = 1; // 1:100
		const int MaximumZoom = 1600; // x16

		public int ZoomPercent
		{
			get { return zoomPercent; }
			set
			{
				if (value < MinimumZoom)
				{
					value = MinimumZoom;
				}

				value = NormalizeZoom(value, SourceImage.Size.Width);
				value = NormalizeZoom(value, SourceImage.Size.Height);

				if (value > MaximumZoom)
				{
					value = MaximumZoom;
				}

				if (value == zoomPercent)
				{
					return;
				}

				zoomPercent = value;
				UpdateParametersAndVisualImage();
			}
		}
		int zoomPercent = 100;

		static int NormalizeZoom(int value, int sourceWidth)
		{
			if (value < 100 && (sourceWidth * value / 100) < MinimumWidth)
			{
				value = MinimumWidth * 100 / sourceWidth;
				if (value > 100)
				{
					value = 100;
				}
			}

			return value;
		}

		#endregion

		#region Visual image frame

		public Size ZoomedImageSize => new Size(SourceImage.Size.Width * ZoomPercent / 100, SourceImage.Size.Height * ZoomPercent / 100);

		public Rectangle VisualImageFrame { get; private set; }

		public Point ImageLocation { get; private set; }

		#endregion

		#region Update visual image

		protected override void UpdateParameters()
		{
			if (ZoomedImageSize.Width <= ImageBoxSize.Width && ZoomedImageSize.Height <= ImageBoxSize.Height)
			{
				VisualImageFrame = new Rectangle(0, 0, ZoomedImageSize.Width, ZoomedImageSize.Height);
			}
			else
			{
				VisualImageFrame = new Rectangle(
					VisualImageFrame.Left,
					VisualImageFrame.Top,
					Math.Min(ImageBoxSize.Width, ZoomedImageSize.Width),
					Math.Min(ImageBoxSize.Height, ZoomedImageSize.Height));

				VisualImageFrame = new Rectangle(
					Math.Max(Math.Min(VisualImageFrame.Left, ZoomedImageSize.Width - VisualImageFrame.Width), 0),
					Math.Max(Math.Min(VisualImageFrame.Top, ZoomedImageSize.Height - VisualImageFrame.Height), 0),
					VisualImageFrame.Width,
					VisualImageFrame.Height);
			}

			ImageLocation = new Point(
				Math.Max((ImageBoxSize.Width - VisualImageFrame.Width) / 2, 0),
				Math.Max((ImageBoxSize.Height - VisualImageFrame.Height) / 2, 0));
		}

		protected override void UpdateVisualImageCore()
		{
			if (ZoomPercent == 100)
			{
				if (VisualImageFrame.Width == SourceImage.Size.Width && VisualImageFrame.Height == SourceImage.Size.Height)
				{
					VisualImage.Size = SourceImage.Size;
					SourceImage.Pixels.CopyTo(VisualImage.Pixels, 0);
				}
				else
				{
					ImageCropper.Crop(SourceImage, VisualImageFrame, ImageCropper.CropKind.Rectangle, VisualImage);
				}
			}
			else
			{
				new ImageResampler().Resample(SourceImage, ZoomedImageSize, ZoomPercent > 100 ? UpscaleFilterType : DownscaleFilterType, VisualImageFrame, VisualImage);
			}
		}

		#endregion

		#region Touch actions

		protected override Point TranslatePoint(Point controllerPoint)
		{
			return new Point(
				controllerPoint.X - ImageLocation.X + VisualImageFrame.Left,
				controllerPoint.Y - ImageLocation.Y + VisualImageFrame.Top);
		}

		protected override TouchPointerStyle GetTouchPointerStyleCore(Point imagePoint)
		{
			if (imagePoint.X >= 0 && imagePoint.X < ZoomedImageSize.Width && imagePoint.Y >= 0 && imagePoint.Y < ZoomedImageSize.Height &&
				(ZoomedImageSize.Width > ImageBoxSize.Width || ZoomedImageSize.Height > ImageBoxSize.Height))
			{
				return TouchPointerStyle.Shift;
			}

			return base.GetTouchPointerStyleCore(imagePoint);
		}

		protected override void OnShiftCore(Point imagePoint, Size shiftSize)
		{
			if (IsTouching && GetTouchPointerStyleCore(new Point(imagePoint.X, imagePoint.Y)) == TouchPointerStyle.Shift)
			{
				VisualImageFrame = new Rectangle(
					Math.Max(Math.Min(VisualImageFrame.Left - shiftSize.Width, ZoomedImageSize.Width - VisualImageFrame.Width), 0),
					Math.Max(Math.Min(VisualImageFrame.Top - shiftSize.Height, ZoomedImageSize.Height - VisualImageFrame.Height), 0),
					VisualImageFrame.Width,
					VisualImageFrame.Height);

				UpdateVisualImage();
			}
			else
			{
				base.OnShiftCore(imagePoint, shiftSize);
			}
		}

		#endregion
	}
}
