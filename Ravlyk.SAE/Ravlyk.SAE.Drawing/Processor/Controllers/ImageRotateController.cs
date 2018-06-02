using System;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageRotateController : ImageController<ImageRotateManipulator>
	{
		public ImageRotateController(ImageRotateManipulator manipulator) : base(manipulator) { }

		public void RotateCW()
		{
			Manipulator.RotateCW();
		}

		public void RotateCCW()
		{
			Manipulator.RotateCCW();
		}

		public void FlipHorizontally()
		{
			Manipulator.FlipHorizontally();
		}

		public void FlipVertically()
		{
			Manipulator.FlipVertically();
		}

		#region Defaults

		protected override void RestoreDefaultsCore() { }

		protected override void SaveDefaultsCore() { }

		protected override void SaveImageSettingsCore(CodedImage image)
		{
			base.SaveImageSettingsCore(image);

			var rotateHistory = Manipulator.GetManipulationsHistory();
			if (!string.IsNullOrEmpty(rotateHistory))
			{
				image.AdditionalData[nameof(ImageRotateController)] = rotateHistory;
			}
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			string rotateHistory;
			if (image.TryGetAdditionalDataAsString(nameof(ImageRotateController), out rotateHistory))
			{
				Manipulator.RestoreManipulations(rotateHistory);
			}
		}

		#endregion
	}
}

