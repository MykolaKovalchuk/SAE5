using System;
using System.Threading.Tasks;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class VisualSymbolsController : VisualScrollableController
	{
		public VisualSymbolsController(ImageSymbolsController controller, Size imageBoxSize = default(Size))
			: base(controller.Manipulator, imageBoxSize)
		{
			using (SuspendUpdateVisualImage())
			{
				Controller = controller;
				CellSize = 32;
				HighlightedCell = new Point(-1, -1);

				PlainBackground = ColorBytes.ToArgb(0, 255, 255, 255);
				PlainFontColor = ColorBytes.ToArgb(0, 0, 0, 0);
				SelectedBackground = ColorBytes.ToArgb(0, 255, 127, 80);
				SelectedFontColor = PlainFontColor;
				HighlightedPlainBackground = ColorBytes.ToArgb(0, 0, 149, 237);
				HighlightedPlainFontColor = ColorBytes.ToArgb(0, 255, 255, 255);
				HighlightedSelectedBackground = ColorBytes.ToArgb(0, 0, 128, 255);
				HighlightedSelectedFontColor = HighlightedPlainFontColor;

				UpdateParametersAndVisualImage();
			}
		}

		public ImageSymbolsController Controller { get; }

		public int CellSize
		{
			get { return cellSize; }
			set
			{
				cellSize = value;
				UpdateParametersAndVisualImage();
			}
		}
		int cellSize;

		#region Visual image

		internal int ColumnsCount { get; private set; }
		internal int RowsCount { get; private set; }
		internal int VisibleRowsCount { get; private set; }
		internal bool RequiresShift { get; private set; }

		protected override void UpdateParameters()
		{
			base.UpdateParameters();

			if (cellSize == 0)
			{
				return;
			}

			ColumnsCount = Math.Min(ImageBoxSize.Width / CellSize, Controller.AvailableSymbols.Count);
			RowsCount = (Controller.AvailableSymbols.Count + ColumnsCount - 1) / ColumnsCount;
			VisibleRowsCount = Math.Min((ImageBoxSize.Height + CellSize - 1) / CellSize, RowsCount);
			RequiresShift = RowsCount * CellSize > ImageBoxSize.Height;

			VerticalShift = VerticalShift;
		}

		protected override CodedImage CreateVisualImage()
		{
			return new CodedImage { Size = new Size(1, 1) };
		}

		protected override void UpdateVisualImageCore()
		{
			if (cellSize == 0)
			{
				return;
			}

			VisualImage.Size = new Size(ColumnsCount * CellSize, Math.Min(VisibleRowsCount * CellSize, ImageBoxSize.Height));

			var firstVisibleSymbol = (VerticalShift / CellSize) * ColumnsCount;
			var lastVisibleSymbolExclusive = ((ImageBoxSize.Height + VerticalShift + CellSize - 1) / CellSize) * ColumnsCount;

			UpdateImageQuick(firstVisibleSymbol, lastVisibleSymbolExclusive);
		}

		void UpdateImageQuick(int firstSymbol, int lastSymbolExclusive)
		{
			var clipRect = new Rectangle(0, 0, VisualImage.Size.Width, VisualImage.Size.Height);

			int[] imagePixels;
			using (VisualImage.LockPixels(out imagePixels))
			{
				Parallel.For(firstSymbol, lastSymbolExclusive,
					index =>
					{
						var cellX = index % ColumnsCount;
						var cellY = index / ColumnsCount;
						var point = new Point(cellX * CellSize, cellY * CellSize - VerticalShift);

						if (index >= Controller.AvailableSymbols.Count)
						{
							SymbolsPainter.PaintSymbol(' ', imagePixels, VisualImage.Size, point, clipRect, PlainFontColor, PlainBackground);
						}
						else
						{
							var symbolSelectionPair = Controller.AvailableSymbols[index];

							var highlight = cellX == HighlightedCell.X && cellY == HighlightedCell.Y;
							int background, fontColor;
							if (symbolSelectionPair.Value)
							{
								background = highlight ? HighlightedSelectedBackground : SelectedBackground;
								fontColor = highlight ? HighlightedSelectedFontColor : SelectedFontColor;
							}
							else
							{
								background = highlight ? HighlightedPlainBackground : PlainBackground;
								fontColor = highlight ? HighlightedPlainFontColor : PlainFontColor;
							}

							SymbolsPainter.PaintSymbol(symbolSelectionPair.Key, imagePixels, VisualImage.Size, point, clipRect, fontColor, background);
						}
					});
			}
		}

		void UpdateCell(Point cell)
		{
			if (cell.X >= 0 && cell.Y >= 0)
			{
				var symbolIndex = cell.Y * ColumnsCount + cell.X;
				if (symbolIndex < Controller.AvailableSymbols.Count)
				{
					UpdateImageQuick(symbolIndex, symbolIndex + 1);
				}
			}
		}

		void ShiftImageAndUpdateRest(int shiftDelta)
		{
			ImageShifter.ShiftPixels(VisualImage, 0, -shiftDelta);

			int firstDirtyRow, lastDirtyRow;
			if (shiftDelta < 0) // Shifting down - repaint rows at top
			{
				firstDirtyRow = VerticalShift / CellSize;
				lastDirtyRow = (VerticalShift - shiftDelta) / CellSize;
			}
			else // Shifting up - repaint rows at bottom
			{
				firstDirtyRow = (VerticalShift - shiftDelta + VisualImage.Size.Height - 1) / CellSize;
				lastDirtyRow = (VerticalShift + VisualImage.Size.Height - 1) / CellSize;
			}

			UpdateImageQuick(firstDirtyRow * ColumnsCount, (lastDirtyRow + 1) * ColumnsCount);
		}

		#region Symbols painters

		CachedSymbolPainter SymbolsPainter
		{
			get
			{
				if (symbolsPainter == null || symbolsPainter.Font != Controller.SymbolsFont || symbolsPainter.PixelHeight != CellSize)
				{
					symbolsPainter = new CachedSymbolPainter(Controller.SymbolsFont, CellSize);
				}
				return symbolsPainter;
			}
		}
		CachedSymbolPainter symbolsPainter;

		public int PlainBackground { get; set; }
		public int SelectedBackground { get; set; }
		public int HighlightedPlainBackground { get; set; }
		public int HighlightedSelectedBackground { get; set; }
		public int PlainFontColor { get; set; }
		public int HighlightedSelectedFontColor { get; set; }
		public int HighlightedPlainFontColor { get; set; }
		public int SelectedFontColor { get; set; }

		#endregion

		#endregion

		#region Touch actions

		protected override Point TranslatePoint(Point controllerPoint)
		{
			return new Point(
				controllerPoint.X - (ImageBoxSize.Width - VisualImage.Size.Width) / 2,
				controllerPoint.Y - (ImageBoxSize.Height - VisualImage.Size.Height) / 2);
		}

		internal Point HighlightedCell
		{
			get { return highlightedCell; }
			private set
			{
				if (highlightedCell != value)
				{
					var oldHighlightedCell = highlightedCell;
					highlightedCell = value;

					if (!IsUpdateVisualImageSuspended)
					{
						UpdateCell(oldHighlightedCell);
						UpdateCell(highlightedCell);
						OnVisualImageChanged();
					}
					else
					{
						UpdateVisualImage();
					}
				}
			}
		}
		Point highlightedCell;

		public int VerticalShift
		{
			get { return verticalShift; }
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				var maxShift = MaxShift;
				if (value > maxShift)
				{
					value = maxShift;
				}

				if (verticalShift != value)
				{
					var oldVerticalShift = verticalShift;
					verticalShift = value;
					var shiftDelta = verticalShift - oldVerticalShift;

					if (!IsUpdateVisualImageSuspended && Math.Abs(shiftDelta) < VisualImage.Size.Height)
					{
						ShiftImageAndUpdateRest(shiftDelta);
						OnVisualImageChanged();
					}
					else
					{
						UpdateVisualImage();
					}
				}
			}
		}
		int verticalShift;

		int MaxShift => RequiresShift ? RowsCount * CellSize - ImageBoxSize.Height : 0;

		protected override void OnHoverCore(Point imagePoint)
		{
			base.OnHoverCore(imagePoint);
			HighlightedCell = GetCellFromImagePoint(imagePoint);
		}

		protected override void OnShiftCore(Point imagePoint, Size shiftSize)
		{
			if (IsTouching && RequiresShift)
			{
				VerticalShift -= shiftSize.Height;
			}
			else
			{
				base.OnShiftCore(imagePoint, shiftSize);
			}
		}

		protected override void OnMouseWheelCore(int delta)
		{
			if (RequiresShift)
			{
				VerticalShift -= delta;
			}
			else
			{
				base.OnMouseWheelCore(delta);
			}
		}

		protected override void OnClickedCore(Point imagePoint)
		{
			var cell = GetCellFromImagePoint(imagePoint);
			var index = cell.Y * ColumnsCount + cell.X;
			if (cell.X >= 0 && cell.X < ColumnsCount && cell.Y >= 0 && cell.Y < RowsCount && index < Controller.AvailableSymbols.Count)
			{
				Controller.SwitchSelection(Controller.AvailableSymbols[index].Key);

				if (!IsUpdateVisualImageSuspended)
				{
					UpdateCell(cell);
					OnVisualImageChanged();
				}
				else
				{
					UpdateVisualImage();
				}
			}
			else
			{
				base.OnClickedCore(imagePoint);
			}
		}

		Point GetCellFromImagePoint(Point imagePoint)
		{
			return new Point(imagePoint.X >= 0 ? imagePoint.X / CellSize : -1, imagePoint.Y >= 0 ? (imagePoint.Y + VerticalShift) / CellSize : -1);
		}

		#endregion

		#region VisualScrollableController

		public override bool ShowVScroll => RequiresShift;

		public override int MaxVSteps => RowsCount * CellSize;

		public override int BigVStep => ImageBoxSize.Height;

		public override int VPosition
		{
			get { return VerticalShift; }
			set { VerticalShift = value; }
		}

		public override bool ShowHScroll => false;

		public override int MaxHSteps => 0;

		public override int BigHStep => 0;

		public override int HPosition
		{
			get { return 0; }
			set { throw new NotSupportedException(); }
		}

		#endregion
	}
}
