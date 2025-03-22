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
			Assert.That(new UndoRedoStep("Aaa").Name, Is.EqualTo("Aaa"));
			Assert.That(new UndoRedoStep(new UndoRedoActionTest.UndoRedoAction4Test("Bbb", null, null)).Name, Is.EqualTo("Bbb"));
		}

		[Test]
		public void TestUndoRedo()
		{
			var step = new UndoRedoStep("Test");
			var s = string.Empty;

			step.Undo();
			Assert.That(s, Is.EqualTo(string.Empty));
			step.Redo();
			Assert.That(s, Is.EqualTo(string.Empty));

			step.AddAction(new UndoRedoActionTest.UndoRedoAction4Test(string.Empty, () => s += "x", () => s += "X"));

			step.Undo();
			Assert.That(s, Is.EqualTo("x"));
			step.Redo();
			Assert.That(s, Is.EqualTo("xX"));

			step.AddAction(new UndoRedoActionTest.UndoRedoAction4Test(string.Empty, () => s += "y", () => s += "Y"));

			s = string.Empty;
			step.Undo();
			Assert.That(s, Is.EqualTo("yx"));
			step.Redo();
			Assert.That(s, Is.EqualTo("yxXY"));
		}
	}
}

