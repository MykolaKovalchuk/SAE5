using System;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.UndoRedo
{
	/// <summary>
	/// Undo/redo provider for <see cref="IndexedImage"/> objects.
	/// </summary>
	public class UndoRedoProvider : Ravlyk.Common.UndoRedo.UndoRedoProvider
	{
		/// <summary>
		/// Initializes undo/redo provider without attaching to image object.
		/// </summary>
		public UndoRedoProvider() { }

		/// <summary>
		/// Initializes undo/redo provider and attaches it to image object.
		/// </summary>
		/// <param name="image">Image object to attach to.</param>
		public UndoRedoProvider(CodedImage image)
			: this()
		{
			using (DisposableLock.Lock(ref updateHasChangesLock))
			{
				AttachToImage(image);
			}
		}

		/// <summary>
		/// Attaches it to image object.
		/// </summary>
		/// <param name="image">Image object to attach to.</param>
		public void AttachToImage(CodedImage image)
		{
			if (Image != null)
			{
				Image.PixelChanged -= PixelColorChanged;
				Image.Palette.ColorAdded -= ColorAdded;
				Image.Palette.ColorRemoved -= ColorRemoved;
				Image.Palette.ColorAttributesChanged -= ColorAttributesChanged;
			}

			Image = image;
			ClearCache();

			if (Image != null)
			{
				Image.PixelChanged += PixelColorChanged;
				Image.Palette.ColorAdded += ColorAdded;
				Image.Palette.ColorRemoved += ColorRemoved;
				Image.Palette.ColorAttributesChanged += ColorAttributesChanged;
			}
		}

		/// <summary>
		/// <see cref="IndexedImage"/> object for undo/redo operations observation.
		/// </summary>
		protected CodedImage Image { get; private set; }

		protected override void OnStateChanged()
		{
			UpdateHasChanges();
			base.OnStateChanged();
		}

		void UpdateHasChanges()
		{
			if (!updateHasChangesLock.IsLocked())
			{
				Image.HasChanges = true;
			}
		}

		DisposableLock updateHasChangesLock;

		#region Implementation

		void PixelColorChanged(object sender, PixelChangedEventArgs e)
		{
			if (!UndoRegistrationSuppressed)
			{
				AddUndoAction(new UndoRedoActionChangePixelColor(Image, e.X, e.Y, (CodedColor)e.OldColor, (CodedColor)e.NewColor));
			}
		}

		void ColorAdded(object sender, Palette.ColorChangedEventArgs e)
		{
			if (!UndoRegistrationSuppressed)
			{
				AddUndoAction(new UndoRedoActionAddColor(Image, e.Color));
			}
		}

		void ColorRemoved(object sender, Palette.ColorChangedEventArgs e)
		{
			if (!UndoRegistrationSuppressed)
			{
				AddUndoAction(new UndoRedoActionRemoveColor(Image, e.Color));
			}
		}

		void ColorAttributesChanged(object sender, CodedPalette.ColorAttributesChangedEventArgs e)
		{
			if (!UndoRegistrationSuppressed)
			{
				AddUndoAction(new UndoRedoActionChangeColorAttributes(Image, (CodedColor)e.Color, e.OldSymbol, e.OldCode, e.OldName, e.NewSymbol, e.NewCode, e.NewName));
			}
		}

		#endregion

		#region Action names

		public static string UndoRedoActionFillRegion => Resources.UndoRedoActionFillRegion;
		public static string UndoRedoActionChangeThread => Resources.UndoRedoActionChangeThread;
		public static string UndoRedoActionAddTherad => Resources.UndoRedoActionAddThread;
		public static string UndoRedoActionCut => Resources.UndoRedoActionCut;
		public static string UndoRedoActionPaste => Resources.UndoRedoActionPaste;

		#endregion
	}
}
