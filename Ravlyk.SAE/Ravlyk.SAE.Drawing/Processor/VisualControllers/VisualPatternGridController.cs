using System;
using System.ComponentModel;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.UndoRedo;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class VisualPatternGridController : VisualScrollableController, INotifyPropertyChanged
	{
		public VisualPatternGridController(IImageProvider imageProvider, TrueTypeFont symbolsFont, IndexedImage whiteCrosses, Size imageBoxSize = default(Size))
			: base(imageProvider, imageBoxSize)
		{
			WhiteCrosses = whiteCrosses;
			SymbolsFont = symbolsFont;
			imageProvider.Image.PixelChanged += Image_PixelChanged;
		}

		#region Properties

		public PatternGridPainter.StitchesPaintMode PaintMode
		{
			get { return GridPainter.PaintMode; }
			set
			{
				if (GridPainter.PaintMode != value)
				{
					GridPainter.PaintMode = value;
					UpdateVisualImage();
					OnPropertyChanged(nameof(PaintMode));
				}
			}
		}

		public int CellSize
		{
			get { return GridPainter.CellSize; }
			set
			{
				if (GridPainter.CellSize != value)
				{
					GridPainter.CellSize = value;
					UpdateParametersAndVisualImage();
					OnPropertyChanged(nameof(CellSize));
				}
			}
		}

		public bool ShowRulers
		{
			get { return GridPainter.ShowRulers; }
			set
			{
				if (GridPainter.ShowRulers != value)
				{
					GridPainter.ShowRulers = value;
					UpdateVisualImage();
					OnPropertyChanged(nameof(ShowRulers));
				}
			}
		}

		public bool ShowLines
		{
			get { return GridPainter.ShowLines; }
			set
			{
				if (GridPainter.ShowLines != value)
				{
					GridPainter.ShowLines = value;
					UpdateVisualImage();
					OnPropertyChanged(nameof(ShowLines));
				}
			}
		}

		public Rectangle SelectedRect
		{
			get { return GridPainter.SelectedRect; }
			set
			{
				if (!GridPainter.SelectedRect.Equals(value))
				{
					GridPainter.SelectedRect = value;
					UpdateVisualImage();
					OnPropertyChanged(nameof(SelectedRect));
				}
			}
		}

		bool ShowSelectedRect
		{
			get { return GridPainter.ShowSelectedRect; }
			set
			{
				if (value != GridPainter.ShowSelectedRect)
				{
					GridPainter.ShowSelectedRect = value;
					if (value)
					{
						SelectedRect = default(Rectangle);
					}
					UpdateVisualImage();
				}
			}
		}

		#endregion

		#region Visual Image

		internal int VisibleColumns { get; private set; }
		internal int VisibleColumnsWidth { get; private set; }
		internal int VisibleRows { get; private set; }
		internal int VisibleRowsHeight { get; private set; }
		internal bool RequiresShift { get; private set; }

		protected override void UpdateParameters()
		{
			base.UpdateParameters();

			VisibleColumns = (ImageBoxSize.Width - GridPainter.RulerWidth) / CellSize;
			VisibleColumnsWidth = VisibleColumns * CellSize;

			VisibleRows = (ImageBoxSize.Height - GridPainter.RulerWidth) / CellSize;
			VisibleRowsHeight = VisibleRows * CellSize;

			RequiresShift = VisibleColumns < SourceImage.Size.Width || VisibleRows < SourceImage.Size.Height;

			PixelsShift = PixelsShift;
		}

		protected override CodedImage CreateVisualImage()
		{
			return new CodedImage { Size = ImageBoxSize };
		}

		protected override void UpdateVisualImageCore()
		{
			if (SymbolsFont != null && CellSize > 0)
			{
				VisualImage.Size = ImageBoxSize;

				Painter.Canvas = VisualImage;
				using (Painter.Clip(new Rectangle(0, 0, ImageBoxSize.Width, ImageBoxSize.Height)))
				{
					GridPainter.Paint(Painter, VisualImage.Size, Painter.ClipRect, new Point(CellsShift.Width, CellsShift.Height));
				}
			}
		}

		#region Painter

		public TrueTypeFont SymbolsFont { get; }

		public IndexedImage WhiteCrosses
		{
			get { return GridPainter.WhiteCrosses; }
			set { GridPainter.WhiteCrosses = value; }
		}

		public PatternGridPainter GridPainter
		{
			get
			{
				if (gridPainter == null)
				{
					gridPainter = new PatternGridPainter(SourceImage, SymbolsFont);
				}
				else if (SymbolsFont != null && gridPainter.SymbolsFont != SymbolsFont)
				{
					gridPainter.SymbolsFont = SymbolsFont;
				}

				return gridPainter;
			}
		}
		PatternGridPainter gridPainter;

		IndexedImagePainter Painter
		{
			get
			{
				if (painter == null)
				{
					painter = new IndexedImagePainter();
				}
				painter.SymbolsFont = SymbolsFont;
				return painter;
			}
		}
		IndexedImagePainter painter;

		#endregion

		#endregion

		#region Drawing

		public enum MouseActionMode
		{
			Shift,
			Pen,
			Fill,
			Select,
			MoveSelection
		}

		public MouseActionMode MouseMode
		{
			get { return mouseMode; }
			set
			{
				if (value == mouseMode)
				{
					return;
				}

				var oldMouseMode = mouseMode;
				mouseMode = value;

				using (SuspendUpdateVisualImage())
				{
					if (oldMouseMode == MouseActionMode.MoveSelection)
					{
						FinishMoveSelection();
					}

					ShowSelectedRect = mouseMode == MouseActionMode.Select || mouseMode == MouseActionMode.MoveSelection;
					GridPainter.ShowSelectedRectPoints = mouseMode == MouseActionMode.Select;
					GridPainter.AllowExceedingSelection = mouseMode == MouseActionMode.MoveSelection;
					if (mouseMode == MouseActionMode.Select)
					{
						SelectedRect = default(Rectangle);
					}
					if (mouseMode != MouseActionMode.MoveSelection)
					{
						GridPainter.InsertedBlock = null;
					}
				}

				OnPropertyChanged(nameof(MouseMode));
			}
		}
		MouseActionMode mouseMode = MouseActionMode.Shift;

		public CodedColor MouseColor { get; set; }

		public void Pen(Point point)
		{
			if (MouseColor != null && point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
			{
				using (SuspendUpdateVisualImage())
				using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
				{
					SourceImage[point.X, point.Y] = MouseColor;
					if (!suspendImageChangedEventOnTouch)
					{
						SourceImage.TriggerImageChanged();
					}
				}
			}
		}

		public void Fill(Point point)
		{
			if (MouseColor != null && point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
			{
				using (SuspendUpdateVisualImage())
				using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
				using (UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionFillRegion))
				{
					ImagePainter.Fill(SourceImage, MouseColor, point.X, point.Y);
					SourceImage.TriggerImageChanged();
				}
			}
		}

		void Image_PixelChanged(object sender, PixelChangedEventArgs e)
		{
			UpdateVisualImage();
		}

		#region Move Selection

		Point initialTouchImagePoint;

		public void InsertBlockAndBeginMoveSelection(CodedImage block)
		{
			using (SuspendUpdateVisualImage())
			{
				if (MouseMode == MouseActionMode.MoveSelection)
				{
					FinishMoveSelection();
				}

				GridPainter.InsertedBlock = block;
				MouseMode = MouseActionMode.MoveSelection;
				SelectedRect = new Rectangle(CellsShift.Width, CellsShift.Height, block.Size.Width, block.Size.Height);
				UpdateVisualImage();
			}
		}

		void MoveSelection(Size shift)
		{
			SelectedRect = new Rectangle(
				Math.Max(-SelectedRect.Width + 1, Math.Min(SourceImage.Size.Width - 1, SelectedRect.Left + shift.Width)),
				Math.Max(-SelectedRect.Height + 1, Math.Min(SourceImage.Size.Height - 1, SelectedRect.Top + shift.Height)),
				SelectedRect.Width,
				SelectedRect.Height);
		}

		public void FinishMoveSelection()
		{
			if (GridPainter.InsertedBlock != null)
			{
				using (SuspendUpdateVisualImage())
				using (SourceImage.Palette.SuppressRemoveColorsWithoutOccurrences())
				using (UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionPaste))
				{
					ImageCopier.CopyWithPalette(GridPainter.InsertedBlock, SourceImage, new Point(SelectedRect.Left, SelectedRect.Top));

					GridPainter.InsertedBlock = null;

					if (MouseMode == MouseActionMode.MoveSelection)
					{
						MouseMode = MouseActionMode.Shift;
					}

					UpdateVisualImage();
					SourceImage.TriggerImageChanged();
				}
			}
		}

		#endregion

		#endregion

		#region UndoRedo

		public UndoRedoProvider UndoRedo
		{
			get
			{
				if (undoRedo == null)
				{
					undoRedo = new UndoRedoProvider(SourceImage);
				}
				return undoRedo;
			}
		}
		UndoRedoProvider undoRedo;

		public void Undo()
		{
			using (SuspendUpdateVisualImage())
			{
				if (MouseMode == MouseActionMode.MoveSelection)
				{
					FinishMoveSelection();
				}

				if (UndoRedo.CanUndo)
				{
					UndoRedo.Undo();
					SourceImage.TriggerImageChanged();
				}
			}
		}

		public void Redo()
		{
			using (SuspendUpdateVisualImage())
			{
				if (MouseMode == MouseActionMode.MoveSelection)
				{
					FinishMoveSelection();
				}

				if (UndoRedo.CanRedo)
				{
					UndoRedo.Redo();
					SourceImage.TriggerImageChanged();
				}
			}
		}

		#endregion

		#region Touch actions

		public Size CellsShift
		{
			get { return cellsShift; }
			private set
			{
				if (value.Width < 0) { value.Width = 0; }
				if (value.Height < 0) { value.Height = 0; }

				var maxHorizontalShift = SourceImage.Size.Width - VisibleColumns;
				if (value.Width > maxHorizontalShift && maxHorizontalShift > 0) { value.Width = maxHorizontalShift; }

				var maxVerticalShift = SourceImage.Size.Height - VisibleRows;
				if (value.Height > maxVerticalShift && maxVerticalShift > 0) { value.Height = maxVerticalShift; }

				if (value != cellsShift)
				{
					var oldCellShift = cellsShift;
					cellsShift = value;
					var shiftDelta = new Size(cellsShift.Width - oldCellShift.Width, cellsShift.Height - oldCellShift.Height);

					if (!synchronizingShifts)
					{
						synchronizingShifts = true;
						PixelsShift = new Size(value.Width * CellSize, value.Height * CellSize);
						synchronizingShifts = false;
					}

					if (!IsUpdateVisualImageSuspended && Math.Abs(shiftDelta.Width) < VisibleColumns && Math.Abs(shiftDelta.Height) < VisibleRows)
					{
						GridPainter.ShiftImageAndUpdateRest(Painter, VisualImage, new Point(CellsShift.Width, CellsShift.Height), shiftDelta);
						OnVisualImageChanged();
					}
					else
					{
						UpdateVisualImage();
					}
				}
			}
		}
		Size cellsShift;

		public Size PixelsShift
		{
			get { return pixelsShift; }
			set
			{
				if (value.Width < 0) { value.Width = 0; }
				if (value.Height < 0) { value.Height = 0; }

				var maxHorizontalShift = SourceImage.Size.Width * CellSize - VisibleColumnsWidth;
				if (value.Width > maxHorizontalShift) { value.Width = maxHorizontalShift; }

				var maxVerticalShift = SourceImage.Size.Height * CellSize - VisibleRowsHeight;
				if (value.Height > maxVerticalShift) { value.Height = maxVerticalShift; }

				if (value != pixelsShift)
				{
					pixelsShift = value;
					if (!synchronizingShifts)
					{
						synchronizingShifts = true;
						CellsShift = new Size(value.Width / CellSize, value.Height / CellSize);
						synchronizingShifts = false;
					}
				}
			}
		}
		Size pixelsShift;
		bool synchronizingShifts;

		Point touchedCell;
		TouchPointerStyle touchedPointerStyle;
		VisualZoomCropController.TouchPoint touchedPoint;

		protected override void OnTouchedCore(Point imagePoint)
		{
			if (MouseMode == MouseActionMode.Shift)
			{
				base.OnTouchedCore(imagePoint);
			}
			else if (MouseMode == MouseActionMode.Pen)
			{
				suspendImageChangedEventOnTouch = true;
				Pen(CellFromImagePoint(imagePoint));
			}
			else if (MouseMode == MouseActionMode.Fill)
			{
				Fill(CellFromImagePoint(imagePoint));
			}
			else if (MouseMode == MouseActionMode.Select)
			{
				touchedPointerStyle = GetTouchPointerStyleCore(imagePoint); // Should be saved calculated first
				touchedPoint = GetTouchPoint(imagePoint);
				var point = touchedCell = CellFromImagePoint(imagePoint);
				if (point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
				{
					if (touchedPoint == VisualZoomCropController.TouchPoint.None || SelectedRect.Width == 0 || SelectedRect.Height == 0)
					{
						SelectedRect = new Rectangle(point.X, point.Y, 1, 1);
					}
				}
			}
			else if (MouseMode == MouseActionMode.MoveSelection)
			{
				var cellPoint = CellFromImagePoint(imagePoint);
				if (!SelectedRect.ContainsPoint(cellPoint))
				{
					FinishMoveSelection();
				}
				else
				{
					initialTouchImagePoint = cellPoint;
				}
			}
		}

		protected override void OnShiftCore(Point imagePoint, Size shiftSize)
		{
			if (IsTouching)
			{
				if (MouseMode == MouseActionMode.Shift && RequiresShift)
				{
					PixelsShift = new Size(PixelsShift.Width - shiftSize.Width, PixelsShift.Height - shiftSize.Height);
				}
				else if (MouseMode == MouseActionMode.Pen)
				{
					Pen(CellFromImagePoint(new Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height)));
				}
				else if (MouseMode == MouseActionMode.Select)
				{
					var newPoint = CellFromImagePoint(new Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height));
					var x1 = SelectedRect.Left;
					var y1 = SelectedRect.Top;
					var x2 = SelectedRect.RightExclusive - 1;
					var y2 = SelectedRect.BottomExclusive - 1;

					switch (touchedPoint)
					{
						case VisualZoomCropController.TouchPoint.Left:
							x1 = newPoint.X;
							break;
						case VisualZoomCropController.TouchPoint.Right:
							x2 = newPoint.X;
							break;
						case VisualZoomCropController.TouchPoint.Top:
							y1 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.Bottom:
							y2 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.LeftTop:
							x1 = newPoint.X;
							y1 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.RightTop:
							x2 = newPoint.X;
							y1 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.LeftBottom:
							x1 = newPoint.X;
							y2 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.RightBottom:
							x2 = newPoint.X;
							y2 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.None:
							x1 = touchedCell.X;
							y1 = touchedCell.Y;
							x2 = newPoint.X;
							y2 = newPoint.Y;
							break;
						case VisualZoomCropController.TouchPoint.Middle:
							x1 = SelectedRect.Left + newPoint.X - touchedCell.X;
							y1 = SelectedRect.Top + newPoint.Y - touchedCell.Y;
							x2 = x1 + SelectedRect.Width - 1;
							y2 = y1 + SelectedRect.Height - 1;
							touchedCell = newPoint;
							break;
					}

					if (x1 > x2)
					{
						var t = x1;
						x1 = x2;
						x2 = t;
					}
					if (y1 > y2)
					{
						var t = y1;
						y1 = y2;
						y2 = t;
					}
					SelectedRect = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
				}
				else if (MouseMode == MouseActionMode.MoveSelection)
				{
					var newPoint = CellFromImagePoint(new Point(imagePoint.X + shiftSize.Width, imagePoint.Y + shiftSize.Height));
					MoveSelection(new Size(newPoint.X - initialTouchImagePoint.X, newPoint.Y - initialTouchImagePoint.Y));
					initialTouchImagePoint = newPoint;
				}
			}
			else
			{
				base.OnShiftCore(imagePoint, shiftSize);
			}
		}

		protected override void OnUntouchedCore(Point imagePoint)
		{
			touchedPointerStyle = TouchPointerStyle.None;
			touchedPoint = VisualZoomCropController.TouchPoint.None;
			touchedCell = new Point(-1, -1);

			if (suspendImageChangedEventOnTouch)
			{
				SourceImage.TriggerImageChanged();
				suspendImageChangedEventOnTouch = false;
			}
			base.OnUntouchedCore(imagePoint);
		}

		Point CellFromImagePoint(Point imagePoint)
		{
			var rulerWidth = GridPainter.RulerWidth;
			return new Point(
				imagePoint.X >= rulerWidth ? (imagePoint.X - rulerWidth) / CellSize + CellsShift.Width : -1,
				imagePoint.Y >= rulerWidth ? (imagePoint.Y - rulerWidth) / CellSize + CellsShift.Height : -1);
		}

		Point ImageFromCellPoint(Point cellPoint)
		{
			var rulerWidth = GridPainter.RulerWidth;
			return new Point(
				cellPoint.X >= 0 && cellPoint.X < SourceImage.Size.Width ? (cellPoint.X - CellsShift.Width) * CellSize + rulerWidth : -1,
				cellPoint.Y >= 0 && cellPoint.Y < SourceImage.Size.Height ? (cellPoint.Y - CellsShift.Height) * CellSize + rulerWidth : -1);
		}

		protected override void OnMouseWheelCore(int delta)
		{
			if (RequiresShift)
			{
				PixelsShift = new Size(PixelsShift.Width, PixelsShift.Height - delta);
			}
			else
			{
				base.OnMouseWheelCore(delta);
			}
		}

		bool suspendImageChangedEventOnTouch;

		protected override TouchPointerStyle GetTouchPointerStyleCore(Point imagePoint)
		{
			var point = CellFromImagePoint(imagePoint);
			if (point.X >= 0 && point.X < SourceImage.Size.Width && point.Y >= 0 && point.Y < SourceImage.Size.Height)
			{
				if (MouseMode == MouseActionMode.MoveSelection)
				{
					if (SelectedRect.ContainsPoint(point))
					{
						return TouchPointerStyle.Shift;
					}
				}
				else if (MouseMode == MouseActionMode.Select)
				{
					if (IsTouching)
					{
						return touchedPointerStyle;
					}

					if (SelectedRect.Width > 0 && SelectedRect.Height > 0)
					{
						var imageRectLeftTop = ImageFromCellPoint(new Point(SelectedRect.Left, SelectedRect.Top));
						var imageRectRightBottom = ImageFromCellPoint(new Point(SelectedRect.RightExclusive, SelectedRect.BottomExclusive));

						if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, imageRectLeftTop.Y) ||
							IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, imageRectRightBottom.Y))
						{
							return TouchPointerStyle.ResizeLeftTop_RightBottom;
						}
						if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, imageRectRightBottom.Y) ||
							IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, imageRectLeftTop.Y))
						{
							return TouchPointerStyle.ResizeRightTop_LeftBottom;
						}

						var midX = (imageRectLeftTop.X + imageRectRightBottom.X) / 2;
						var midY = (imageRectLeftTop.Y + imageRectRightBottom.Y) / 2;

						if (IsInsideRadius(imagePoint.X, imagePoint.Y, midX, imageRectLeftTop.Y) ||
							IsInsideRadius(imagePoint.X, imagePoint.Y, midX, imageRectRightBottom.Y))
						{
							return TouchPointerStyle.ResizeVertical;
						}
						if (IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectLeftTop.X, midY) ||
							IsInsideRadius(imagePoint.X, imagePoint.Y, imageRectRightBottom.X, midY))
						{
							return TouchPointerStyle.ResizeHorizontal;
						}

						if (SelectedRect.ContainsPoint(point))
						{
							return TouchPointerStyle.ResizeAll;
						}
					}

					return TouchPointerStyle.Cross;
				}
			}

			return base.GetTouchPointerStyleCore(imagePoint);
		}

		VisualZoomCropController.TouchPoint GetTouchPoint(Point imagePoint)
		{
			if (MouseMode != MouseActionMode.Select && MouseMode != MouseActionMode.MoveSelection)
			{
				return VisualZoomCropController.TouchPoint.None;
			}

			var imageRectLeftTop = ImageFromCellPoint(new Point(SelectedRect.Left, SelectedRect.Top));
			var imageRectRightBottom = ImageFromCellPoint(new Point(SelectedRect.RightExclusive, SelectedRect.BottomExclusive));

			return VisualZoomCropController.GetTouchPoint(imagePoint, GetTouchPointerStyleCore(imagePoint),
				new Rectangle(imageRectLeftTop.X, imageRectLeftTop.Y, imageRectRightBottom.X - imageRectLeftTop.X, imageRectRightBottom.Y - imageRectLeftTop.Y));
		}

		static bool IsInsideRadius(int x1, int y1, int x2, int y2, int radius = 3)
		{
			return VisualZoomCropController.IsInsideRadius(x1, x2, radius) && VisualZoomCropController.IsInsideRadius(y1, y2, radius);
		}

		#endregion

		#region VisualScrollableController

		public override bool ShowVScroll => RequiresShift;

		public override int MaxVSteps => SourceImage.Size.Height;

		public override int BigVStep => VisibleRows;

		public override int VPosition
		{
			get { return CellsShift.Height; }
			set { CellsShift = new Size(CellsShift.Width, value); }
		}

		public override bool ShowHScroll => RequiresShift;

		public override int MaxHSteps => SourceImage.Size.Width;

		public override int BigHStep => VisibleColumns;

		public override int HPosition
		{
			get { return CellsShift.Width; }
			set { CellsShift = new Size(value, CellsShift.Height); }
		}

		#endregion

		#region INotifyPropertyChanged

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
