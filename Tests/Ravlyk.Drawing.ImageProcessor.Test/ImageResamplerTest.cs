using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.Drawing.ImageProcessor.Test
{
	[TestFixture]
	public class ImageResamplerTest
	{
		[Test]
		public void TestResampleWithCrop()
		{
			var sourceImage = new IndexedImage();
			sourceImage.Size = new Size(43, 37);
			var random = new Random();
			for (int i = 0; i < sourceImage.Pixels.Length; i++)
			{
				sourceImage.Pixels[i] = random.Next();
			}

			var newSize = new Size(71, 59);
			var fullResampledImage = new ImageResampler().Resample(sourceImage, newSize, ImageResampler.FilterType.Lanczos3);
			Assert.AreEqual(newSize, fullResampledImage.Size);

			var frame = new Rectangle(7, 11, 23, 19);
			var croppedResampledImage = new ImageResampler().Resample(sourceImage, newSize, ImageResampler.FilterType.Lanczos3, frame);
			Assert.AreEqual(new Size(frame.Width, frame.Height), croppedResampledImage.Size);

			for (int y = 0; y < frame.Height; y++)
			{
				for (int x = 0; x < frame.Width; x++)
				{
					Assert.AreEqual(fullResampledImage.Pixels[(y + frame.Top) * fullResampledImage.Size.Width + (x + frame.Left)], croppedResampledImage.Pixels[y * croppedResampledImage.Size.Width + x]);
				}
			}
		}
	}
}
