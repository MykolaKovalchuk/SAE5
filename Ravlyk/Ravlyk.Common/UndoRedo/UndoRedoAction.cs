using System;

namespace Ravlyk.Common.UndoRedo
{
	/// <summary>
	/// Simple one-operation undo/redo action.
	/// </summary>
	public abstract class UndoRedoAction
	{
		/// <summary>
		/// Default name of undo/redo action.
		/// </summary>
		public abstract string DefaultName { get; }

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public abstract void Undo();

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public abstract void Redo();
	}
}
