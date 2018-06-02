using System;
using NUnit.Framework;

namespace Ravlyk.Common.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoStepTest
	{
		[Test]
		public void TestName()
		{
			Assert.AreEqual("Aaa", new UndoRedoStep("Aaa").Name);
			Assert.AreEqual("Bbb", new UndoRedoStep(new UndoRedoActionTest.UndoRedoAction4Test("Bbb", null, null)).Name);
		}

		[Test]
		public void TestUndoRedo()
		{
			var step = new UndoRedoStep("Test");
			var s = string.Empty;

			step.Undo();
			Assert.AreEqual(string.Empty, s);
			step.Redo();
			Assert.AreEqual(string.Empty, s);

			step.AddAction(new UndoRedoActionTest.UndoRedoAction4Test(string.Empty, () => s += "x", () => s += "X"));

			step.Undo();
			Assert.AreEqual("x", s);
			step.Redo();
			Assert.AreEqual("xX", s);

			step.AddAction(new UndoRedoActionTest.UndoRedoAction4Test(string.Empty, () => s += "y", () => s += "Y"));

			s = string.Empty;
			step.Undo();
			Assert.AreEqual("yx", s);
			step.Redo();
			Assert.AreEqual("yxXY", s);
		}
	}
}

