using System;

using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class VisualBoxedController : VisualController
	{
		protected VisualBoxedController(IImageProvider imageProvider, Size imageBoxSize) : base(imageProvider)
		{
			ImageBoxSize = imageBoxSize.Width > 0 && imageBoxSize.Height > 0 ? imageBoxSize : new Size(16, 16);
		}

		public Size ImageBoxSize
		{
			get { return imageBoxSize; }
			set
			{
				if (value.Width < MinimumWidth)
				{
					value.Width = MinimumWidth;
				}
				if (value.Height < MinimumWidth)
				{
					value.Height = MinimumWidth;
				}

				if (value == imageBoxSize)
				{
					return;
				}

				imageBoxSize = value;
				UpdateParametersAndVisualImage();
			}
		}
		Size imageBoxSize;

		public const int MinimumWidth = 16;

		public void UpdateParametersAndVisualImage()
		{
			using (SuspendUpdateVisualImage())
			{
				UpdateParameters();
				UpdateVisualImage();
			}
		}

		protected virtual void UpdateParameters() { }

		protected override void OnSourceImageChanged()
		{
			UpdateParametersAndVisualImage();
		}
	}
}
