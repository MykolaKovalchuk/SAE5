using System;
using NUnit.Framework;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageSizeManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestOperationsCallsOnImageChanged()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var resizer = new ImageSizeManipulator(image);
			var childManipulator = new ImageManipulatorTest.DummyManipulator(resizer);

			AssertActionCallsOnImageChanged(childManipulator, () => resizer.Resize(new Size(3, 3), ImageResampler.FilterType.Box, ImageSizeManipulator.SizeLockType.KeepWidthAndHeight));
		}

		[Test]
		public void TestResize()
		{
			var resizer = new ImageSizeManipulator(new CodedImage { Size = new Size(5, 5) });
			Assert.AreEqual(new Size(5, 5), resizer.ManipulatedImage.Size);

			resizer.Resize(new Size(10, 7), ImageResampler.FilterType.Box, ImageSizeManipulator.SizeLockType.None);
			Assert.AreEqual(new Size(10, 7), resizer.ManipulatedImage.Size);
		}

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			AssertResizeWithLastSettingsOnParentImageChanged(new Size(20, 15), new Size(12, 8), ImageSizeManipulator.SizeLockType.None, new Size(30, 40), new Size(30, 40));
			AssertResizeWithLastSettingsOnParentImageChanged(new Size(20, 15), new Size(12, 8), ImageSizeManipulator.SizeLockType.KeepWidthAndHeight, new Size(30, 40), new Size(12, 8));
			AssertResizeWithLastSettingsOnParentImageChanged(new Size(20, 15), new Size(12, 8), ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight, new Size(30, 40), new Size(12, 16));
			AssertResizeWithLastSettingsOnParentImageChanged(new Size(20, 15), new Size(12, 8), ImageSizeManipulator.SizeLockType.ScaleWidthKeepHeight, new Size(30, 40), new Size(6, 8));
		}

		void AssertResizeWithLastSettingsOnParentImageChanged(Size initialSize, Size changedSize, ImageSizeManipulator.SizeLockType sizeLockBy, Size newSize, Size expectedSize)
		{
			var dummyManipulator = new ImageManipulatorTest.DummyManipulator(new CodedImage { Size = initialSize });
			var resizer = new ImageSizeManipulator(dummyManipulator);
			Assert.AreEqual(initialSize, resizer.ManipulatedImage.Size, "Precondition");

			resizer.Resize(changedSize, ImageResampler.FilterType.Box, sizeLockBy);
			Assert.AreEqual(changedSize, resizer.ManipulatedImage.Size, "Precondition 2");

			dummyManipulator.ManipulatedImage.Size = newSize;
			Assert.AreEqual(changedSize, resizer.ManipulatedImage.Size, "Not changed yet");

			dummyManipulator.CallOnImageChanged();
			Assert.AreEqual(expectedSize, resizer.ManipulatedImage.Size);
		}
	}
}

