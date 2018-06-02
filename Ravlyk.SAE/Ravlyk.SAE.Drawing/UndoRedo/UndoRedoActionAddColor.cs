using System;

using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Add color undo/redo action.
	/// </summary>
	public class UndoRedoActionAddColor : UndoRedoAction
	{
		/// <summary>
		/// Initialize undo/redo action.
		/// </summary>
		/// <param name="image">Image object for undo/redo operation.</param>
		/// <param name="color">Added color object.</param>
		public UndoRedoActionAddColor(CodedImage image, Color color) : base(image)
		{
			this.color = color;
		}

		readonly Color color;

		/// <summary>
		/// Default name of undo/redo action.
		/// </summary>
		public override string DefaultName => Resources.UndoRedoActionAddThread;

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public override void Undo()
		{
			Image.Palette.Remove(color.GetHashCode());
		}

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public override void Redo()
		{
			Image.Palette.Add(color);
		}
	}
}
