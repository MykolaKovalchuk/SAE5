using System;
using NUnit.Framework;

namespace Ravlyk.Common.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionTest
	{
		[Test]
		public void TestUndoRedoActionTest()
		{
			int index = 0;

			var action = new UndoRedoAction4Test("Test", () => index--, () => index++);
			Assert.That(index, Is.EqualTo(0));

			action.Undo();
			Assert.That(index, Is.EqualTo(-1));

			action.Redo();
			Assert.That(index, Is.EqualTo(0));

			action.Redo();
			Assert.That(index, Is.EqualTo(1));

			action.Undo();
			Assert.That(index, Is.EqualTo(0));

			action.Undo();
			Assert.That(index, Is.EqualTo(-1));
		}

		internal class UndoRedoAction4Test : UndoRedoAction
		{
			public UndoRedoAction4Test(string name, Action undo, Action redo)
			{
				this.name = name;
				this.undo = undo;
				this.redo = redo;
			}

			readonly string name;
			readonly Action undo;
			readonly Action redo;

			public override string DefaultName
			{
				get { return name; }
			}

			public override void Undo()
			{
				undo();
			}

			public override void Redo()
			{
				redo();
			}
		}
	}
}

