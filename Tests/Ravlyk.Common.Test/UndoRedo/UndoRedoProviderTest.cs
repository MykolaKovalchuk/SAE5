using System;
using NUnit.Framework;

namespace Ravlyk.Common.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoProviderTest
	{
		[Test]
		public void TestCanUndoRedo()
		{
			var provider = new UndoRedoProvider4Test();
			Assert.That(provider.CanUndo, Is.False);
			Assert.That(provider.CanRedo, Is.False);

			provider.AddUndoActionExposed("Test", () => { }, () => { });
			Assert.That(provider.CanUndo, Is.True);
			Assert.That(provider.CanRedo, Is.False);

			provider.Undo();
			Assert.That(provider.CanUndo, Is.False);
			Assert.That(provider.CanRedo, Is.True);

			provider.Redo();
			Assert.That(provider.CanUndo, Is.True);
			Assert.That(provider.CanRedo, Is.False);
		}

		[Test]
		public void TestAddUndoActionClearsRedoSteps()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			provider.Undo();
			Assert.That(provider.CanRedo, Is.True);
			Assert.That(provider.RedoDescription, Is.EqualTo("Aaa"));

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.That(provider.CanRedo, Is.False);
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));
		}

		[Test]
		public void TestUndoRedoDescription()
		{
			var provider = new UndoRedoProvider4Test();
			Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));

			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.That(provider.UndoDescription, Is.EqualTo("Bbb"));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));

			provider.Undo();
			Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));
			Assert.That(provider.RedoDescription, Is.EqualTo("Bbb"));

			provider.Undo();
			Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
			Assert.That(provider.RedoDescription, Is.EqualTo("Aaa"));

			provider.Redo();
			Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));
			Assert.That(provider.RedoDescription, Is.EqualTo("Bbb"));

			provider.Redo();
			Assert.That(provider.UndoDescription, Is.EqualTo("Bbb"));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));
		}

		[Test]
		public void TestUndoRedoDescriptionForMultiActionsStep()
		{
			var provider = new UndoRedoProvider4Test();
			using (provider.BeginMultiActionsUndoRedoStep("Mmm"))
			{
				provider.AddUndoActionExposed("Aaa", () => { }, () => { });
				Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
				provider.AddUndoActionExposed("Bbb", () => { }, () => { });
				Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
			}
			Assert.That(provider.UndoDescription, Is.EqualTo("Mmm"));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));

			provider.Undo();
			Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
			Assert.That(provider.RedoDescription, Is.EqualTo("Mmm"));
		}

		[Test]
		public void TestUndoRedo()
		{
			var provider = new UndoRedoProvider4Test();
			var s = string.Empty;

			provider.AddUndoActionExposed(string.Empty, () => s += "a", () => s += "A");
			provider.AddUndoActionExposed(string.Empty, () => s += "b", () => s += "B");
			provider.AddUndoActionExposed(string.Empty, () => s += "c", () => s += "C");

			Assert.That(s, Is.EqualTo(string.Empty));

			provider.Undo();
			Assert.That(s, Is.EqualTo("c"));
			provider.Redo();
			Assert.That(s, Is.EqualTo("cC"));

			s = string.Empty;
			provider.Undo();
			provider.Undo();
			provider.Undo();
			Assert.That(s, Is.EqualTo("cba"));
			provider.Redo();
			provider.Redo();
			Assert.That(s, Is.EqualTo("cbaAB"));
			provider.Redo();
			Assert.That(s, Is.EqualTo("cbaABC"));
		}

		[Test]
		public void TestUndoRedoMultiActionsStep()
		{
			var provider = new UndoRedoProvider4Test();
			var s = string.Empty;

			using (provider.BeginMultiActionsUndoRedoStep(string.Empty))
			{
				provider.AddUndoActionExposed(string.Empty, () => s += "a", () => s += "A");
				provider.AddUndoActionExposed(string.Empty, () => s += "b", () => s += "B");
			}
			using (provider.BeginMultiActionsUndoRedoStep(string.Empty))
			{
				provider.AddUndoActionExposed(string.Empty, () => s += "c", () => s += "C");
				provider.AddUndoActionExposed(string.Empty, () => s += "d", () => s += "D");
			}

			Assert.That(s, Is.EqualTo(string.Empty));

			provider.Undo();
			Assert.That(s, Is.EqualTo("dc"));
			provider.Redo();
			Assert.That(s, Is.EqualTo("dcCD"));

			s = string.Empty;
			provider.Undo();
			provider.Undo();
			Assert.That(s, Is.EqualTo("dcba"));
			provider.Redo();
			provider.Redo();
			Assert.That(s, Is.EqualTo("dcbaABCD"));
		}

		[Test]
		public void TestClearCache()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			provider.Undo();

			Assert.That(provider.CanUndo, Is.True);
			Assert.That(provider.CanRedo, Is.True);
			Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));
			Assert.That(provider.RedoDescription, Is.EqualTo("Bbb"));

			provider.ClearCache();

			Assert.That(provider.CanUndo, Is.False);
			Assert.That(provider.CanRedo, Is.False);
			Assert.That(provider.UndoDescription, Is.EqualTo(string.Empty));
			Assert.That(provider.RedoDescription, Is.EqualTo(string.Empty));
		}

		[Test]
		public void TestSuppressUndoRedo()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));

			using (provider.SuppressUndoRegistration())
			{
				provider.AddUndoActionExposed("Bbb", () => { }, () => { });
				Assert.That(provider.UndoDescription, Is.EqualTo("Aaa"));
			}

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.That(provider.UndoDescription, Is.EqualTo("Bbb"));
		}

		[Test]
		public void TestStateChanged()
		{
			var provider = new UndoRedoProvider4Test();
			var counter = 0;

			provider.StateChanged += (sender, e) => counter++;
			Assert.That(counter, Is.EqualTo(0));

			provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
			Assert.That(counter, Is.EqualTo(1));

			provider.Undo();
			Assert.That(counter, Is.EqualTo(2));

			provider.Redo();
			Assert.That(counter, Is.EqualTo(3));

			using (provider.BeginMultiActionsUndoRedoStep(string.Empty))
			{
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
				Assert.That(counter, Is.EqualTo(3));
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
				Assert.That(counter, Is.EqualTo(3));
			}
			Assert.That(counter, Is.EqualTo(4));

			using (provider.SuppressUndoRegistration())
			{
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
			}
			Assert.That(counter, Is.EqualTo(4));

			provider.ClearCache();
			Assert.That(counter, Is.EqualTo(5));
		}

		class UndoRedoProvider4Test : UndoRedoProvider
		{
			public void AddUndoActionExposed(string name, Action undo, Action redo)
			{
				AddUndoAction(new UndoRedoActionTest.UndoRedoAction4Test(name, undo, redo));
			}
		}
	}
}

