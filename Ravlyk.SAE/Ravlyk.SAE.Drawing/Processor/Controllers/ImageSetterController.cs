using System;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSetterController : ImageController<ImageSetterManipulator>
	{
		public ImageSetterController(ImageSetterManipulator manipulator) : base(manipulator) { }

		public void SetNewImage(CodedImage newImage)
		{
			Manipulator.SetNewImage(newImage);
		}

		public string ImageSourceDescription => Manipulator.ManipulatedImage.SourceImageFileName;

		#region Defaults

		protected override void RestoreDefaultsCore() { }

		protected override void SaveDefaultsCore() { }

		#endregion
	}
}

