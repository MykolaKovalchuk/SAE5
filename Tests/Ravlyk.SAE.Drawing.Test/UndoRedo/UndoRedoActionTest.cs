using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.UndoRedo.Test
{
	[TestFixture]
	public class UndoRedoActionTest
	{
		[Test]
		public void TestImage()
		{
			var image = new CodedImage();
			var action = new DummyUndoRedoAction(image);
			Assert.AreSame(image, action.ImageExposed);
		}

		#region Test classes

		class DummyUndoRedoAction : UndoRedoAction
		{
			public DummyUndoRedoAction(CodedImage image) : base(image) { }

			public CodedImage ImageExposed
			{
				get { return Image; }
			}

			public override string DefaultName
			{
				get { throw new NotImplementedException(); }
			}

			public override void Undo()
			{
				throw new NotImplementedException();
			}

			public override void Redo()
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

