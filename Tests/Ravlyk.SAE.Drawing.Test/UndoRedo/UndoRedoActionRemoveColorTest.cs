using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionRemoveColorTest
	{
		[Test]
		public void TestAction()
		{
			var image = new CodedImage();
			image.Palette = new CodedPalette();
			var color = new CodedColor(1);
			var action = new UndoRedoActionRemoveColor(image, color);

			Assert.AreEqual(0, image.Palette.Count, "Precondition");

			action.Undo();
			Assert.AreEqual(1, image.Palette.Count);
			Assert.AreEqual(color, image.Palette[1]);

			action.Redo();
			Assert.AreEqual(0, image.Palette.Count);
		}
	}
}

