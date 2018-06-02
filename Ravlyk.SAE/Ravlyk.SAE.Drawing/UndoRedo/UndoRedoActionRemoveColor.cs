using System;

using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Remove color undo/redo action.
	/// </summary>
	public class UndoRedoActionRemoveColor : UndoRedoAction
	{
		/// <summary>
		/// Initialize undo/redo action.
		/// </summary>
		/// <param name="image">Image object for undo/redo operation.</param>
		/// <param name="color">Removed color object.</param>
		public UndoRedoActionRemoveColor(CodedImage image, Color color) : base(image)
		{
			this.color = color;
		}

		readonly Color color;

		/// <summary>
		/// Default name of undo/redo action.
		/// </summary>
		public override string DefaultName => Resources.UndoRedoActionRemoveThread;

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public override void Undo()
		{
			Image.Palette.Add(color);
		}

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public override void Redo()
		{
			Image.Palette.Remove(color.GetHashCode());
		}
	}
}
