using System;
using System.IO;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Serialization.Test
{
	[TestFixture]
	public class ImageSerializerTest
	{
		[Test]
		public void TestSerialize()
		{
			var image = new CodedImage { Size = new Size(2, 3), SourceImageFileName = @"c:\Temp\Test Image.jpg" };
			image.CompletePalette();
			image[0, 0] = new CodedColor(1);
			image[0, 1] = new CodedColor(100);
			image[0, 2] = new CodedColor(2);
			image[1, 0] = new CodedColor(2);
			image[1, 1] = new CodedColor(100);
			image[1, 2] = new CodedColor(3);
			Assert.AreEqual(4, image.Palette.Count, "Precondition");

			var tempFileName = Path.GetTempFileName();
			try
			{
				using (var fs = new FileStream(tempFileName, FileMode.Create))
				{
					image.SaveToStream(fs);
				}

				using (var fs = new FileStream(tempFileName, FileMode.Open))
				{
					var reloadedImage = ImageSerializer.LoadFromStream(fs);

					Assert.AreEqual(new Size(2, 3), reloadedImage.Size);
					Assert.AreEqual(@"c:\Temp\Test Image.jpg", reloadedImage.SourceImageFileName);

					Assert.IsNotNull(reloadedImage.Palette);
					Assert.AreEqual(4, reloadedImage.Palette.Count);
					Assert.IsTrue(reloadedImage.Palette[1] is CodedColor);
					Assert.IsTrue(reloadedImage.Palette[2] is CodedColor);
					Assert.IsTrue(reloadedImage.Palette[3] is CodedColor);
					Assert.IsTrue(reloadedImage.Palette[100] is CodedColor);

					Assert.AreEqual(new CodedColor(1), reloadedImage[0, 0]);
					Assert.AreEqual(new CodedColor(100), reloadedImage[0, 1]);
					Assert.AreEqual(new CodedColor(2), reloadedImage[0, 2]);
					Assert.AreEqual(new CodedColor(2), reloadedImage[1, 0]);
					Assert.AreEqual(new CodedColor(100), reloadedImage[1, 1]);
					Assert.AreEqual(new CodedColor(3), reloadedImage[1, 2]);
				}
			}
			finally
			{
				if (File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}
			}
		}
	}
}

