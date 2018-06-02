using System;
using System.Linq;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageCropController : ImageController<ImageCropManipulator>
	{
		public ImageCropController(ImageCropManipulator manipulator) : base(manipulator)
		{
			ResetCropRect();

			Manipulator.SourceImageChanged += SourceImageChanged;
		}

		public ImageCropper.CropKind CropKind
		{
			get { return cropKind; }
			set
			{
				if (cropKind == value)
				{
					return;
				}

				cropKind = value;
				CallManipulations();

				OnPropertyChanged(nameof(CropKind));
			}
		}
		ImageCropper.CropKind cropKind = ImageCropper.CropKind.None;

		public Rectangle CropRect
		{
			get { return cropRect; }
			set
			{
				if (value.Left < 0) { value.Left = 0; }
				if (value.Left >= Manipulator.SourceImage.Size.Width) { value.Left = Manipulator.SourceImage.Size.Width - 1; }
				if (value.Top < 0) { value.Top = 0; }
				if (value.Top >= Manipulator.SourceImage.Size.Height) { value.Top = Manipulator.SourceImage.Size.Height - 1; }
				if (value.RightExclusive > Manipulator.SourceImage.Size.Width) { value.Width = Manipulator.SourceImage.Size.Width - value.Left; }
				if (value.BottomExclusive > Manipulator.SourceImage.Size.Height) { value.Height = Manipulator.SourceImage.Size.Height - value.Top; }
				if (value.Width < 1) { value.Width = 1; }
				if (value.Height < 1) { value.Height = 1; }

				if (cropRect == value)
				{
					return;
				}

				cropRect = value;
				CallManipulations();
			}
		}
		Rectangle cropRect;

		void ResetCropRect()
		{
			cropRect = new Rectangle(0, 0, Manipulator.SourceImage.Size.Width, Manipulator.SourceImage.Size.Height);
		}

		void SourceImageChanged(object sender, EventArgs e)
		{
			ResetCropRect();
		}

		protected override void CallManipulationsCore()
		{
			if (CropRect.Width > 0 && CropRect.Height > 0)
			{
				switch (CropKind)
				{
					case ImageCropper.CropKind.Rectangle:
						Manipulator.CropRect(CropRect);
						break;
					case ImageCropper.CropKind.Arc:
						Manipulator.CropArc(CropRect);
						break;
					case ImageCropper.CropKind.None:
						Manipulator.Reset();
						break;
					default:
						throw new NotSupportedException($"CropKind {CropKind} is not supported.");
				}
			}
		}

		#region Defaults

		protected override void RestoreDefaultsCore() { }

		protected override void SaveDefaultsCore() { }

		protected override void SaveImageSettingsCore(CodedImage image)
		{
			base.SaveImageSettingsCore(image);

			image.AdditionalData[nameof(CropKind)] = CropKind.ToString();
			if (CropKind != ImageCropper.CropKind.None)
			{
				image.AdditionalData[nameof(CropRect)] = $"{CropRect.Left},{CropRect.Top},{CropRect.Width},{CropRect.Height}";
			}
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			using (SuspendCallManipulations())
			{
				string cropKindText;
				if (image.TryGetAdditionalDataAsString(nameof(CropKind), out cropKindText))
				{
					ImageCropper.CropKind cropKindValue;
					if (Enum.TryParse(cropKindText, out cropKindValue))
					{
						CropKind = cropKindValue;
					}
				}

				if (CropKind != ImageCropper.CropKind.None)
				{
					string cropRectText;
					if (image.TryGetAdditionalDataAsString(nameof(CropRect), out cropRectText))
					{
						var cropRectComponents = cropRectText.Split(',')
							.Select(componentText =>
							{
								int componentValue;
								return int.TryParse(componentText, out componentValue) ? componentValue : -1;
							})
							.ToList();
						if (cropRectComponents.Count == 4 && cropRectComponents.All(c => c >= 0))
						{
							CropRect = new Rectangle(cropRectComponents[0], cropRectComponents[1], cropRectComponents[2], cropRectComponents[3]);
						}
					}
				}
			}
		}

		#endregion
	}
}

