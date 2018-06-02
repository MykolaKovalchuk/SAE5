using System;
using NUnit.Framework;
using Ravlyk.Drawing;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Test
{
	[TestFixture]
	public class CodedImageTest
	{
		[Test]
		public void TestPalette()
		{
			var image = new CodedImage();
			Assert.IsTrue(image.Palette is CodedPalette);

			var newPalette = new CodedPalette();
			image.Palette = newPalette;
			Assert.AreEqual(newPalette, image.Palette);
		}

		[Test]
		public void TestFromIndexedImage()
		{
			var indexedImage = new IndexedImage { Size = new Size(2, 3) };
			var codedImage = CodedImage.FromIndexedImage(indexedImage);
			Assert.AreEqual(indexedImage.Size, codedImage.Size);

			int[] indexedPixels, codedPixels;
			using (indexedImage.LockPixels(out indexedPixels))
			using (codedImage.LockPixels(out codedPixels))
			{
				Assert.AreSame(indexedPixels, codedPixels, "Should use same pixels array");
			}
		}

		[Test]
		public void TestClone()
		{
			var image = new CodedImage { Size = new Size(2, 3), SourceImageFileName = @"c:\test.jpg" };
			CodedImage clonedImage = image.Clone(false);
			Assert.AreEqual(@"c:\test.jpg", clonedImage.SourceImageFileName);
		}
	}
}

