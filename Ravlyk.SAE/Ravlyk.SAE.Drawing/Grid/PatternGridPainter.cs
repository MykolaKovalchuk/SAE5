using System;
using System.Threading.Tasks;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Grid
{
	public class PatternGridPainter
	{
		#region StitchesPaintMode

		/// <summary>
		/// Pattern grid stitches paint mode.
		/// </summary>
		public enum StitchesPaintMode
		{
			/// <summary>
			/// Draw only black symbols.
			/// </summary>
			Symbols,

			/// <summary>
			/// Draw only white symbols on black background.
			/// </summary>
			WhiteSymbols,

			/// <summary>
			/// Draw darkened colored symbols.
			/// </summary>
			ColoredSymbols,

			/// <summary>
			/// Draw lighted colors and black symbols.
			/// </summary>
			HalfTones,

			/// <summary>
			/// Draw colors and black or white symbols.
			/// </summary>
			Full,

			/// <summary>
			/// Draw only colors.
			/// </summary>
			Colors,

			/// <summary>
			/// Draw pseudo cross-stitches.
			/// </summary>
			Cross,
		}

		#endregion

		public PatternGridPainter(CodedImage image, TrueTypeFont symbolsFont)
		{
			Image = image;
			SymbolsFont = symbolsFont;

			CellSize = GridPainterSettings.Default.CellSize;
			LineWidth = 1;
			Line10DoubleWidth = GridPainterSettings.Default.Line10DoubleWidth;
			SymbolsWeight = 100;
			ShowRulers = GridPainterSettings.Default.ShowRulers;
			ShowLines = GridPainterSettings.Default.ShowLines;

			StitchesPaintMode paintMode;
			PaintMode = Enum.TryParse(GridPainterSettings.Default.PaintMode, out paintMode) ? paintMode : StitchesPaintMode.HalfTones;

			LineArgb = GridPainterSettings.Default.LineArgb.ToArgb();
			Line5Argb = GridPainterSettings.Default.Line5Argb.ToArgb();
			Line10Argb = GridPainterSettings.Default.Line10Argb.ToArgb();
			NumbersArgb = GridPainterSettings.Default.NumbersArgb.ToArgb();
			SelectionArgb1 = GridPainterSettings.Default.SelectionArgb1.ToArgb();
			SelectionArgb2 = GridPainterSettings.Default.SelectionArgb2.ToArgb();
		}

		#region Properties

		public CodedImage Image { get; }

		public TrueTypeFont SymbolsFont { get; set; }

		public StitchesPaintMode PaintMode { get; set; }

		public int CellSize
		{
			get { return cellSize; }
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				cellSize = value;
			}
		}

		int cellSize;

		public int SymbolsWeight { get; set; }

		public int LineWidth { get; set; }

		public int Line10Width { get; set; }

		public bool Line10DoubleWidth
		{
			set { Line10Width = value ? LineWidth * 2 : LineWidth; }
		}

		public int RulerWidth => ShowRulers ? CellSize * 2 : 0;

		int SymbolBoxSize => ShowLines ? CellSize - LineWidth : CellSize;

		int SymbolSize => (ShowLines ? SymbolBoxSize : CellSize) * SymbolsWeight / 100;

		int NumberSize => (ShowLines ? SymbolBoxSize : CellSize) * 3 / 4;

		public bool ShowRulers { get; set; }

		public bool ShowLines
		{
			get { return showLines && CellSize >= 4 && (PaintMode != StitchesPaintMode.Cross || WhiteCrosses == null); }
			set { showLines = value; }
		}

		bool showLines;

		public int LineArgb { get; set; }

		public int Line5Argb { get; set; }

		public int Line10Argb { get; set; }

		public int NumbersArgb { get; set; }

		public int SelectionArgb1 { get; set; }

		public int SelectionArgb2 { get; set; }

		public readonly int WhiteColorArgb = ColorBytes.ToArgb(0, 255, 255, 255);
		readonly int BlackColorArgb = 0;

		public void SaveSettings()
		{
			GridPainterSettings.Default.CellSize = CellSize;
			GridPainterSettings.Default.ShowRulers = ShowRulers;
			GridPainterSettings.Default.ShowLines = showLines; // Save field value here instead fo calculable property
			GridPainterSettings.Default.PaintMode = PaintMode.ToString();
			GridPainterSettings.Default.LineArgb = System.Drawing.Color.FromArgb(LineArgb);
			GridPainterSettings.Default.Line10Argb = System.Drawing.Color.FromArgb(Line10Argb);
			GridPainterSettings.Default.NumbersArgb = System.Drawing.Color.FromArgb(NumbersArgb);
			GridPainterSettings.Default.Save();
		}

		#endregion

		#region Selected rectangle

		public Rectangle SelectedRect
		{
			get { return selectedRect; }
			set
			{
				if (!AllowExceedingSelection)
				{
					if (value.Left < 0)
					{
						value.Width += value.Left;
						value.Left = 0;
					}
					if (value.Top < 0)
					{
						value.Height += value.Top;
						value.Top = 0;
					}
					if (value.RightExclusive > Image.Size.Width)
					{
						value.Width -= value.RightExclusive - Image.Size.Width;
					}
					if (value.BottomExclusive > Image.Size.Height)
					{
						value.Height -= value.BottomExclusive - Image.Size.Height;
					}
				}

				selectedRect = value;
			}
		}
		Rectangle selectedRect;

		public bool ShowSelectedRect { get; set; }

		public bool ShowSelectedRectPoints { get; set; }

		public bool AllowExceedingSelection { get; set; }

		public CodedImage InsertedBlock { get; set; }

		#endregion

		#region Paint

		public void Paint(IPainter painter, Size canvasSize, Rectangle canvasClipRect, Point imageStartPoint, bool includeTrimmedPart = true, Size maximumImageSize = default(Size))
		{
			painter.FillRectangle(canvasClipRect.Left, canvasClipRect.Top, canvasClipRect.Width, canvasClipRect.Height, WhiteColorArgb);

			var imageVisibleSize = includeTrimmedPart
				? new Size(
					(canvasSize.Width - RulerWidth + CellSize - 1) / CellSize,
					(canvasSize.Height - RulerWidth + CellSize - 1) / CellSize)
				: new Size(
					(canvasSize.Width - RulerWidth) / CellSize,
					(canvasSize.Height - RulerWidth) / CellSize);
			if (maximumImageSize.Width > 0 && maximumImageSize.Height > 0 && (maximumImageSize.Width < imageVisibleSize.Width || maximumImageSize.Height < imageVisibleSize.Height))
			{
				imageVisibleSize = maximumImageSize;
			}

			var imageClipRect = new Rectangle(
				imageStartPoint.X,
				imageStartPoint.Y,
				Math.Min(imageVisibleSize.Width, Image.Size.Width - imageStartPoint.X),
				Math.Min(imageVisibleSize.Height, Image.Size.Height - imageStartPoint.Y));

			var canvasStartPoint = new Point(RulerWidth, RulerWidth);
			PaintGrid(painter, canvasClipRect, canvasStartPoint, imageClipRect);

			if (ShowRulers)
			{
				PaintRulerCorner(painter, canvasClipRect);
				PaintHRuler(painter, canvasClipRect, imageClipRect.Left, imageClipRect.RightExclusive);
				PaintVRuler(painter, canvasClipRect, imageClipRect.Top, imageClipRect.BottomExclusive);
			}

			PaintSelection(painter, canvasClipRect, canvasStartPoint, imageStartPoint);
		}

		internal void ShiftImageAndUpdateRest(IPainter painter, IndexedImage canvas, Point imageStartPoint, Size shiftDelta)
		{
			var pixelShift = new Size(shiftDelta.Width * CellSize, shiftDelta.Height * CellSize);
			ImageShifter.ShiftPixels(canvas, -pixelShift.Width, -pixelShift.Height);

			if (shiftDelta.Height != 0)
			{
				if (shiftDelta.Height < 0) // Shifting down - repaint rows at top
				{
					using (painter.Clip(new Rectangle(0, 0, canvas.Size.Width, RulerWidth - pixelShift.Height)))
					{
						Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
					}
				}
				else // Shifting up - repaint rows at bottom
				{
					using (painter.Clip(new Rectangle(0, canvas.Size.Height - pixelShift.Height, canvas.Size.Width, pixelShift.Height)))
					{
						Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
					}
					if (ShowRulers)
					{
						using (painter.Clip(new Rectangle(0, 0, canvas.Size.Width, RulerWidth)))
						{
							Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
						}
					}
				}
			}

			if (shiftDelta.Width != 0)
			{
				if (shiftDelta.Width < 0) // Shifting right - repaint columns at left
				{
					using (painter.Clip(new Rectangle(0, 0, RulerWidth - pixelShift.Width, canvas.Size.Height)))
					{
						Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
					}
				}
				else // Shift left - repaint columns at right
				{
					using (painter.Clip(new Rectangle(canvas.Size.Width - pixelShift.Width, 0, pixelShift.Width, canvas.Size.Height)))
					{
						Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
					}
					if (ShowRulers)
					{
						using (painter.Clip(new Rectangle(0, 0, RulerWidth, canvas.Size.Height)))
						{
							Paint(painter, canvas.Size, painter.ClipRect, imageStartPoint);
						}
					}
				}
			}
		}

		void PaintRulerCorner(IPainter painter, Rectangle canvasClipRect)
		{
			var left = canvasClipRect.Left;
			var top = canvasClipRect.Top;
			var right = Math.Min(RulerWidth, canvasClipRect.RightExclusive);
			var width = right - left;
			var bottom = Math.Min(RulerWidth, canvasClipRect.BottomExclusive);
			var height = bottom - top;

			if (width > 0 && height > 0)
			{
				if (ShowLines)
				{
					if (right == RulerWidth)
					{
						painter.DrawVerticalLine(RulerWidth - LineWidth, top, height, Line10Argb, Line10Width);
					}
					if (bottom == RulerWidth)
					{
						painter.DrawHorizontalLine(left, RulerWidth - LineWidth, width, Line10Argb, Line10Width);
					}
				}
			}
		}

		void PaintHRuler(IPainter painter, Rectangle canvasClipRect, int startIndex, int endIndexExclusive)
		{
			var left = Math.Max(RulerWidth, canvasClipRect.Left);
			var top = canvasClipRect.Top;
			var width = Math.Min(RulerWidth + (endIndexExclusive - startIndex) * CellSize, canvasClipRect.RightExclusive) - left;
			var bottom = Math.Min(RulerWidth, canvasClipRect.BottomExclusive);
			var height = bottom - top;

			if (width > 0 && height > 0)
			{
				Action<int> paintAction =
					index =>
					{
						var x = RulerWidth + index * CellSize;
						if (x >= canvasClipRect.Left - CellSize && x < canvasClipRect.RightExclusive)
						{
							var actualIndex = index + startIndex;
							painter.PaintText((actualIndex + 1).ToString(), new Point(x, RulerWidth - LineWidth * 4), NumberSize, argb: NumbersArgb, spaceBetweenCharacters: LineWidth, direction: FontBasePainter.TextDirection.VerticalUpward);

							if (ShowLines)
							{
								x += CellSize - LineWidth;
								if (x < canvasClipRect.RightExclusive)
								{
									painter.DrawVerticalLine(x, top, height, GetLineColor(actualIndex, Image.Size.Width), GetLineWidth(actualIndex, Image.Size.Width));
								}
							}
						}
					};

				if (painter.SupportsMultithreading)
				{
					Parallel.For(0, endIndexExclusive - startIndex, paintAction);
				}
				else
				{
					for (int i = 0; i < endIndexExclusive - startIndex; i++)
					{
						paintAction(i);
					}
				}
				if (ShowLines && bottom == RulerWidth)
				{
					painter.DrawHorizontalLine(left, RulerWidth - LineWidth, width, Line10Argb, Line10Width);
				}
			}
		}

		void PaintVRuler(IPainter painter, Rectangle canvasClipRect, int startIndex, int endIndexExclusive)
		{
			var left = canvasClipRect.Left;
			var top = Math.Max(RulerWidth, canvasClipRect.Top);
			var right = Math.Min(RulerWidth, canvasClipRect.RightExclusive);
			var width = right - left;
			var height = Math.Min(RulerWidth + (endIndexExclusive - startIndex) * CellSize, canvasClipRect.BottomExclusive) - top;

			if (width > 0 && height > 0)
			{
				Action<int> paintAction =
					index =>
					{
						var y = RulerWidth + index * CellSize;
						if (y >= canvasClipRect.Top - CellSize && y < canvasClipRect.BottomExclusive)
						{
							var actualIndex = index + startIndex;
							var text = (actualIndex + 1).ToString();
							var textSize = painter.GetTextSize(text, NumberSize);
							painter.PaintText(text, new Point(RulerWidth - textSize.Width - LineWidth * 4, y), NumberSize, argb: NumbersArgb, spaceBetweenCharacters: LineWidth, direction: FontBasePainter.TextDirection.LeftToRight);

							if (ShowLines)
							{
								y += CellSize - LineWidth;
								if (y < canvasClipRect.BottomExclusive)
								{
									painter.DrawHorizontalLine(left, y, width, GetLineColor(actualIndex, Image.Size.Height), GetLineWidth(actualIndex, Image.Size.Height));
								}
							}
						}
					};
				if (painter.SupportsMultithreading)
				{
					Parallel.For(0, endIndexExclusive - startIndex, paintAction);
				}
				else
				{
					for (int i = 0; i < endIndexExclusive - startIndex; i++)
					{
						paintAction(i);
					}
				}

				if (ShowLines && right == RulerWidth)
				{
					painter.DrawVerticalLine(RulerWidth - LineWidth, top, height, Line10Argb, Line10Width);
				}
			}
		}

		void PaintGrid(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Rectangle imageClipRect)
		{
			if (PaintMode == StitchesPaintMode.Cross)
			{
				CrossPainter?.InitializeCrossesForAllColors(Image.Palette);
			}

			Action<int> paintAction =
				y =>
				{
					var canvasY = (y - imageClipRect.Top) * CellSize + canvasStartPoint.Y;
					for (int x = imageClipRect.Left, imageIndex = y * Image.Size.Width + imageClipRect.Left, canvasX = canvasStartPoint.X;
						x < imageClipRect.RightExclusive;
						x++, imageIndex++, canvasX += CellSize)
					{
						var pixelColor = GetPixelColor(x, y, imageIndex);
						if (!pixelColor.Hidden)
						{
							if (PaintMode == StitchesPaintMode.Cross && CrossPainter != null)
							{
								var cross = CrossPainter.GetCross(pixelColor.Argb);
								if (cross != null)
								{
									painter.PaintImage(cross, new Point(canvasX, canvasY));
								}
							}
							else
							{
								painter.PaintSymbol(pixelColor.SymbolChar, new Point(canvasX, canvasY), SymbolSize, SymbolBoxSize, GetSymbolColorArgb(pixelColor), GetBackColorArgb(pixelColor));
							}
						}
					}
				};

			if (painter.SupportsMultithreading)
			{
				Parallel.For(imageClipRect.Top, imageClipRect.BottomExclusive, paintAction);
			}
			else
			{
				for (int y = imageClipRect.Top; y < imageClipRect.BottomExclusive; y++)
				{
					paintAction(y);
				}
			}

			if (ShowLines)
			{
				DrawHorizontalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, LineArgb);
				DrawVerticalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, LineArgb);

				if (Line5Argb != LineArgb)
				{
					DrawHorizontalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, Line5Argb);
					DrawVerticalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, Line5Argb);
				}

				if (Line10Argb != Line5Argb & Line10Argb != LineArgb)
				{
					DrawHorizontalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, Line10Argb);
					DrawVerticalLines(painter, canvasClipRect, canvasStartPoint, imageClipRect, Line10Argb);
				}
			}
		}

		CodedColor GetPixelColor(int x, int y, int imageIndex)
		{
			if (InsertedBlock != null && SelectedRect.ContainsPoint(new Point(x, y)) &&
				x < InsertedBlock.Size.Width + SelectedRect.Left && y < InsertedBlock.Size.Height + SelectedRect.Top)
			{
				return InsertedBlock[x - SelectedRect.Left, y - SelectedRect.Top];
			}
			else
			{
				return Image.Palette[Image.Pixels[imageIndex]];
			}
		}

		void DrawHorizontalLines(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Rectangle imageClipRect, int filterArgb)
		{
			Action<int> paintAction =
				y =>
				{
					var currentLineArgb = GetLineColor(y, Image.Size.Height);
					if (currentLineArgb == filterArgb)
					{
						var canvasY = (y - imageClipRect.Top + 1) * CellSize + canvasStartPoint.Y - LineWidth;
						if (canvasY < canvasClipRect.BottomExclusive)
						{
							var left = Math.Max(RulerWidth, canvasClipRect.Left);
							var right = Math.Min(imageClipRect.Width * CellSize + canvasStartPoint.X, canvasClipRect.RightExclusive);
							painter.DrawHorizontalLine(left, canvasY, right - left, currentLineArgb, GetLineWidth(y, Image.Size.Height));
						}
					}
				};

			if (painter.SupportsMultithreading)
			{
				Parallel.For(imageClipRect.Top, imageClipRect.BottomExclusive, paintAction);
			}
			else
			{
				for (int y = imageClipRect.Top; y < imageClipRect.BottomExclusive; y++)
				{
					paintAction(y);
				}
			}
		}

		void DrawVerticalLines(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Rectangle imageClipRect, int filterArgb)
		{
			Action<int> paintAction =
				x =>
				{
					var currentLineArgb = GetLineColor(x, Image.Size.Width);
					if (currentLineArgb == filterArgb)
					{
						var canvasX = (x - imageClipRect.Left + 1) * CellSize + canvasStartPoint.X - LineWidth;
						if (canvasX < canvasClipRect.RightExclusive)
						{
							var top = Math.Max(RulerWidth, canvasClipRect.Top);
							var bottom = Math.Min(imageClipRect.Height * CellSize + canvasStartPoint.Y, canvasClipRect.BottomExclusive);
							painter.DrawVerticalLine(canvasX, top, bottom - top, currentLineArgb, GetLineWidth(x, Image.Size.Width));
						}
					}
				};

			if (painter.SupportsMultithreading)
			{
				Parallel.For(imageClipRect.Left, imageClipRect.RightExclusive, paintAction);
			}
			else
			{
				for (int x = imageClipRect.Left; x < imageClipRect.RightExclusive; x++)
				{
					paintAction(x);
				}
			}
		}

		#region Selection

		internal void PaintSelection(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Point imageStartPoint)
		{
			if (painter.SupportsMultithreading &&
				ShowSelectedRect &&
				SelectedRect.Width > 0 && SelectedRect.Height > 0)
			{
				PaintVSelectionBorder(painter, canvasClipRect, canvasStartPoint, imageStartPoint, selectedRect.Left, selectedRect.Top, selectedRect.BottomExclusive);
				PaintVSelectionBorder(painter, canvasClipRect, canvasStartPoint, imageStartPoint, selectedRect.RightExclusive, selectedRect.Top, selectedRect.BottomExclusive);

				PaintHSelectionBorder(painter, canvasClipRect, canvasStartPoint, imageStartPoint, selectedRect.Left, selectedRect.RightExclusive, selectedRect.Top);
				PaintHSelectionBorder(painter, canvasClipRect, canvasStartPoint, imageStartPoint, selectedRect.Left, selectedRect.RightExclusive, selectedRect.BottomExclusive);

				if (ShowSelectedRectPoints)
				{
					var x1 = (SelectedRect.Left - imageStartPoint.X) * CellSize + canvasStartPoint.X - 1;
					var y1 = (selectedRect.Top - imageStartPoint.Y) * CellSize + canvasStartPoint.Y - 1;
					var x2 = (SelectedRect.RightExclusive - imageStartPoint.X) * CellSize + canvasStartPoint.X - 1;
					var y2 = (selectedRect.BottomExclusive - imageStartPoint.Y) * CellSize + canvasStartPoint.Y - 1;

					PaintShifter(painter, canvasClipRect, x1, y1);
					PaintShifter(painter, canvasClipRect, x2, y1);
					PaintShifter(painter, canvasClipRect, x1, y2);
					PaintShifter(painter, canvasClipRect, x2, y2);

					var xm = (x1 + x2) / 2;
					var ym = (y1 + y2) / 2;

					PaintShifter(painter, canvasClipRect, xm, y1);
					PaintShifter(painter, canvasClipRect, xm, y2);
					PaintShifter(painter, canvasClipRect, x1, ym);
					PaintShifter(painter, canvasClipRect, x2, ym);
				}
			}
		}

		void PaintVSelectionBorder(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Point imageStartPoint, int imageX, int imageY1, int imageY2)
		{
			var canvasX = (imageX - imageStartPoint.X) * CellSize + canvasStartPoint.X - 1;

			var canvasY1 = Math.Max((imageY1 - imageStartPoint.Y) * CellSize + canvasStartPoint.Y - 1, canvasClipRect.Top);
			var canvasY2 = Math.Min((imageY2 - imageStartPoint.Y) * CellSize + canvasStartPoint.Y - 1, canvasClipRect.BottomExclusive);

			if (canvasY2 > canvasY1 && canvasX >= canvasClipRect.Left && canvasX <= canvasClipRect.RightExclusive)
			{
				Parallel.For(canvasY1, canvasY2 + 1, y =>
				{
					for (int x = canvasX - 1; x <= canvasX + 1; x++)
					{
						if (x >= canvasClipRect.Left && x < canvasClipRect.RightExclusive)
						{
							painter.FillRectangle(x, y, 1, 1, GetSelectionRectPixelColor(x, y));
						}
					}
				});
			}
		}

		void PaintHSelectionBorder(IPainter painter, Rectangle canvasClipRect, Point canvasStartPoint, Point imageStartPoint, int imageX1, int imageX2, int imageY)
		{
			var canvasX1 = Math.Max((imageX1 - imageStartPoint.X) * CellSize + canvasStartPoint.X - 1, canvasClipRect.Left);
			var canvasX2 = Math.Min((imageX2 - imageStartPoint.X) * CellSize + canvasStartPoint.X - 1, canvasClipRect.RightExclusive);

			var canvasY = (imageY - imageStartPoint.Y) * CellSize + canvasStartPoint.Y - 1;

			if (canvasX2 > canvasX1 && canvasY >= canvasClipRect.Top && canvasY <= canvasClipRect.BottomExclusive)
			{
				Parallel.For(canvasX1, canvasX2 + 1, x =>
				{
					for (int y = canvasY - 1; y <= canvasY + 1; y++)
					{
						if (y >= canvasClipRect.Top && y < canvasClipRect.BottomExclusive)
						{
							painter.FillRectangle(x, y, 1, 1, GetSelectionRectPixelColor(x, y));
						}
					}
				});
			}
		}

		int GetSelectionRectPixelColor(int x, int y)
		{
			var x1 = x % 5;
			var y1 = y % 5;
			return x1 == y1 || x1 - 1 == y1 || (x1 == 0 && y1 == 4) ? SelectionArgb1 : SelectionArgb2;
		}

		void PaintShifter(IPainter painter, Rectangle canvasClipRect, int canvasX, int canvasY)
		{
			if (canvasX >= canvasClipRect.Left && canvasX <= canvasClipRect.RightExclusive && canvasY >= canvasClipRect.Top && canvasY <= canvasClipRect.BottomExclusive)
			{
				painter.FillRectangle(canvasX - 2, canvasY - 2, 5, 5, SelectionArgb1);
				painter.DrawHorizontalLine(canvasX - 3, canvasY - 3, 7, SelectionArgb2);
				painter.DrawHorizontalLine(canvasX - 3, canvasY + 3, 7, SelectionArgb2);
				painter.DrawVerticalLine(canvasX - 3, canvasY - 2, 5, SelectionArgb2);
				painter.DrawVerticalLine(canvasX + 3, canvasY - 2, 5, SelectionArgb2);
			}
		}

		#endregion

		#endregion

		#region Colors calculation

		internal int GetBackColorArgb(Color pixelColor)
		{
			switch (PaintMode)
			{
				case StitchesPaintMode.Symbols:
				case StitchesPaintMode.ColoredSymbols:
				case StitchesPaintMode.Cross:
					return WhiteColorArgb;
				case StitchesPaintMode.WhiteSymbols:
					return BlackColorArgb;
				case StitchesPaintMode.HalfTones:
					return pixelColor.HalfToneHighColor.Argb;
				case StitchesPaintMode.Full:
				case StitchesPaintMode.Colors:
					return pixelColor.Argb;
				default:
					return WhiteColorArgb;
			}
		}

		internal int GetSymbolColorArgb(Color pixelColor)
		{
			switch (PaintMode)
			{
				case StitchesPaintMode.Symbols:
				case StitchesPaintMode.HalfTones:
					return BlackColorArgb;
				case StitchesPaintMode.WhiteSymbols:
					return WhiteColorArgb;
				case StitchesPaintMode.ColoredSymbols:
					return pixelColor.HalfToneLowColor.Argb;
				case StitchesPaintMode.Full:
					return pixelColor.Darkness > ColorBytes.HalfDark ? BlackColorArgb : WhiteColorArgb;
				case StitchesPaintMode.Colors:
				case StitchesPaintMode.Cross:
					return pixelColor.Argb;
				default:
					return BlackColorArgb;
			}
		}

		int GetLineColor(int index, int length)
		{
			return
				(index + 1) % 10 == 0 || index + 1 == length
					? Line10Argb
					: (index + 1) % 5 == 0
						? Line5Argb
						: LineArgb;
		}

		int GetLineWidth(int index, int length)
		{
			return (index + 1) % 10 == 0 || index + 1 == length ? Line10Width : LineWidth;
		}

		#endregion

		#region Painters

		CrossPainter CrossPainter
		{
			get
			{
				if ((crossPainter == null || crossPainter.PixelHeight != SymbolBoxSize) && WhiteCrosses != null)
				{
					crossPainter = new CrossPainter(SymbolBoxSize, WhiteCrosses, Image.Palette);
				}
				return crossPainter;
			}
		}
		CrossPainter crossPainter;

		public IndexedImage WhiteCrosses { get; set; }

		#endregion
	}
}
