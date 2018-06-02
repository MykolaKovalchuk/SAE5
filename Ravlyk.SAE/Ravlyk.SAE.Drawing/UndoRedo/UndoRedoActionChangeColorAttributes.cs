using System;

using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Change color attributes undo/redo action.
	/// </summary>
	public class UndoRedoActionChangeColorAttributes : UndoRedoAction
	{
		/// <summary>
		/// Initializes undo/redo action.
		/// </summary>
		/// <param name="image">Image object for undo/redo operation.</param>
		/// <param name="color">Chaged color object.</param>
		/// <param name="oldSymbol">Color old symbol.</param>
		/// <param name="oldCode">Color old code.</param>
		/// <param name="oldName">Color old name.</param>
		/// <param name="newSymbol">Color new symbol.</param>
		/// <param name="newCode">Color new code.</param>
		/// <param name="newName">Color new name.</param>
		public UndoRedoActionChangeColorAttributes(CodedImage image, CodedColor color, char oldSymbol, string oldCode, string oldName, char newSymbol, string newCode, string newName)
			: base(image)
		{
			this.color = color;
			this.oldSymbol = oldSymbol;
			this.oldCode = oldCode;
			this.oldName = oldName;
			this.newSymbol = newSymbol;
			this.newCode = newCode;
			this.newName = newName;
		}

		readonly CodedColor color;
		readonly char oldSymbol;
		readonly string oldCode;
		readonly string oldName;
		readonly char newSymbol;
		readonly string newCode;
		readonly string newName;

		/// <summary>
		/// Default name of undo/redo action.
		/// </summary>
		public override string DefaultName => Resources.UndoRedoActionChangeColorSymbol;

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public override void Undo()
		{
			foreach (CodedColor c in Image.Palette)
			{
				if (c.Equals(color))
				{
					c.SymbolChar = oldSymbol;
					c.ColorCode = oldCode;
					c.ColorName = oldName;
				}
			}
		}

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public override void Redo()
		{
			foreach (CodedColor c in Image.Palette)
			{
				if (c.Equals(color))
				{
					c.SymbolChar = newSymbol;
					c.ColorCode = newCode;
					c.ColorName = newName;
				}
			}
		}
	}
}
