using System;
using System.Linq;

using NUnit.Framework;

using Ravlyk.Common;
using Ravlyk.SAE.Resources;
using Ravlyk.Drawing.ImageProcessor.Utilities;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	class ImageSymbolsManipulatorTest : ImageManipulatorTestCase
	{
		[Test]
		public void TestApplySymbols()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = i % 3;
				}
			}
			srcImage.CompletePalette();

			Assert.AreEqual(3, srcImage.Palette.Count, "Precondition - there should be 3 colors.");

			var symboler = new ImageSymbolsManipulator(srcImage);

			Assert.AreSame(symboler.SourceImage, symboler.ManipulatedImage);

			var font = SAEResources.GetAllFonts(string.Empty).First();
			symboler.ApplySymbols(new[] { ' ', '.', 'X' }, font);

			Assert.AreEqual(font.Name, srcImage.Palette.SymbolsFont);

			var colors = srcImage.Palette.OrderByDarknes(true).Cast<CodedColor>().ToArray();

			Assert.AreEqual(3, colors.Length);
			Assert.AreEqual(2, colors[0].Argb);
			Assert.AreEqual(' ', colors[0].SymbolChar);
			Assert.AreEqual(1, colors[1].Argb);
			Assert.AreEqual('.', colors[1].SymbolChar);
			Assert.AreEqual(0, colors[2].Argb);
			Assert.AreEqual('X', colors[2].SymbolChar);
		}

		[Test]
		public override void TestRestoreManipulationsCore()
		{
			var srcImage = new CodedImage { Size = new Size(3, 2) };
			srcImage.CompletePalette();
			Assert.AreEqual(1, srcImage.Palette.Count, "Precondition - there should be one color for #00000000.");

			var symboler = new ImageSymbolsManipulator(srcImage);

			Assert.AreSame(symboler.SourceImage, symboler.ManipulatedImage);

			var font = SAEResources.GetAllFonts(string.Empty).First();
			symboler.ApplySymbols(new[] { 'x' }, font);

			Assert.AreEqual('x', srcImage.Palette.First<CodedColor>().SymbolChar);

			int[] pixels;
			using (srcImage.LockPixels(out pixels))
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = 0x00ffffff;
				}
			}
			srcImage.CompletePalette();
			srcImage.Palette.SymbolsFont = "abc";

			Assert.AreEqual(1, srcImage.Palette.Count, "There should be one color for #00ffffff.");
			Assert.AreEqual(' ', srcImage.Palette.First<CodedColor>().SymbolChar, "Space should be default symbol for new colors.");
			Assert.AreNotEqual(font.Name, srcImage.Palette.SymbolsFont);

			symboler.RestoreManipulations();

			Assert.AreEqual('x', srcImage.Palette.First<CodedColor>().SymbolChar);
			Assert.AreEqual(font.Name, srcImage.Palette.SymbolsFont);
		}
	}
}
