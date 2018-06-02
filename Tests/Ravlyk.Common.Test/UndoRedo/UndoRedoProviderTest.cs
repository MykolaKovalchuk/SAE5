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
			Assert.IsFalse(provider.CanUndo);
			Assert.IsFalse(provider.CanRedo);

			provider.AddUndoActionExposed("Test", () => { }, () => { });
			Assert.IsTrue(provider.CanUndo);
			Assert.IsFalse(provider.CanRedo);

			provider.Undo();
			Assert.IsFalse(provider.CanUndo);
			Assert.IsTrue(provider.CanRedo);

			provider.Redo();
			Assert.IsTrue(provider.CanUndo);
			Assert.IsFalse(provider.CanRedo);
		}

		[Test]
		public void TestAddUndoActionClearsRedoSteps()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			provider.Undo();
			Assert.IsTrue(provider.CanRedo);
			Assert.AreEqual("Aaa", provider.RedoDescription);

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.IsFalse(provider.CanRedo);
			Assert.AreEqual(string.Empty, provider.RedoDescription);
		}

		[Test]
		public void TestUndoRedoDescription()
		{
			var provider = new UndoRedoProvider4Test();
			Assert.AreEqual(string.Empty, provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);

			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			Assert.AreEqual("Aaa", provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.AreEqual("Bbb", provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);

			provider.Undo();
			Assert.AreEqual("Aaa", provider.UndoDescription);
			Assert.AreEqual("Bbb", provider.RedoDescription);

			provider.Undo();
			Assert.AreEqual(string.Empty, provider.UndoDescription);
			Assert.AreEqual("Aaa", provider.RedoDescription);

			provider.Redo();
			Assert.AreEqual("Aaa", provider.UndoDescription);
			Assert.AreEqual("Bbb", provider.RedoDescription);

			provider.Redo();
			Assert.AreEqual("Bbb", provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);
		}

		[Test]
		public void TestUndoRedoDescriptionForMultiActionsStep()
		{
			var provider = new UndoRedoProvider4Test();
			using (provider.BeginMultiActionsUndoRedoStep("Mmm"))
			{
				provider.AddUndoActionExposed("Aaa", () => { }, () => { });
				Assert.AreEqual(string.Empty, provider.UndoDescription);
				provider.AddUndoActionExposed("Bbb", () => { }, () => { });
				Assert.AreEqual(string.Empty, provider.UndoDescription);
			}
			Assert.AreEqual("Mmm", provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);

			provider.Undo();
			Assert.AreEqual(string.Empty, provider.UndoDescription);
			Assert.AreEqual("Mmm", provider.RedoDescription);
		}

		[Test]
		public void TestUndoRedo()
		{
			var provider = new UndoRedoProvider4Test();
			var s = string.Empty;

			provider.AddUndoActionExposed(string.Empty, () => s += "a", () => s += "A");
			provider.AddUndoActionExposed(string.Empty, () => s += "b", () => s += "B");
			provider.AddUndoActionExposed(string.Empty, () => s += "c", () => s += "C");

			Assert.AreEqual(string.Empty, s);

			provider.Undo();
			Assert.AreEqual("c", s);
			provider.Redo();
			Assert.AreEqual("cC", s);

			s = string.Empty;
			provider.Undo();
			provider.Undo();
			provider.Undo();
			Assert.AreEqual("cba", s);
			provider.Redo();
			provider.Redo();
			Assert.AreEqual("cbaAB", s);
			provider.Redo();
			Assert.AreEqual("cbaABC", s);
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

			Assert.AreEqual(string.Empty, s);

			provider.Undo();
			Assert.AreEqual("dc", s);
			provider.Redo();
			Assert.AreEqual("dcCD", s);

			s = string.Empty;
			provider.Undo();
			provider.Undo();
			Assert.AreEqual("dcba", s);
			provider.Redo();
			provider.Redo();
			Assert.AreEqual("dcbaABCD", s);
		}

		[Test]
		public void TestClearCache()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			provider.Undo();

			Assert.IsTrue(provider.CanUndo);
			Assert.IsTrue(provider.CanRedo);
			Assert.AreEqual("Aaa", provider.UndoDescription);
			Assert.AreEqual("Bbb", provider.RedoDescription);

			provider.ClearCache();

			Assert.IsFalse(provider.CanUndo);
			Assert.IsFalse(provider.CanRedo);
			Assert.AreEqual(string.Empty, provider.UndoDescription);
			Assert.AreEqual(string.Empty, provider.RedoDescription);
		}

		[Test]
		public void TestSuppressUndoRedo()
		{
			var provider = new UndoRedoProvider4Test();
			provider.AddUndoActionExposed("Aaa", () => { }, () => { });
			Assert.AreEqual("Aaa", provider.UndoDescription);

			using (provider.SuppressUndoRegistration())
			{
				provider.AddUndoActionExposed("Bbb", () => { }, () => { });
				Assert.AreEqual("Aaa", provider.UndoDescription);
			}

			provider.AddUndoActionExposed("Bbb", () => { }, () => { });
			Assert.AreEqual("Bbb", provider.UndoDescription);
		}

		[Test]
		public void TestStateChanged()
		{
			var provider = new UndoRedoProvider4Test();
			var counter = 0;

			provider.StateChanged += (sender, e) => counter++;
			Assert.AreEqual(0, counter);

			provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
			Assert.AreEqual(1, counter);

			provider.Undo();
			Assert.AreEqual(2, counter);

			provider.Redo();
			Assert.AreEqual(3, counter);

			using (provider.BeginMultiActionsUndoRedoStep(string.Empty))
			{
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
				Assert.AreEqual(3, counter);
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
				Assert.AreEqual(3, counter);
			}
			Assert.AreEqual(4, counter);

			using (provider.SuppressUndoRegistration())
			{
				provider.AddUndoActionExposed(string.Empty, () => { }, () => { });
			}
			Assert.AreEqual(4, counter);

			provider.ClearCache();
			Assert.AreEqual(5, counter);
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

