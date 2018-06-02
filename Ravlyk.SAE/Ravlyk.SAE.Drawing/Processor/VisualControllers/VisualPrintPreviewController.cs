using System;
using System.Collections.Generic;
using System.Linq;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class VisualPrintPreviewController : VisualBoxedController
	{
		public VisualPrintPreviewController(IImageProvider imageProvider, PatternGridPainter gridPainter, IList<CodedColor> orderedColors, Size imageBoxSize = default(Size))
			: base(imageProvider, imageBoxSize)
		{
			GridPainter = gridPainter;
			OrderedColors = orderedColors ?? SourceImage.Palette.OrderByDarknes().Cast<CodedColor>().ToList();
		}

		IList<CodedColor> OrderedColors { get; }

		#region Properties

		const decimal MinCellSize = 2.0m;
		const decimal MaxCellSize = 20.0m;

		/// <summary>
		/// Printed cell size in mm.
		/// </summary>
		public decimal CellSizeMm
		{
			get { return cellSizeMm; }
			set
			{
				if (value < MinCellSize) { value = MinCellSize; }
				if (value > MaxCellSize) { value = MaxCellSize; }

				if (value != cellSizeMm)
				{
					cellSizeMm = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		decimal cellSizeMm = GridPainterSettings.Default.PrintCellSize;

		/// <summary>
		/// Number of pixels per mm.
		/// </summary>
		public decimal PixelsPerMm
		{
			get { return pixelsPerMm; }
			set
			{
				if (value < 1.0m) { value = 1.0m; }

				if (value != pixelsPerMm)
				{
					pixelsPerMm = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		decimal pixelsPerMm = 1.0m;

		/// <summary>
		/// Cell size in pixels.
		/// </summary>
		int CellWidth => (int)(CellSizeMm * PixelsPerMm);

		int RulerWidth => CellWidth * 2;

		int ColorRowHeight => (int)(4 * PixelsPerMm);

		int ColorColumnNoWidth => ColorRowHeight * 3;
		int ColorColumnSymbolWidth => ColorRowHeight * 2;
		int ColorColumnCodeWidth => ColorRowHeight * 6;
		int ColorColumnColorWidth => ColorRowHeight * 4;
		int ColorColumnCountWidth => ColorRowHeight * 5;

		Size ColorRowSize => new Size(ColorColumnNoWidth + ColorColumnSymbolWidth + ColorColumnCodeWidth + ColorColumnColorWidth + ColorColumnCountWidth, ColorRowHeight);
		Size ColorRowsPerPage => new Size(
			Math.Max((PagePrintSize.Width + ColorRowHeight) / (ColorRowSize.Width + ColorRowHeight), 1),
			Math.Max(PagePrintSize.Height / ColorRowSize.Height - 1, 1));

		/// <summary>
		/// Page printable size in pixels (after removing all margins).
		/// </summary>
		public Size PagePrintSize
		{
			get { return pagePrintSize; }
			set
			{
				if (value != pagePrintSize)
				{
					pagePrintSize = value;
					UpdateParametersAndVisualImage();
				}
			}
		}
		Size pagePrintSize;

		public Size PageCellsSize { get; private set; }

		public Size PrintSchemePagesCount { get; private set; }
		public int PrintPalettePagesCount { get; private set; }
		public int PagesCount => PrintSchemePagesCount.Width * PrintSchemePagesCount.Height + PrintPalettePagesCount;

		public void SaveSettings()
		{
			GridPainterSettings.Default.PrintCellSize = CellSizeMm;
			GridPainterSettings.Default.Save();
		}

		#endregion

		#region Update Visual Image

		public IDisposable SuspendChangePageCellsSize()
		{
			return DisposableLock.Lock(ref lockChangePageCellsSize);
		}

		DisposableLock lockChangePageCellsSize;

		protected override CodedImage CreateVisualImage()
		{
			return new CodedImage();
		}

		Size pageThumbnailSize;

		const int PreviewPagesMargin = 8;
		const int PreviewPageBorder = 4;

		readonly int BlackColorArgb = ColorBytes.ToArgb(0, 105, 105, 105);
		readonly int WhiteColorArgb = ColorBytes.ToArgb(0, 255, 255, 255);

		protected override void UpdateParameters()
		{
			base.UpdateParameters();

			if (PagePrintSize.Width <= 0 || PagePrintSize.Height <= 0) return;

			var newPageCellsSize = new Size((PagePrintSize.Width - RulerWidth) / CellWidth, (PagePrintSize.Height - RulerWidth) / CellWidth);
			if (!lockChangePageCellsSize.IsLocked() || newPageCellsSize.Width + 1 < PageCellsSize.Width || newPageCellsSize.Height + 1 < PageCellsSize.Height)
			{
				PageCellsSize = newPageCellsSize;
			}
			if (PageCellsSize.Width <= 0 || PageCellsSize.Height <= 0) return;

			PrintSchemePagesCount = new Size((SourceImage.Size.Width + PageCellsSize.Width - 1) / PageCellsSize.Width, (SourceImage.Size.Height + PageCellsSize.Height - 1) / PageCellsSize.Height);
			if (PrintSchemePagesCount.Width <= 0 || PrintSchemePagesCount.Height <= 0) return;

			PrintPalettePagesCount = Math.Max(((SourceImage.Palette.Count + 20 + ColorRowsPerPage.Width - 1) / ColorRowsPerPage.Width + ColorRowsPerPage.Height - 1) / ColorRowsPerPage.Height, 1);

			var palettePagesColumns = (PrintPalettePagesCount + PrintSchemePagesCount.Height - 1) / PrintSchemePagesCount.Height;
			var actualColumnsCount = PrintSchemePagesCount.Width + palettePagesColumns;

			var maxPageWidth = (ImageBoxSize.Width - PreviewPagesMargin * 3) / actualColumnsCount - PreviewPagesMargin;
			var maxPageHeight = (ImageBoxSize.Height - PreviewPagesMargin) / PrintSchemePagesCount.Height - PreviewPagesMargin;
			if (maxPageWidth <= 0 || maxPageHeight <= 0) return;

			pageThumbnailSize = PagePrintSize.Width * maxPageHeight > PagePrintSize.Height * maxPageWidth
				? new Size(maxPageWidth, PagePrintSize.Height * maxPageWidth / PagePrintSize.Width)
				: new Size(PagePrintSize.Width * maxPageHeight / PagePrintSize.Height, maxPageHeight);
			if (pageThumbnailSize.Width > maxPageWidth) { pageThumbnailSize.Width = maxPageWidth; }
			if (pageThumbnailSize.Height > maxPageHeight) { pageThumbnailSize.Height = maxPageHeight; }
		}

		protected override void UpdateVisualImageCore()
		{
			base.UpdateVisualImageCore();

			VisualImage.Size = ImageBoxSize;
			ImagePainter.FillRect(VisualImage, new Rectangle(0, 0, VisualImage.Size.Width, VisualImage.Size.Height), BlackColorArgb);

			if (PagePrintSize.Width <= 0 || PagePrintSize.Height <= 0 || PrintSchemePagesCount.Width <= 0 || PrintSchemePagesCount.Height <= 0 || pageThumbnailSize.Width <= 0 || pageThumbnailSize.Height <= 0)
			{
				return;
			}

			var imageVisualPartSize = new Size(pageThumbnailSize.Width - PreviewPageBorder * 2, pageThumbnailSize.Height - PreviewPageBorder * 2);

			var pageWithMarginHeight = pageThumbnailSize.Height + PreviewPagesMargin;
			var startY = (VisualImage.Size.Height - pageWithMarginHeight * PrintSchemePagesCount.Height + PreviewPagesMargin) / 2;

			var pageWithMarginWidth = pageThumbnailSize.Width + PreviewPagesMargin;
			var palettePagesColumns = (PrintPalettePagesCount + PrintSchemePagesCount.Height - 1) / PrintSchemePagesCount.Height;
			var actualColumnsCount = PrintSchemePagesCount.Width + palettePagesColumns;
			var startX = (VisualImage.Size.Width - pageWithMarginWidth * actualColumnsCount) / 2;
			if (startX < 0) { startX = 0; }

			var schemePagesCount = PrintSchemePagesCount.Width * PrintSchemePagesCount.Height;

			for (int row = 0, y = startY, srcY = 0;
				row < PrintSchemePagesCount.Height;
				row++, y += pageWithMarginHeight, srcY += PageCellsSize.Height)
			{
				for (int col = 0, x = startX, srcX = 0;
					col < actualColumnsCount;
					col++, x += pageWithMarginWidth, srcX += PageCellsSize.Width)
				{
					var actualX = x;
					if (col >= PrintSchemePagesCount.Width)
					{
						actualX += PreviewPageBorder * 2;
					}

					if (col < PrintSchemePagesCount.Width && imageVisualPartSize.Width > 0 && imageVisualPartSize.Height > 0)
					{
						ImagePainter.FillRect(VisualImage, new Rectangle(actualX, y, pageThumbnailSize.Width, pageThumbnailSize.Height), WhiteColorArgb);

						var sourceCropRect = new Rectangle(srcX, srcY, PageCellsSize.Width, PageCellsSize.Height);
						if (sourceCropRect.RightExclusive > SourceImage.Size.Width)
						{
							sourceCropRect.Width = SourceImage.Size.Width - srcX;
						}
						if (sourceCropRect.BottomExclusive > SourceImage.Size.Height)
						{
							sourceCropRect.Height = SourceImage.Size.Height - srcY;
						}
						var croppedImage = ImageCropper.Crop(SourceImage, sourceCropRect);

						var zoomSize = new Size(sourceCropRect.Width * imageVisualPartSize.Width / PageCellsSize.Width, sourceCropRect.Height * imageVisualPartSize.Height / PageCellsSize.Height);
						var zoomedPartImage = new ImageResampler().Resample(croppedImage, zoomSize, ImageResampler.FilterType.Box);

						ImageCopier.Copy(zoomedPartImage, VisualImage, new Point(actualX + PreviewPageBorder, y + PreviewPageBorder));

						PaintPageNumber(row * PrintSchemePagesCount.Width + col + 1, actualX, y);
					}
					else if (row * palettePagesColumns + col - PrintSchemePagesCount.Width + schemePagesCount < PagesCount)
					{
						ImagePainter.FillRect(VisualImage, new Rectangle(actualX, y, pageThumbnailSize.Width, pageThumbnailSize.Height), WhiteColorArgb);

						var lineX = actualX + PreviewPageBorder;
						var lineLength = pageThumbnailSize.Width / 2;
						var lastLineY = y + pageThumbnailSize.Height - PreviewPageBorder * 2;
						for (int lineY = y + PreviewPageBorder; lineY < lastLineY; lineY += 3)
						{
							ImagePainter.DrawHorizontalLine(VisualImage, lineX, lineY, lineLength, BlackColorArgb);
						}

						PaintPageNumber(schemePagesCount + row + (col - PrintSchemePagesCount.Width) * PrintSchemePagesCount.Height + 1, actualX, y);
					}
				}
			}
		}

		void PaintPageNumber(int number, int x, int y)
		{
			var numberString = number.ToString();
			var numberSize = PageNumberPainter.GetTextSize(numberString);
			ImagePainter.FillRect(VisualImage, new Rectangle(x, y, numberSize.Width + 4, numberSize.Height + 4), WhiteColorArgb);
			PageNumberPainter.PaintText(numberString, VisualImage.Pixels, VisualImage.Size, new Point(x + 2, y + 2));
		}

		TextPainter PageNumberPainter => pageNumberPainter ?? (pageNumberPainter = new TextPainter(GridPainter.SymbolsFont, 16));
		TextPainter pageNumberPainter;

		#endregion

		#region Paint Page

		public PatternGridPainter GridPainter { get; }

		public void PaintPrintPageImage(IPainter painter, int page, Size pageSize, Rectangle margins, string appInfo, string fileInfo)
		{
			var lineWidth = Math.Max((int)(PixelsPerMm / 4m), 1);

			painter.FillRectangle(margins.Left, margins.Top, margins.Width, margins.Height, WhiteColorArgb);

			using (painter.TranslateCoordinates(new Size(margins.Left, margins.Top)))
			{
				if (page < PagesCount - PrintPalettePagesCount)
				{
					PaintSchemePage(painter, page, new Size(margins.Width, margins.Height), new Rectangle(0, 0, margins.Width, margins.Height), lineWidth);
				}
				else
				{
					PaintPalettePage(painter, page, lineWidth, margins);
				}
			}

			var infoY = margins.BottomExclusive + (int)(CellSizeMm / 2);
			var textSize = (int)(PixelsPerMm * 3);
			var textPainter = new TextPainter(GridPainter.SymbolsFont, (int)(PixelsPerMm * 3));

			painter.PaintText(appInfo, new Point(margins.Left, infoY), textSize, spaceBetweenCharacters: lineWidth);

			var fileInfoWidth = textPainter.GetTextSize(fileInfo, spaceBetweenCharacters: lineWidth).Width;
			painter.PaintText(fileInfo, new Point(margins.RightExclusive - fileInfoWidth, infoY), textSize, spaceBetweenCharacters: lineWidth);

			var pageNo = String.Format(Resources.PrintPageNo, page + 1, PagesCount);
			var pageNoWidth = textPainter.GetTextSize(pageNo, spaceBetweenCharacters: lineWidth).Width;
			painter.PaintText(pageNo, new Point(margins.Left + (margins.Width - pageNoWidth) / 2, infoY), textSize, spaceBetweenCharacters: lineWidth);
		}

		void PaintSchemePage(IPainter painter, int page, Size canvasSize, Rectangle clipRect, int lineWidth)
		{
			var pageRow = page / PrintSchemePagesCount.Width;
			var pageCol = page - pageRow * PrintSchemePagesCount.Width;
			var gridRow = pageRow * PageCellsSize.Height;
			var gridCol = pageCol * PageCellsSize.Width;

			var originalCellSize = GridPainter.CellSize;
			var originalLineWidth = GridPainter.LineWidth;
			try
			{
				GridPainter.CellSize = CellWidth;
				GridPainter.LineWidth = lineWidth;
				GridPainter.Paint(painter, canvasSize, clipRect, new Point(gridCol, gridRow), false, PageCellsSize);

				try
				{
					var centerRow = SourceImage.Size.Height / 2;
					if (centerRow >= gridRow && centerRow < gridRow + PageCellsSize.Height)
					{
						var x = -CellWidth - lineWidth * 2;
						var y = GridPainter.RulerWidth + (centerRow - gridRow) * CellWidth;
						if (SourceImage.Size.Height % 2 == 0)
						{
							y -= CellWidth / 2;
						}

						painter.PaintSymbol('>', new Point(x, y), CellWidth, CellWidth, GridPainter.Line10Argb);
					}

					var centerCol = SourceImage.Size.Width / 2;
					if (centerCol >= gridCol && centerCol < gridCol + PageCellsSize.Width)
					{
						var x = GridPainter.RulerWidth + (centerCol - gridCol) * CellWidth;
						var y = -CellWidth - lineWidth * 2;
						if (SourceImage.Size.Width % 2 == 0)
						{
							x -= CellWidth / 2;
						}

						painter.PaintSymbol('v', new Point(x, y), CellWidth, CellWidth, GridPainter.Line10Argb);
					}
				}
				catch
				{
					//Markers are not important, and scheme itself is already painted, ignore any exception here.
				}
			}
			finally
			{
				GridPainter.LineWidth = originalLineWidth;
				GridPainter.CellSize = originalCellSize;
			}
		}

		void PaintPalettePage(IPainter painter, int page, int lineWidth, Rectangle margins)
		{
			var palettePage = page - PagesCount + PrintPalettePagesCount;

			var zoomedColorRowHeight = ColorRowHeight;
			var zoomedColorRowWidth = ColorRowSize.Width;
			var zoomedColorRowWidthWithSpacer = zoomedColorRowWidth + zoomedColorRowHeight;

			if (palettePage == 0)
			{
				var thumbnailMaxSize = zoomedColorRowHeight * 9;
				var sourceImageMaxSize = Math.Max(SourceImage.Size.Width, SourceImage.Size.Height);
				var thumbnailSize = new Size(SourceImage.Size.Width * thumbnailMaxSize / sourceImageMaxSize, SourceImage.Size.Height * thumbnailMaxSize / sourceImageMaxSize);
				var thumbnailImage = new ImageResampler().Resample(SourceImage, thumbnailSize, ImageResampler.FilterType.Box);
				painter.PaintImage(thumbnailImage, new Point(0, 0));

				var infoX = thumbnailMaxSize + zoomedColorRowHeight;
				var infoY = 0;
				var textSize = zoomedColorRowHeight - lineWidth * 2;
				painter.PaintText(SourceImage.Description, new Point(infoX, infoY), textSize);

				infoY += zoomedColorRowHeight * 2;
				painter.PaintText(string.Format(Resources.PrintThreadsName, SourceImage.Palette.Name), new Point(infoX, infoY), textSize);

				var indexTextSize = textSize * 2 / 3;
				var indexCellSize = new Size(textSize, indexTextSize * 4 / 3);
				var startIndexX = margins.Width - PrintSchemePagesCount.Width * indexCellSize.Width;
				var indexY = 0;
				painter.PaintText(Resources.PrintPagesIndex, new Point(startIndexX, indexY), indexTextSize);
				indexY += textSize;
				for (int row = 0, index = 1; row < PrintSchemePagesCount.Height; row++, indexY += indexCellSize.Height)
				{
					for (int col = 0, indexX = startIndexX; col < PrintSchemePagesCount.Width; col++, index++, indexX += indexCellSize.Width)
					{
						painter.PaintText(index.ToString(), new Point(indexX, indexY), indexTextSize);
					}
				}
			}

			var startNo = palettePage * ColorRowsPerPage.Width * ColorRowsPerPage.Height;
			if (palettePage > 0)
			{
				startNo -= 20;
			}
			var startY = palettePage > 0 ? 0 : zoomedColorRowHeight * 10;
			var rowsPerPage = palettePage > 0 ? ColorRowsPerPage.Height : ColorRowsPerPage.Height - 10;

			for (int col = 0, x = 0; col < ColorRowsPerPage.Width; col++, x += zoomedColorRowWidthWithSpacer, startNo += rowsPerPage)
			{
				DrawColorsTable(painter, new Point(x, startY), OrderedColors, startNo, startNo + rowsPerPage,
					lineWidth, ColorRowSize, GridPainter.NumbersArgb, GridPainter.LineArgb, GridPainter.Line10Argb,
					ColorColumnNoWidth, ColorColumnSymbolWidth, ColorColumnCodeWidth, ColorColumnColorWidth);
			}
		}

		#region Draw color rows

		public static void DrawColorsTable(IPainter painter, Point point, IList<CodedColor> orderedColors,
			int lineWidth, int rowHeight, int numbersArgb, int lineArgb, int line10Argb)
		{
			var colorColumnNoWidth = rowHeight * 3;
			var colorColumnSymbolWidth = rowHeight * 2;
			var colorColumnCodeWidth = rowHeight * 6;
			var colorColumnColorWidth = rowHeight * 4;
			var colorColumnCountWidth = rowHeight * 5;

			var rowSize = new Size(colorColumnNoWidth + colorColumnSymbolWidth + colorColumnCodeWidth + colorColumnColorWidth + colorColumnCountWidth, rowHeight);

			DrawColorsTable(painter, point, orderedColors, 0, orderedColors.Count,
				lineWidth, rowSize, numbersArgb, lineArgb, line10Argb,
				colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth);
		}

		public static int GetColorRowWidth(int rowHeight)
		{
			var colorColumnNoWidth = rowHeight * 3;
			var colorColumnSymbolWidth = rowHeight * 2;
			var colorColumnCodeWidth = rowHeight * 6;
			var colorColumnColorWidth = rowHeight * 4;
			var colorColumnCountWidth = rowHeight * 5;

			return colorColumnNoWidth + colorColumnSymbolWidth + colorColumnCodeWidth + colorColumnColorWidth + colorColumnCountWidth;
		}

		static void DrawColorsTable(IPainter painter, Point point, IList<CodedColor> orderedColors, int from, int toExclusive,
			int lineWidth, Size rowSize, int numbersArgb, int lineArgb, int line10Argb,
			int colorColumnNoWidth, int colorColumnSymbolWidth, int colorColumnCodeWidth, int colorColumnColorWidth)
		{
			DrawColorRow(painter, new Point(point.X, point.Y), lineWidth, 0, null,
				rowSize, numbersArgb, lineArgb, line10Argb,
				colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth);

			for (int startNo = from, y = point.Y + rowSize.Height; startNo < toExclusive && startNo < orderedColors.Count; startNo++, y += rowSize.Height)
			{
				DrawColorRow(painter, new Point(point.X, y), lineWidth, startNo + 1, orderedColors[startNo],
					rowSize, numbersArgb, lineArgb, line10Argb,
					colorColumnNoWidth, colorColumnSymbolWidth, colorColumnCodeWidth, colorColumnColorWidth);
			}
		}

		static void DrawColorRow(IPainter painter, Point point, int lineWidth, int no, CodedColor color,
			Size rowSize, int numbersArgb, int lineArgb, int line10Argb,
			int colorColumnNoWidth, int colorColumnSymbolWidth, int colorColumnCodeWidth, int colorColumnColorWidth)
		{
			var zoomedColorRowHeight = rowSize.Height;
			var zoomedColorRowWidth = rowSize.Width;

			var textArgb = color == null ? numbersArgb : 0;
			var textSize = zoomedColorRowHeight - lineWidth * 2;
			var symbolSize = zoomedColorRowHeight - lineWidth;

			var x = point.X;
			painter.PaintText(color != null ? no.ToString() : "#", new Point(x, point.Y), textSize, argb: textArgb);

			x += colorColumnNoWidth;
			if (color != null)
			{
				painter.PaintSymbol(color.SymbolChar, new Point(x, point.Y + lineWidth), symbolSize, zoomedColorRowHeight);
			}

			x += colorColumnSymbolWidth;
			painter.PaintText(color != null ? color.ColorCode : Resources.PrintColumnCode, new Point(x, point.Y), textSize, argb: textArgb);

			x += colorColumnCodeWidth;
			var colorColumnWidth = colorColumnColorWidth;
			if (color != null)
			{
				painter.FillRectangle(x, point.Y + lineWidth, colorColumnWidth * 2 / 3, zoomedColorRowHeight - lineWidth, color.Argb);
			}
			else
			{
				painter.PaintText(Resources.PrintColumnColor, new Point(x, point.Y), textSize, argb: textArgb);
			}

			x += colorColumnWidth;
			painter.PaintText(color?.OccurrencesCount.ToString() ?? Resources.PrintColumnCount, new Point(x, point.Y), textSize, argb: textArgb);

			painter.DrawHorizontalLine(point.X, point.Y + zoomedColorRowHeight, zoomedColorRowWidth, color == null ? line10Argb : lineArgb, lineWidth);
		}

		#endregion

		#endregion
	}
}
