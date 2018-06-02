using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Test
{
	[TestFixture]
	public class CodedPaletteTest
	{
		[Test]
		public void TestNewColor()
		{
			CodedPalette palette =
				new CodedPalette
				{
					new Color(1),
					new CodedColor(2)
				};

			Assert.AreEqual(typeof(CodedColor), palette[1].GetType());
			Assert.AreEqual(typeof(CodedColor), palette[2].GetType());

			IndexedImage image = new IndexedImage { Size = new Size(2, 2) };
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				pixels[0] = 0;
				pixels[1] = 1;
				pixels[2] = 2;
				pixels[3] = 1;
			}

			image.Palette = new CodedPalette();
			image.CompletePalette();

			Assert.AreEqual(3, image.Palette.Count);
			foreach (var color in image.Palette)
			{
				Assert.AreEqual(typeof(CodedColor), color.GetType());
			}
		}

		[Test]
		public void TestChangeColorAttributes()
		{
			CodedColor color = new CodedColor(0) { SymbolChar = 'A', ColorCode = "100", ColorName = "Black" };
			CodedPalette palette = new CodedPalette();

			bool eventFired = false;
			palette.ColorAttributesChanged +=
				(sender, e) =>
				{
					eventFired = true;
					Assert.AreSame(palette, sender);
					Assert.AreEqual('A', e.OldSymbol);
					Assert.AreEqual('B', e.NewSymbol);
					Assert.AreEqual("100", e.OldCode);
					Assert.AreEqual("200", e.NewCode);
					Assert.AreEqual("Black", e.OldName);
					Assert.AreEqual("White", e.NewName);
				};

			Assert.IsFalse(eventFired, "Precondition");
			palette.ChangeColorAttributes(color, 'B', "200", "White");
			Assert.IsTrue(eventFired, "Event should have been fired");
		}

		[Test]
		public void TestGetEnumerator()
		{
			CodedPalette palette =
				new CodedPalette
				{
					new CodedColor(1),
					new CodedColor(2),
					new CodedColor(3)
				};

			Assert.AreEqual(3, palette.Count);
			foreach (CodedColor color in (IEnumerable<CodedColor>)palette)
			{
				Assert.IsNotNull(color);
			}
		}

		[Test]
		public void TestClone()
		{
			CodedPalette palette = new CodedPalette { Name = "Test1", SymbolsFont = "Arial" };
			CodedPalette clonedPalette = palette.Clone();

			Assert.AreEqual(typeof(CodedPalette), clonedPalette.GetType());
			Assert.AreNotSame(palette, clonedPalette);
			Assert.AreEqual(palette.Count, clonedPalette.Count);
			Assert.IsTrue(((IEnumerable<CodedColor>)palette).All(color => clonedPalette.Contains(color)));
			Assert.AreEqual("Test1", clonedPalette.Name);
			Assert.AreEqual("Arial", clonedPalette.SymbolsFont);
		}
	}
}

