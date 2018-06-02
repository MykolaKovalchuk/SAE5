using System;

using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Undo/Redo action for <see cref="IndexedImage"/> objects.
	/// </summary>
	public abstract class UndoRedoAction : Ravlyk.Common.UndoRedo.UndoRedoAction
	{
		/// <summary>
		/// Initializes undo/redo action.
		/// </summary>
		/// <param name="image"><see cref="IndexedImage"/> object.</param>
		protected UndoRedoAction(CodedImage image)
		{
			Image = image;
		}

		/// <summary>
		/// <see cref="IndexedImage"/> object for undo/redo operation.
		/// </summary>
		protected CodedImage Image { get; private set; }
	}
}
