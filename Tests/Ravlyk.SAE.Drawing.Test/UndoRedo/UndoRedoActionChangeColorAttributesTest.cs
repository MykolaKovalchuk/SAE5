using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionChangeColorAttributesTest
	{
		[Test]
		public void TestAction()
		{
			var image = new CodedImage();
			image.Palette = new CodedPalette();
			image.Palette.Add(new CodedColor(1));
			var action = new UndoRedoActionChangeColorAttributes(image, new CodedColor(1), 'A', "100", "Black", 'B', "200", "White");

			AssertColor(image, 1, ' ', null, null);

			action.Undo();
			AssertColor(image, 1, 'A', "100", "Black");

			action.Redo();
			AssertColor(image, 1, 'B', "200", "White");
		}

		void AssertColor(CodedImage image, int colorHash, char expectedSymbol, string expectedCode, string expectedName)
		{
			var color = image.Palette[colorHash];
			Assert.IsNotNull(color);

			Assert.AreEqual(expectedSymbol, color.SymbolChar);
			Assert.AreEqual(expectedCode, color.ColorCode);
			Assert.AreEqual(expectedName, color.ColorName);
		}
	}
}

