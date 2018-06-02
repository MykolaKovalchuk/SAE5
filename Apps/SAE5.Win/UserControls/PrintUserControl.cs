using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Adopted.PdfSharp;
using Ravlyk.Adopted.PdfSharp.Drawing;
using Ravlyk.Adopted.PdfSharp.Pdf;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE5.WinForms.Properties;
using Rectangle = Ravlyk.Common.Rectangle;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class PrintUserControl : UserControl
	{
		public enum SizeUnit
		{
			Mm,
			Inch
		}

		static PrintUserControl()
		{
			SaeFontResolver.Setup();
		}

		public PrintUserControl()
		{
			InitializeComponent();

			printDocument = new PrintDocument { DocumentName = Resources.SaeFileDescription };
			printDocument.PrintPage += PrintDocument_PrintPage;
			printDocument.DefaultPageSettings.Landscape = Settings.Default.PrintPageLandscape;
			printDocument.DefaultPageSettings.Margins = Settings.Default.PrintPageMargins;
		}

		readonly PrintDocument printDocument;
		PrintPreviewControl previewControl;

		public IList<string> GetAvailablePrinters(out string defaultPrinter)
		{
			defaultPrinter = printDocument.PrinterSettings.PrinterName;
			return PrinterSettings.InstalledPrinters.Cast<string>().ToList();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Settings.Default.PrintPageMargins = printDocument.DefaultPageSettings.Margins;
				Settings.Default.PrintPageLandscape = printDocument.DefaultPageSettings.Landscape;
				Settings.Default.Save();

				Controller?.SaveSettings();

				printDocument?.Dispose();
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Controller

		public VisualPrintPreviewController Controller
		{
			get { return visualControlPrintPreview.Controller as VisualPrintPreviewController; }
			set
			{
				if (value != null && value != visualControlPrintPreview.Controller)
				{
					value.ImageBoxSize = new Size(visualControlPrintPreview.Width, visualControlPrintPreview.Height);

					using (value.SuspendUpdateVisualImage())
					{
						UpdatePagePrintSize(value);
					}

					printDocument.DocumentName = value.SourceImage.Description;
				}

				visualControlPrintPreview.Controller = value;
			}
		}

		public void UpdatePagePrintSize(VisualPrintPreviewController controller, bool keepPageCellsSize = false)
		{
			if (controller == null)
			{
				return;
			}

			var pageSettings = printDocument.DefaultPageSettings;
			using (controller.SuspendUpdateVisualImage())
			{
				var pageSize = new Size(
					pageSettings.Bounds.Width - pageSettings.Margins.Left - pageSettings.Margins.Right,
					pageSettings.Bounds.Height - pageSettings.Margins.Top - pageSettings.Margins.Bottom);
				if (ForPdf)
				{
					pageSize = new Size(pageSize.Width * 72 / 100, pageSize.Height * 72 / 100);
				}
				using (keepPageCellsSize ? controller.SuspendChangePageCellsSize() : null)
				{
					controller.PagePrintSize = pageSize;
					controller.PixelsPerMm = (ForPdf ? 72m : 100m) / 25.4m;
				}
			}

			printDocument.PrinterSettings.MinimumPage = 1;
			printDocument.PrinterSettings.FromPage = 1;
			printDocument.PrinterSettings.MaximumPage = controller.PagesCount;
			printDocument.PrinterSettings.ToPage = controller.PagesCount;
		}

		public bool ForPdf { get; set; }

		#endregion

		#region Print options

		public void SetPrinter(string printerName)
		{
			printDocument.PrinterSettings.PrinterName = printerName;
			UpdatePagePrintSize(Controller);
			ResetPreviewControl();
		}

		public decimal CellSize
		{
			get { return Controller.CellSizeMm / CellSizeToMmCoeff; }
			set
			{
				Controller.CellSizeMm = value * CellSizeToMmCoeff;
				UpdatePagePrintSize(Controller);
				ResetPreviewControl();
			}
		}

		public SizeUnit Unit { get; set; }

		decimal CellSizeToMmCoeff => Unit == SizeUnit.Inch ? 25.4m : 1m;

		public void OpenPageSetup()
		{
			ResetPreviewControl();
			using (var pageSetupDialog = new PageSetupDialog
			{
				Document = printDocument,
				EnableMetric = true,
				AllowPrinter = !ForPdf
			})
			{
				if (ForPdf)
				{
					SelectVirtualPrinter(printDocument.PrinterSettings);
				}

				if (pageSetupDialog.ShowDialog() == DialogResult.OK)
				{
					UpdatePagePrintSize(Controller);
				}
			}
		}

		static void SelectVirtualPrinter(PrinterSettings printreSettings)
		{
			var virtualPrinterNameParts = new[] { "PDF", "XPS" };
			if (virtualPrinterNameParts.Any(namePart => printreSettings.PrinterName.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0))
			{
				return;
			}

			foreach (var namePart in virtualPrinterNameParts)
			{
				var virtualPrinterName = PrinterSettings.InstalledPrinters.Cast<string>().FirstOrDefault(printerName => printerName.IndexOf(namePart, StringComparison.OrdinalIgnoreCase) >= 0);
				if (!string.IsNullOrEmpty(virtualPrinterName))
				{
					printreSettings.PrinterName = virtualPrinterName;
					return;
				}
			}
		}

		#endregion

		#region Print preview

		bool showPreview;

		public void ShowPreview(bool show)
		{
			currentPage = 0;
			showPreview = show;
			UpdatePreviewControl();
		}

		void ResetPreviewControl()
		{
			showPreview = false;
			UpdatePreviewControl();
		}

		void UpdatePreviewControl()
		{
			if (showPreview)
			{
				visualControlPrintPreview.Visible = false;

				originalForPdf = ForPdf;
				ForPdf = false;
				UpdatePagePrintSize(Controller, originalForPdf);

				previewControl = new PrintPreviewControl
				{
					Document = printDocument,
					Dock = DockStyle.Fill
				};

				Controls.Add(previewControl);
				previewControl.BringToFront();
			}
			else
			{
				if (previewControl != null)
				{
					ForPdf = originalForPdf;
					UpdatePagePrintSize(Controller);

					previewControl.Dispose();
					previewControl = null;
				}
				visualControlPrintPreview.Visible = true;
			}
		}

		bool originalForPdf;

		public int PreviewPage
		{
			get { return previewControl?.StartPage + 1 ?? 1; }
			set
			{
				if (previewControl != null && value > 0 && value <= Controller.PagesCount)
				{
					previewControl.StartPage = value - 1;
				}
			}
		}

		public void SetZoom(string zoomValue)
		{
			if (previewControl != null)
			{
				if (zoomValue == Resources.PrintPreviewZoomAuto)
				{
					previewControl.AutoZoom = true;
				}
				else
				{
					double newZoom;
					if (double.TryParse(zoomValue.Replace("%", "").Trim(), out newZoom))
					{
						previewControl.AutoZoom = false;
						previewControl.Zoom = newZoom / 100d;
					}
				}
			}
		}

		#endregion

		#region Print

		public void Print()
		{
			ResetPreviewControl();

			using (var printDialog =
				new PrintDialog
				{
					Document = printDocument,
					AllowSomePages = true,
					AllowCurrentPage = false,
					AllowSelection = false,
					UseEXDialog = true
				})
			{
				if (printDialog.ShowDialog() == DialogResult.OK)
				{
					UpdatePagePrintSize(Controller);
					currentPage = 0;
					printDocument.Print();

					Dispose();
				}
			}
		}

		void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			var realPage = currentPage;
			if (printDocument.PrinterSettings.PrintRange == PrintRange.SomePages)
			{
				realPage += printDocument.PrinterSettings.FromPage - 1;
			}

			using (var painter = new GdiPainter(e.Graphics, IndexedImageExtensions.ToBitmap, true)
				{
					FontFamily = FontHelper.GetFontFamily(Controller.GridPainter.SymbolsFont.Name)
				})
			{
				Controller.PaintPrintPageImage(
					painter,
					realPage,
					new Size(e.PageBounds.Width, e.PageBounds.Height),
					new Rectangle(e.MarginBounds.X, e.MarginBounds.Y, e.MarginBounds.Width, e.MarginBounds.Height),
					AppInfo.AppDescription,
					Controller.SourceImage.Description ?? Resources.UnsavedImageDescription);
			}

			currentPage++;
			e.HasMorePages =
				currentPage < Controller.PagesCount &&
				(printDocument.PrinterSettings.PrintRange == PrintRange.AllPages || realPage < printDocument.PrinterSettings.ToPage - 1);
		}

		int currentPage;

		#endregion

		#region Pdf

		internal string PreselectedPdfFileName { get; set; }

		public void SaveToPdf()
		{
			ResetPreviewControl();
			using (var saveDialog = new SaveFileDialog { Filter = Resources.FileFilterPDF, FilterIndex = 0, FileName = PreselectedPdfFileName })
			{
				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					PreselectedPdfFileName = string.Empty;
					var wasForPdf = ForPdf;

					Cursor = Cursors.WaitCursor;
					try
					{
						var document = new PdfDocument();
						document.Info.Title = Controller.SourceImage.Description;
						document.Info.Author = SystemInformation.UserName;
						document.Info.Creator = AppInfo.AppDescription + " (" + AppInfo.AppVersion + ")";
						document.Info.CreationDate = DateTime.Now;
						document.Info.ModificationDate = document.Info.CreationDate;
						document.Info.Subject = Resources.SaeFileDescription;

						var pageSettings = printDocument.DefaultPageSettings;
						var pageMargins = pageSettings.Margins;

						var pageWidth = new XUnit(pageSettings.PaperSize.Width * 72.0 / 100.0, XGraphicsUnit.Point);
						var pageHeight = new XUnit(pageSettings.PaperSize.Height * 72.0 / 100.0, XGraphicsUnit.Point);
						if (pageSettings.Landscape)
						{
							var x = pageWidth;
							pageWidth = pageHeight;
							pageHeight = x;
						}
						var pageSize = new Size((int)pageWidth.Point, (int)pageHeight.Point);

						var marginsRect = new Rectangle(
							pageMargins.Left * 72 / 100,
							pageMargins.Top * 72 / 100,
							pageSize.Width - (pageMargins.Left + pageMargins.Right) * 72 / 100,
							pageSize.Height - (pageMargins.Top + pageMargins.Bottom) * 72 / 100);

						ForPdf = true;
						UpdatePagePrintSize(Controller);

						using (var form = new Form
						{
							Text = Resources.PrintSavingToPdf,
							Size = new System.Drawing.Size(200, 130),
							TopMost = true,
							FormBorderStyle = FormBorderStyle.FixedDialog,
							ShowIcon = false,
							ControlBox = false
						})
						{
							using (var painter = new PdfSharpPainter(IndexedImageExtensions.ToBitmap)
							{
								FontName = Controller.GridPainter.SymbolsFont.Name,
								FontFamily = FontHelper.GetFontFamily(Controller.GridPainter.SymbolsFont.Name)
							})
							{
								var progressLabel = new Label { Location = new System.Drawing.Point(32, 24), AutoSize = true };
								form.Controls.Add(progressLabel);
								form.Show();

								for (int i = 0; i < Controller.PagesCount; i++)
								{
									progressLabel.Text = string.Format(Resources.PrintPageNo, i + 1, Controller.SourceImage.Description);
									form.Invalidate();
									form.Update();

									var page = document.AddPage();
									page.Orientation = pageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
									page.Width = pageWidth;
									page.Height = pageHeight;

									using (var pdfGraphics = XGraphics.FromPdfPage(page))
									{
										painter.PdfGraphics = pdfGraphics; // Keep same painter for different pages to share fonts

										Controller.PaintPrintPageImage(
											painter,
											i,
											pageSize,
											marginsRect,
											AppInfo.AppDescription,
											Controller.SourceImage.Description ?? Resources.UnsavedImageDescription);
									}
								}
							}

							document.Save(saveDialog.FileName);
						}

						if (File.Exists(saveDialog.FileName))
						{
							Process.Start(saveDialog.FileName);
						}

						Dispose();
					}
					catch (IOException ex)
					{
						MessageBox.Show(Resources.ErrorSavePdf + Environment.NewLine + ex.Message);
						ForPdf = wasForPdf;
						UpdatePagePrintSize(Controller);
						Cursor = DefaultCursor;
					}
				}
			}
		}

		#endregion
	}
}
