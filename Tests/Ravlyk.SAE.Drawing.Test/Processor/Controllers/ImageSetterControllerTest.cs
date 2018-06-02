using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageSetterControllerTest
	{
		[Test]
		public void TestSetNewImage()
		{
			var controller = new ImageSetterController(new ImageSetterManipulator());

			var newImage = new CodedImage { Size = new Size(5, 5) };
			int[] newPixels, manipulatedPixels;
			using (newImage.LockPixels(out newPixels))
			{
				for (int i = 0; i < newPixels.Length; i++)
				{
					newPixels[i] = i;
				}
			}

			controller.SetNewImage(newImage);

			Assert.AreEqual(newImage.Size, controller.Manipulator.ManipulatedImage.Size);

			using (newImage.LockPixels(out newPixels))
			using (controller.Manipulator.ManipulatedImage.LockPixels(out manipulatedPixels))
			{
				Assert.AreEqual(newPixels.Length, manipulatedPixels.Length);
				for (int i = 0; i < newPixels.Length; i++)
				{
					Assert.AreEqual(newPixels[i], manipulatedPixels[i], string.Format("Pixels in position {0} should be same", i));
				}
			}
		}
	}
}

