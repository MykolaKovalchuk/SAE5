using System;
using NUnit.Framework;
using Ravlyk.Drawing;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoProviderTest
	{
		[Test]
		public void TestAttachToImage()
		{
			var image1 = new CodedImage { Palette = new CodedPalette() };
			var image2 = new CodedImage { Palette = new CodedPalette() };
			var undoRedoProvider = new UndoRedoProvider(image1);

			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image2.Palette.Add(new Color(1));
			Assert.IsFalse(undoRedoProvider.CanUndo);
			image1.Palette.Add(new Color(1));
			Assert.IsTrue(undoRedoProvider.CanUndo);

			undoRedoProvider.AttachToImage(image2);
			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image1.Palette.Add(new Color(2));
			Assert.IsFalse(undoRedoProvider.CanUndo);
			image2.Palette.Add(new Color(2));
			Assert.IsTrue(undoRedoProvider.CanUndo);
		}

		[Test]
		public void TestPixelColorChanged()
		{
			var image = new CodedImage { Size = new Size(2, 2), Palette = new CodedPalette { new CodedColor(0), new CodedColor(1) } };
			image[0, 0] = new CodedColor(1);
			image[0, 1] = new CodedColor(1);
			image[1, 0] = new CodedColor(0);
			image[1, 1] = new CodedColor(0);

			var undoRedoProvider = new UndoRedoProvider(image);

			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image[1, 1] = new CodedColor(1);
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeCellColor, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(new CodedColor(1), image[1, 1]);

			undoRedoProvider.Undo();
			Assert.IsFalse(undoRedoProvider.CanUndo);
			Assert.IsTrue(undoRedoProvider.CanRedo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeCellColor, undoRedoProvider.RedoDescription);
			Assert.AreEqual(new CodedColor(0), image[1, 1]);

			undoRedoProvider.Redo();
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeCellColor, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(new CodedColor(1), image[1, 1]);
		}

		[Test]
		public void TestColorAdded()
		{
			var image = new CodedImage { Size = new Size(2, 3), Palette = new CodedPalette() };
			var undoRedoProvider = new UndoRedoProvider(image);

			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image.Palette.Add(new CodedColor(1) { ColorCode = "100" });
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionAddThread, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(1, image.Palette.Count);
			Assert.AreEqual("100", image.Palette[1].ColorCode);

			undoRedoProvider.Undo();
			Assert.IsFalse(undoRedoProvider.CanUndo);
			Assert.IsTrue(undoRedoProvider.CanRedo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionAddThread, undoRedoProvider.RedoDescription);
			Assert.AreEqual(0, image.Palette.Count);

			undoRedoProvider.Redo();
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionAddThread, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(1, image.Palette.Count);
			Assert.AreEqual("100", image.Palette[1].ColorCode);
		}

		[Test]
		public void TestColorRemoved()
		{
			var image = new CodedImage { Size = new Size(2, 3), Palette = new CodedPalette { new CodedColor(1) { ColorCode = "100" } } };
			var undoRedoProvider = new UndoRedoProvider(image);

			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image.Palette.Remove(1);
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionRemoveThread, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(0, image.Palette.Count);

			undoRedoProvider.Undo();
			Assert.IsFalse(undoRedoProvider.CanUndo);
			Assert.IsTrue(undoRedoProvider.CanRedo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionRemoveThread, undoRedoProvider.RedoDescription);
			Assert.AreEqual(1, image.Palette.Count);
			Assert.AreEqual("100", image.Palette[1].ColorCode);

			undoRedoProvider.Redo();
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionRemoveThread, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual(0, image.Palette.Count);
		}

		[Test]
		public void TestColorAttributesChanged()
		{
			var image = new CodedImage { Size = new Size(2, 3), Palette = new CodedPalette { new CodedColor(1) { SymbolChar = 'A', ColorCode = "100", ColorName = "Black" } } };
			var color = image.Palette[1];
			var undoRedoProvider = new UndoRedoProvider(image);

			Assert.IsFalse(undoRedoProvider.CanUndo, "Precondition");

			image.Palette.ChangeColorAttributes(color, 'B', "200", "White");
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeColorSymbol, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual('B', color.SymbolChar);
			Assert.AreEqual("200", color.ColorCode);
			Assert.AreEqual("White", color.ColorName);

			undoRedoProvider.Undo();
			Assert.IsFalse(undoRedoProvider.CanUndo);
			Assert.IsTrue(undoRedoProvider.CanRedo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeColorSymbol, undoRedoProvider.RedoDescription);
			Assert.AreEqual('A', color.SymbolChar);
			Assert.AreEqual("100", color.ColorCode);
			Assert.AreEqual("Black", color.ColorName);

			undoRedoProvider.Redo();
			Assert.IsTrue(undoRedoProvider.CanUndo);
			Assert.AreEqual(Ravlyk.SAE.Drawing.Properties.Resources.UndoRedoActionChangeColorSymbol, undoRedoProvider.UndoDescription);
			Assert.IsFalse(undoRedoProvider.CanRedo);
			Assert.AreEqual('B', color.SymbolChar);
			Assert.AreEqual("200", color.ColorCode);
			Assert.AreEqual("White", color.ColorName);
		}
	}
}

