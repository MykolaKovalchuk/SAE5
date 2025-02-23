using System;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Common;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSizeController : ImageController<ImageSizeManipulator>
	{
		public enum SizeUnit
		{
			Stitch,
			Cm,
			Inch
		}

		public ImageSizeController(ImageSizeManipulator manipulator) : base(manipulator)
		{
			SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight;

			height = manipulator.SourceImage.Size.Height * width / manipulator.SourceImage.Size.Width;
			if (height == 0) { height = 1; }

			using (SuspendCallManipulations())
			{
				RestoreDefaults();
				CallManipulations();
			}
		}

		public bool KeepAspect
		{
			get { return keepAspect; }
			set
			{
				if (value == keepAspect)
				{
					return;
				}

				keepAspect = value;
				if (keepAspect)
				{
					if (SizeLockBy == ImageSizeManipulator.SizeLockType.ScaleWidthKeepHeight)
					{
						var x = height;
						height = 0;
						Height = x;
					}
					else
					{
						var x = width;
						width = 0;
						Width = x;
					}
				}
			}
		}
		bool keepAspect = true;

		internal ImageSizeManipulator.SizeLockType SizeLockBy { get; set; }

		public ImageResampler.FilterType FilterType
		{
			get { return filterType; }
			set
			{
				if (filterType == value)
				{
					return;
				}

				filterType = value;

				CallManipulations();

				OnPropertyChanged(nameof(FilterType));
			}
		}
		ImageResampler.FilterType filterType = ImageResampler.FilterType.Lanczos3;

		public int Width
		{
			get { return width; }
			set
			{
				if (value < MinimumSize) { value = MinimumSize; }
				if (value > MaximumSize) { value = MaximumSize; }
				if (width == value)
				{
					return;
				}

				width = value;

				if (KeepAspect)
				{
					height = Manipulator.SourceImage.Size.Height * width / Manipulator.SourceImage.Size.Width;
					if (height == 0)
					{
						height = 1;
					}

					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight;
				}
				else
				{
					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthAndHeight;
				}

				CallManipulations();

				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(Height));
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));
			}
		}
		int width = 200;

		public int Height
		{
			get { return height; }
			set
			{
				if (value < MinimumSize) { value = MinimumSize; }
				if (value > MaximumSize) { value = MaximumSize; }
				if (height == value)
				{
					return;
				}

				height = value;

				if (KeepAspect)
				{
					width = Manipulator.SourceImage.Size.Width * height / Manipulator.SourceImage.Size.Height;
					if (width == 0)
					{
						width = 1;
					}

					SizeLockBy = ImageSizeManipulator.SizeLockType.ScaleWidthKeepHeight;
				}
				else
				{
					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthAndHeight;
				}

				CallManipulations();

				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(Height));
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));
			}
		}
		int height = 150;

		public const int MinimumSize = 20;
		public const int MaximumSize = 1000;

		public decimal SchemeWidth
		{
			get { return Width / StitchesPerUnitWidth; }
			set { Width = (int)(value * StitchesPerUnitWidth); }
		}

		public decimal SchemeHeight
		{
			get { return Height / StitchesPerUnitHeight; }
			set { Height = (int)(value * StitchesPerUnitHeight); }
		}

		public SizeUnit Unit
		{
			get { return unit; }
			set
			{
				if (value != unit)
				{
					if (unitForStitchesPerUnit == SizeUnit.Stitch)
					{
						StitchesPerUnitHeight = value == SizeUnit.Inch ? 8.0m : 3.15m;
						StitchesPerUnitWidth = value == SizeUnit.Inch ? 8.0m : 3.15m;
					}

					if (value == SizeUnit.Cm && unitForStitchesPerUnit == SizeUnit.Inch)
					{
						stitchesPerUnitHeight = stitchesPerUnitHeight * 10m / 25.4m;
						stitchesPerUnitWidth = stitchesPerUnitWidth * 10m / 25.4m;
					}
					else if (value == SizeUnit.Inch && unitForStitchesPerUnit == SizeUnit.Cm)
					{
						stitchesPerUnitWidth = stitchesPerUnitWidth * 25.4m / 10m;
						stitchesPerUnitHeight = stitchesPerUnitHeight * 25.4m / 10m;
					}

					unit = value;
					if (unit != SizeUnit.Stitch)
					{
						unitForStitchesPerUnit = unit;
					}

					OnPropertyChanged(nameof(Unit));
					OnPropertyChanged(nameof(StitchesPerUnitHeight));
					OnPropertyChanged(nameof(StitchesPerUnitWidth));
					OnPropertyChanged(nameof(SchemeWidth));
					OnPropertyChanged(nameof(SchemeHeight));
				}
			}
		}
		SizeUnit unit = SizeUnit.Stitch;

		public decimal StitchesPerUnitWidth
		{
			get { return Unit == SizeUnit.Stitch ? 1 : stitchesPerUnitWidth; }
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				if (value != stitchesPerUnitWidth)
				{
					width = (int)(SchemeWidth * value);
					stitchesPerUnitWidth = value;

					CallManipulations();

					OnPropertyChanged(nameof(stitchesPerUnitWidth));
					OnPropertyChanged(nameof(Width));
					OnPropertyChanged(nameof(SchemeWidth));
				}
			}
		}
		decimal stitchesPerUnitWidth = 1;
		public decimal StitchesPerUnitHeight
		{
			get { return Unit == SizeUnit.Stitch ? 1 : stitchesPerUnitHeight; }
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				if (value != stitchesPerUnitHeight)
				{
					height = (int)(SchemeHeight * value);
					stitchesPerUnitHeight = value;

					CallManipulations();

					OnPropertyChanged(nameof(StitchesPerUnitHeight));
					OnPropertyChanged(nameof(Height));
					OnPropertyChanged(nameof(SchemeHeight));
				}
			}
		}
		decimal stitchesPerUnitHeight = 1;
		SizeUnit unitForStitchesPerUnit = SizeUnit.Stitch;

		protected override void CallManipulationsCore()
		{
			Manipulator.Resize(new Size(Width, Height), FilterType, SizeLockBy);
		}

		protected override void UpdateValuesFromManipulatedImage()
		{
			base.UpdateValuesFromManipulatedImage();

			width = Manipulator.ManipulatedImage.Size.Width;
			height = Manipulator.ManipulatedImage.Size.Height;

			OnPropertyChanged(nameof(Width));
			OnPropertyChanged(nameof(Height));
			OnPropertyChanged(nameof(SchemeWidth));
			OnPropertyChanged(nameof(SchemeHeight));
		}

		#region Defaults

		protected override void RestoreDefaultsCore()
		{
			using (SuspendCallManipulations())
			{
				KeepAspect = SAEWizardSettings.Default.KeepAspect;
				Height = SAEWizardSettings.Default.ResizeHeight;
				Width = SAEWizardSettings.Default.ResizeWidth; // Set Width last to have KeepWidthScaleHeight by default

				ImageResampler.FilterType defaultFilterType;
				if (Enum.TryParse(SAEWizardSettings.Default.ResizeFilterType, out defaultFilterType))
				{
					FilterType = defaultFilterType;
				}
			}
		}

		protected override void SaveDefaultsCore()
		{
			SAEWizardSettings.Default.KeepAspect = KeepAspect;
			SAEWizardSettings.Default.ResizeWidth = Width;
			SAEWizardSettings.Default.ResizeHeight = Height;
			SAEWizardSettings.Default.ResizeFilterType = FilterType.ToString();
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			using (SuspendCallManipulations())
			{
				height = image.Size.Height;
				width = image.Size.Width;
				CallManipulations();
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));

				Unit = SizeUnit.Stitch;
			}
		}

		#endregion
	}
}

