using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionChangePixelColorTest
	{
		[Test]
		public void TestAction()
		{
			var image = new CodedImage { Size = new Size(2, 3) };
			image.CompletePalette();
			var oldColor = new CodedColor(100);
			var newColor = new CodedColor(200);
			var action = new UndoRedoActionChangePixelColor(image, 1, 2, oldColor, newColor);
			image[1, 2] = new CodedColor(0);

			Assert.AreEqual(new CodedColor(0), image[1, 2], "Precondition");

			action.Undo();
			Assert.AreEqual(oldColor, image[1, 2]);

			action.Redo();
			Assert.AreEqual(newColor, image[1, 2]);
		}
	}
}

