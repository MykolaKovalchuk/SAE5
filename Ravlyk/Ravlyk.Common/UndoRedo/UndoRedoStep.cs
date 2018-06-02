using System;
using System.Collections.Generic;

namespace Ravlyk.Common.UndoRedo
{
	/// <summary>
	/// Multi-action undo/redo step.
	/// </summary>
	public class UndoRedoStep
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UndoRedoStep"/> class. 
		/// </summary>
		/// <param name="name">Name of undo/redo operation.</param>
		public UndoRedoStep(string name)
		{
			actions = new List<UndoRedoAction>();
			Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UndoRedoStep"/> class with one <see cref="UndoRedoAction"/>. 
		/// </summary>
		/// <param name="action">Simple undo/redo action.</param>
		public UndoRedoStep(UndoRedoAction action) : this(action.DefaultName)
		{
			AddAction(action);
		}

		readonly List<UndoRedoAction> actions;

		/// <summary>
		/// Name of undo/redo operation.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Adds additional undo/redo action to this undo/redo step.
		/// </summary>
		/// <param name="action">Simple undo/redo action.</param>
		public void AddAction(UndoRedoAction action)
		{
			actions.Add(action);
		}

		/// <summary>
		/// Executes undo operations.
		/// </summary>
		public void Undo()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
			{
				actions[i].Undo();
			}
		}

		/// <summary>
		/// Executes redo operations.
		/// </summary>
		public void Redo()
		{
			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Redo();
			}
		}
	}
}
