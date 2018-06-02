using System;

using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Pixel color change undo/redo operation.
	/// </summary>
	public class UndoRedoActionChangePixelColor : UndoRedoAction
	{
		/// <summary>
		/// Initialized undo/redo action.
		/// </summary>
		/// <param name="image">Image object for undo/redo operation.</param>
		/// <param name="x">Pixel's x-coordinate.</param>
		/// <param name="y">Pixel's y-coordinate.</param>
		/// <param name="oldColor">Pixel's old color.</param>
		/// <param name="newColor">Pixel's new color.</param>
		public UndoRedoActionChangePixelColor(CodedImage image, int x, int y, CodedColor oldColor, CodedColor newColor) : base(image)
		{
			this.x = x;
			this.y = y;
			this.oldColor = oldColor;
			this.newColor = newColor;
		}

		readonly int x;
		readonly int y;
		readonly CodedColor oldColor;
		readonly CodedColor newColor;

		/// <summary>
		/// Default name of undo/redo action.
		/// </summary>
		public override string DefaultName => Resources.UndoRedoActionChangeCellColor;

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public override void Undo()
		{
			Image[x, y] = oldColor;
		}

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public override void Redo()
		{
			Image[x, y] = newColor;
		}
	}
}
