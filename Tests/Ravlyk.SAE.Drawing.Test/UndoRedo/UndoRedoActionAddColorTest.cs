using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionAddColorTest
	{
		[Test]
		public void TestAction()
		{
			var image = new CodedImage { Palette = new CodedPalette() };
			var color = new CodedColor(1) { ColorCode = "100" };
			var action = new UndoRedoActionAddColor(image, color);

			Assert.AreEqual(0, image.Palette.Count, "Precondition");

			action.Redo();
			Assert.AreEqual(1, image.Palette.Count);
			Assert.AreEqual(color, image.Palette[1]);
			Assert.AreEqual("100", image.Palette[1].ColorCode);

			action.Undo();
			Assert.AreEqual(0, image.Palette.Count);
		}
	}
}

