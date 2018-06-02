using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ravlyk.Common.UndoRedo
{
	/// <summary>
	/// Base undo/redo provider.
	/// </summary>
	public class UndoRedoProvider
	{
		/// <summary>
		/// Checks if there are operation for undo.
		/// </summary>
		public bool CanUndo => undoSteps.Count > 0;

		/// <summary>
		/// Checks if there are operations for redo.
		/// </summary>
		public bool CanRedo => redoSteps.Count > 0;

		/// <summary>
		/// Provides text description for first available undo operation.
		/// </summary>
		public string UndoDescription => CanUndo ? undoSteps.Peek().Name : string.Empty;

		/// <summary>
		/// Provides text description for first available redo operation.
		/// </summary>
		public string RedoDescription => CanRedo ? redoSteps.Peek().Name : string.Empty;

		/// <summary>
		/// Executes undo operation.
		/// </summary>
		public void Undo()
		{
			Debug.Assert(CanUndo, "There is nothing to undo");

			using (SuppressUndoRegistration())
			{
				var undoRedoStep = undoSteps.Pop();
				undoRedoStep.Undo();
				AddRedoStep(undoRedoStep);
			}
		}

		/// <summary>
		/// Executes redo operation.
		/// </summary>
		public void Redo()
		{
			Debug.Assert(CanRedo, "There is nothing to redo");

			using (SuppressUndoRegistration())
			{
				var undoRedoStep = redoSteps.Pop();
				undoRedoStep.Redo();
				AddUndoStep(undoRedoStep);
			}
		}

		/// <summary>
		/// Clears undo and redo caches.
		/// </summary>
		public void ClearCache()
		{
			undoSteps.Clear();
			redoSteps.Clear();
			OnStateChanged();
		}

		protected virtual void OnStateChanged()
		{
			StateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Fires when undo/redo state (operations) is changed.
		/// </summary>
		public event EventHandler StateChanged;

		#region MultiActions steps

		/// <summary>
		/// Current multi-action undo step.
		/// </summary>
		protected UndoRedoStep CurrentStep { get; private set; }

		/// <summary>
		/// Starts new multi-action undo/redo step.
		/// </summary>
		/// <param name="name">Name of undo/redo step.</param>
		/// <returns>Temporary disposable object to dispose when step is finished.</returns>
		public IDisposable BeginMultiActionsUndoRedoStep(string name)
		{
			Debug.Assert(CurrentStep == null, "It is not possible to begin new undo-redo step until finish current.");

			CurrentStep = new UndoRedoStep(name);
			return new DisposableAction(
				() =>
				{
					AddUndoStep(CurrentStep);
					CurrentStep = null;
				});
		}

		#endregion

		#region Suppression

		/// <summary>
		/// Checks if undo/redo observation is currently suppressed.
		/// </summary>
		protected bool UndoRegistrationSuppressed => undoRegistrationSuppressionIndex > 0;

		int undoRegistrationSuppressionIndex;

		/// <summary>
		/// Suppress undo/redo observation.
		/// </summary>
		/// <returns>Temporary disposable object to dispose when undo/redo observation should be restored.</returns>
		public IDisposable SuppressUndoRegistration()
		{
			undoRegistrationSuppressionIndex++;
			return new DisposableAction(() => undoRegistrationSuppressionIndex--);
		}

		#endregion

		#region Implementation

		readonly Stack<UndoRedoStep> undoSteps = new Stack<UndoRedoStep>();
		readonly Stack<UndoRedoStep> redoSteps = new Stack<UndoRedoStep>();

		/// <summary>
		/// Adds new undo action to undo stack of current multi-action undo step.
		/// </summary>
		/// <param name="action">Undo action to add.</param>
		protected void AddUndoAction(UndoRedoAction action)
		{
			if (!UndoRegistrationSuppressed)
			{
				redoSteps.Clear();

				if (CurrentStep == null)
				{
					AddUndoStep(new UndoRedoStep(action));
				}
				else
				{
					CurrentStep.AddAction(action);
				}
			}
		}

		/// <summary>
		/// Adds undo step to undo stack.
		/// </summary>
		/// <param name="step">Undo step to add.</param>
		protected void AddUndoStep(UndoRedoStep step)
		{
			undoSteps.Push(step);
			OnStateChanged();
		}

		/// <summary>
		/// Adds redo step to redo stack.
		/// </summary>
		/// <param name="step">Redo step to add.</param>
		protected void AddRedoStep(UndoRedoStep step)
		{
			redoSteps.Push(step);
			OnStateChanged();
		}

		#endregion
	}
}
