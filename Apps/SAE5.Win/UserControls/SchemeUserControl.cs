using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Export;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Painters;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Drawing.Properties;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE.Drawing.UndoRedo;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using SAE5.Win;
using Point = System.Drawing.Point;
using Size = Ravlyk.Common.Size;
using Rectangle = Ravlyk.Common.Rectangle;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class SchemeUserControl : UserControl, ICanClose
	{
		public SchemeUserControl(CodedImage schemeImage)
		{
			schemeImage.CompletePalette();

			SchemeImage = schemeImage;
			SchemeFont = SAEResources.GetImageFont(schemeImage);

			InitializeComponent();
		}

		public CodedImage SchemeImage { get; }
		public TrueTypeFont SchemeFont { get; }

		public bool RequstsReturningToWizard { get; private set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			splitContainerMain.SplitterDistance = Settings.Default.SchemeSplitterMainPosition;
			splitContainerMain.SplitterWidth = 10;
			splitContainerMain.Panel1MinSize = 50;
			splitContainerMain.Panel2MinSize = 200;
			splitContainerMain.UpdateCollapseButton();

			splitContainerInfo.SplitterDistance = Settings.Default.SchemeSplitterInfoPosition;
			splitContainerInfo.SplitterWidth = 8;
			splitContainerInfo.Panel1MinSize = 50;
			splitContainerInfo.Panel2MinSize = 50;

			imageBoxPreview.Controller = new VisualZoomController(SchemeImage);

			var patternGridController = new VisualPatternGridController(SchemeImage, SchemeFont, SAEResources.GetCrossImage());
			patternGridController.UndoRedo.StateChanged += UndoRedo_StateChanged;
			patternGridController.PropertyChanged += SchemeUserControlPropertyChanged;
			scrollControlGrid.Controller = patternGridController;

			paletteUserControl.Controller = new PaletteController(SchemeImage.Palette, SchemeFont);
			paletteUserControl.CompactMode = Settings.Default.SchemePaletteCompactMode;

			scrollControlGrid.Focus();

			ribbon.CaptionBarVisible = false;
			ribbon.Minimized = false;
			ribbon.Height = 120;
			ribbon.OrbText = Resources.FileMenuName;

			buttonUndo2.ToolTipTitle = buttonUndo.ToolTipTitle = buttonUndo.Text;
			buttonRedo2.ToolTipTitle = buttonRedo.ToolTipTitle = buttonRedo.Text;

			switch (patternGridController.PaintMode)
			{
				case PatternGridPainter.StitchesPaintMode.Symbols:
					buttonViewSymbols.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.WhiteSymbols:
					buttonViewDark.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.ColoredSymbols:
					buttonViewColoredSymbols.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.HalfTones:
					buttonViewHalfTone.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.Full:
					buttonViewFull.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.Colors:
					buttonViewColors.Checked = true;
					break;
				case PatternGridPainter.StitchesPaintMode.Cross:
					buttonViewCross.Checked = true;
					break;
			}

			upDownGridCellSize.TextBoxText = patternGridController.CellSize.ToString();
			checkBoxGridRulers.Checked = patternGridController.ShowRulers;
			checkBoxGridLines.Checked = patternGridController.ShowLines;

			using (DisposableLock.Lock(ref zoomChangingLock))
			{
				var fixedZooms = new List<int>(80);
				for (int i = 1, z = 1; i <= 80; i++, z += i <= 40 ? 1 : i <= 60 ? 2 : 3)
				{
					fixedZooms.Add(z);
				}
				zoomSliderGridCellSize.FixedZoomValues = fixedZooms.ToArray();
				zoomSliderGridCellSize.ZoomValue = patternGridController.CellSize;
			}

			ribbonMenuRegister.Visible = !RegistrationHelper.IsRegistered;
			if (RegistrationHelper.IsRegistered)
			{
				ribbon.OrbDropDown.MenuItems.Remove(ribbonMenuRegister);
				ribbonMenuRegister.Dispose();
			}

			if (string.IsNullOrEmpty(SchemeImage.SourceImageFileName) || !File.Exists(SchemeImage.SourceImageFileName))
			{
				buttonBackToWizard.Visible = false;
			}

			panelInfo.Caption = Resources.ImageInfoCaption;
			panelInfo.IsCollapsed = !Settings.Default.SchemeInfoVisible;
			UpdateImageInfo();

			buttonSave.Image = SchemeImage.HasChanges || string.IsNullOrEmpty(SchemeImage.FileName) ? Resources.Save48 : Resources.SaveB48;

			BeginClipboardMonitor();
		}

		void SchemeUserControlPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(VisualPatternGridController.MouseMode):
					switch (PatternGridController.MouseMode)
					{
						case VisualPatternGridController.MouseActionMode.Shift:
							if (!buttonDrawNone.Checked)
							{
								buttonDrawNone_Click(sender, EventArgs.Empty);
							}
							break;
						case VisualPatternGridController.MouseActionMode.Pen:
							if (!buttonDrawPencil.Checked)
							{
								buttonDrawPencil_Click(sender, EventArgs.Empty);
							}
							break;
						case VisualPatternGridController.MouseActionMode.Fill:
							if (!buttonDrawFill.Checked)
							{
								buttonDrawFill_Click(sender, EventArgs.Empty);
							}
							break;
						case VisualPatternGridController.MouseActionMode.Select:
							if (!buttonDrawSelect.Checked)
							{
								buttonDrawSelect_Click(sender, EventArgs.Empty);
							}
							break;
						case VisualPatternGridController.MouseActionMode.MoveSelection:
							buttonDrawNone.Checked = false;
							buttonDrawPencil.Checked = false;
							buttonDrawFill.Checked = false;
							buttonDrawSelect.Checked = false;
							UndoRedo_StateChanged(sender, EventArgs.Empty);
							break;
					}
					UpdateCutCopyButtons();
					break;
				case nameof(VisualPatternGridController.SelectedRect):
					UpdateCutCopyButtons();
					break;
			}
		}

		void UpdateCutCopyButtons()
		{
			buttonCut.Enabled = buttonCopy.Enabled =
				PatternGridController.MouseMode == VisualPatternGridController.MouseActionMode.Select &&
				PatternGridController.SelectedRect.Width > 0 && PatternGridController.SelectedRect.Height > 0;
		}

		VisualPatternGridController PatternGridController => scrollControlGrid.Controller as VisualPatternGridController;

		void UpdateImageInfo()
		{
			labelFileInfo.Text = string.Format(Resources.ImageInfoLabel,
				SchemeImage.Description,
				SchemeImage.Size.Width, SchemeImage.Size.Height,
				SchemeImage.Palette.Name,
				SchemeImage.Palette.Count);
		}

		#region Save

		void buttonSave_Click(object sender, EventArgs e)
		{
			Save(SchemeImage.FileName);
		}

		void buttonSaveAs_Click(object sender, EventArgs e)
		{
			Save(null);
		}

		void ribbonButtonOrbPng_Click(object sender, EventArgs e)
		{
			Save(null, SaveFormat.Png);
		}

		void ribbonButtonOrbJpg_Click(object sender, EventArgs e)
		{
			Save(null, SaveFormat.Jpg);
		}

		void ribbonButtonOrbEmf_Click(object sender, EventArgs e)
		{
			Save(null, SaveFormat.Emf);
		}

		void ribbonButtonOrbThumbnail_Click(object sender, EventArgs e)
		{
			Save(null, SaveFormat.PngThumbnail);
		}

		void ribbonButtonOrbXls_Click(object sender, EventArgs e)
		{
			Save(null, SaveFormat.Excel);
		}

		bool Save(string fileName, SaveFormat initialFormat = SaveFormat.Any)
		{
			try
			{
				var format = SaveFormat.Sa4;

				if (string.IsNullOrEmpty(fileName))
				{
					using (var saveDialog = new SaveFileDialog())
					{
						saveDialog.Filter = GetSaveAsFilter();
						saveDialog.FilterIndex = SaveFormatToIndex(initialFormat) + 1;
						saveDialog.OverwritePrompt = true;
						if (saveDialog.ShowDialog() == DialogResult.OK)
						{
							fileName = saveDialog.FileName;
							format = IndexToSaveFormat(saveDialog.FilterIndex - 1);
						}
						else
						{
							return false;
						}
					}
				}

				SaveAs(fileName, format);
				return true;
			}
			catch (Exception ex) when (ex is IOException || ex is NotSupportedException)
			{
				MessageBox.Show(Resources.ErrorSaveSchema + Environment.NewLine + ex.Message);
				return false;
			}
		}

		void SaveAs(string fileName, SaveFormat format)
		{
			if (format == SaveFormat.Sa4)
			{
				SaveSa4(fileName);
			}
			else if (format == SaveFormat.PngThumbnail)
			{
				SaveThumbnail(fileName);
			}
			else if (format == SaveFormat.Png || format == SaveFormat.Jpg)
			{
				SaveImage(fileName, format);
			}
			else if (format == SaveFormat.Emf)
			{
				SaveEmf(fileName);
			}
			else if (format == SaveFormat.Pdf)
			{
				SavePdf(fileName);
			}
			else if (format == SaveFormat.Excel)
			{
				SaveExcel(fileName);
			}
		}

		#region Save in format

		void SaveSa4(string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
			{
				SchemeImage.SaveToStream(stream);
				SchemeImage.FileName = fileName;
				UpdateImageInfo();
				//PatternGridController.UndoRedo.ClearCache();
				SchemeImage.HasChanges = false;
				buttonSave.Image = Resources.SaveB48;
				Settings.Default.AddLastOpenFile(fileName);
			}
		}

		void SaveThumbnail(string fileName)
		{
			using (var bitmap = SchemeImage.ToBitmap())
			{
				bitmap.Save(fileName);
			}
		}

		void SaveImage(string fileName, SaveFormat format)
		{
			var extraWidth = PatternGridController.GridPainter.RulerWidth + PatternGridController.GridPainter.LineWidth;
			var imageSize = new Size(
				SchemeImage.Size.Width * PatternGridController.CellSize + extraWidth,
				SchemeImage.Size.Height * PatternGridController.CellSize + extraWidth);
			var image = new IndexedImage { Size = imageSize };
			var painter = new IndexedImagePainter { Canvas = image, SymbolsFont = SchemeFont, SupportLineWidth = true };
			PaintScheme(painter, imageSize);
			using (var bitmap = image.ToBitmap())
			{
				SaveBitmap(bitmap, fileName, format);
			}

			var colorsTableRowHeight = PatternGridController.CellSize * 4 / 3;
			var colorsImageSize = new Size(
				VisualPrintPreviewController.GetColorRowWidth(colorsTableRowHeight),
				colorsTableRowHeight * (SchemeImage.Palette.Count + 1) + 1);
			var colorsImage = new IndexedImage { Size = colorsImageSize };
			painter.Canvas = colorsImage;
			PaintColorsTable(painter, colorsImageSize, colorsTableRowHeight);
			using (var bitmap = colorsImage.ToBitmap())
			{
				SaveBitmap(bitmap, Path.ChangeExtension(fileName, ".Colors" + Path.GetExtension(fileName)), format);
			}
		}

		void PaintScheme(IPainter painter, Size imageSize)
		{
			var clipRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
			using (painter.Clip(clipRect))
			{
				PatternGridController.GridPainter.Paint(painter, imageSize, clipRect, new Common.Point(0, 0));
			}
		}

		void PaintColorsTable(IPainter painter, Size imageSize, int colorsTableRowHeight)
		{
			var clipRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
			using (painter.Clip(clipRect))
			{
				painter.FillRectangle(0, 0, clipRect.Width, clipRect.Height, PatternGridController.GridPainter.WhiteColorArgb);
				VisualPrintPreviewController.DrawColorsTable(painter, new Common.Point(0, 0),
					paletteUserControl.Controller?.OrderedColors ?? SchemeImage.Palette.OrderByDarknes().Cast<CodedColor>().ToList(),
					1, colorsTableRowHeight,
					PatternGridController.GridPainter.NumbersArgb, PatternGridController.GridPainter.LineArgb, PatternGridController.GridPainter.Line10Argb);
			}
		}

		static void SaveBitmap(Image bitmap, string fileName, SaveFormat format)
		{
			switch (format)
			{
				case SaveFormat.Png:
					bitmap.Save(fileName, ImageFormat.Png);
					break;
				case SaveFormat.Jpg:
					bitmap.Save(fileName, ImageFormat.Jpeg);
					break;
				default:
					bitmap.Save(fileName);
					break;
			}
		}

		void SavePdf(string fileName)
		{
			buttonPdfPreview_Click(buttonPdfPreview, EventArgs.Empty);
			printUserControl.PreselectedPdfFileName = fileName;
		}

		void SaveEmf(string fileName)
		{
			var extraWidth = PatternGridController.GridPainter.RulerWidth + PatternGridController.GridPainter.LineWidth;
			var imageSize = new Size(
				SchemeImage.Size.Width * PatternGridController.CellSize + extraWidth,
				SchemeImage.Size.Height * PatternGridController.CellSize + extraWidth);
			using (var g0 = CreateGraphics())
			{
				using (var painter = new EmfPainter(fileName, g0.GetHdc(), imageSize, IndexedImageExtensions.ToBitmap)
					{
						FontFamily = FontHelper.GetFontFamily(PatternGridController.GridPainter.SymbolsFont.Name)
					})
				{
					PaintScheme(painter, imageSize);
				}
				g0.ReleaseHdc();
			}

			var colorsTableRowHeight = PatternGridController.CellSize * 4 / 3;
			imageSize = new Size(
				VisualPrintPreviewController.GetColorRowWidth(colorsTableRowHeight),
				colorsTableRowHeight * (SchemeImage.Palette.Count + 1) + 1);
			using (var g0 = CreateGraphics())
			{
				using (var painter = new EmfPainter(Path.ChangeExtension(fileName, ".Colors" + Path.GetExtension(fileName)), g0.GetHdc(), imageSize, IndexedImageExtensions.ToBitmap)
					{
						FontFamily = FontHelper.GetFontFamily(PatternGridController.GridPainter.SymbolsFont.Name)
					})
				{
					PaintColorsTable(painter, imageSize, colorsTableRowHeight);
				}
				g0.ReleaseHdc();
			}
		}

		void SaveExcel(string fileName)
		{
			Cursor = Cursors.WaitCursor;
			try
			{
				using (var form = new Form
				{
					Text = Resources.FileSavingToExcel,
					Size = new System.Drawing.Size(200, 130),
					TopMost = true,
					FormBorderStyle = FormBorderStyle.FixedDialog,
					ShowIcon = false,
					ControlBox = false
				})
				{
					var progressLabel = new Label { Location = new Point(32, 24), Text = Resources.FileSavingToExcelLabel, AutoSize = true };
					form.Controls.Add(progressLabel);
					form.Show();
					form.Invalidate();
					form.Update();

					ExcelExporter.ExportToExcel(SchemeImage, fileName, PatternGridController.GridPainter, paletteUserControl.Controller?.OrderedColors ?? SchemeImage.Palette.OrderByDarknes().Cast<CodedColor>().ToList());
				}

				if (File.Exists(fileName))
				{
					Process.Start(fileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(Resources.ErrorCannotSaveFile + Environment.NewLine + ex.Message);
			}
			finally
			{
				Cursor = DefaultCursor;
			}
		}

		#endregion

		#region File formats

		enum SaveFormat
		{
			Any,
			Sa4,
			Png,
			Jpg,
			Emf,
			PngThumbnail,
			Pdf,
			Excel
		}

		static string GetSaveAsFilter()
		{
			var sb = new StringBuilder();
			sb.Append(Resources.FileFilterSAE);
			sb.Append('|').AppendFormat(Resources.FileFilterImage, "PNG", "png");
			sb.Append('|').AppendFormat(Resources.FileFilterImage, "JPG", "jpg");
			sb.Append('|').AppendFormat(Resources.FileFilterImage, "EMF", "emf");
			sb.Append('|').AppendFormat(Resources.FileFilterThumbnail, "PNG", "png");
			sb.Append('|').Append(Resources.FileFilterPDF);
			sb.Append('|').Append(Resources.FileFilterExcel);
			return sb.ToString();
		}

		int SaveFormatToIndex(SaveFormat format)
		{
			switch (format)
			{
				case SaveFormat.Sa4:
					return 0;
				case SaveFormat.Png:
					return 1;
				case SaveFormat.Jpg:
					return 2;
				case SaveFormat.Emf:
					return 3;
				case SaveFormat.PngThumbnail:
					return 4;
				case SaveFormat.Pdf:
					return 5;
				case SaveFormat.Excel:
					return 6;
				default:
					return -1;
			}
		}

		SaveFormat IndexToSaveFormat(int filterIndex)
		{
			switch (filterIndex)
			{
				case 0:
					return SaveFormat.Sa4;
				case 1:
					return SaveFormat.Png;
				case 2:
					return SaveFormat.Jpg;
				case 3:
					return SaveFormat.Emf;
				case 4:
					return SaveFormat.PngThumbnail;
				case 5:
					return SaveFormat.Pdf;
				case 6:
					return SaveFormat.Excel;
				default:
					throw new NotSupportedException("Selected file format is not supported.");
			}
		}

		#endregion

		#endregion

		#region Close

		void buttonHome_Click(object sender, EventArgs e)
		{
			var cancelArgs = new CancelEventArgs(false);
			CheckIfSaveNeeded(Resources.QuestionSaveBeforeReturning, cancelArgs);
			if (!cancelArgs.Cancel)
			{
				Dispose();
			}
		}

		void buttonBackToWizard_Click(object sender, EventArgs e)
		{
			var cancelArgs = new CancelEventArgs(false);
			CheckIfSaveNeeded(Resources.QuestionSaveBeforeBackToWizard, cancelArgs);
			if (!cancelArgs.Cancel)
			{
				RequstsReturningToWizard = true;
				Dispose();
			}
		}

		void ICanClose.OnClosing(CancelEventArgs e)
		{
			printUserControl?.Dispose();
			CheckIfSaveNeeded(Resources.QuestionSaveBeforeClosing, e);
		}

		void CheckIfSaveNeeded(string prompt, CancelEventArgs e)
		{
			if (PatternGridController.MouseMode == VisualPatternGridController.MouseActionMode.MoveSelection)
			{
				PatternGridController.FinishMoveSelection();
			}

			var hasChanges = string.IsNullOrEmpty(SchemeImage.FileName) || SchemeImage.HasChanges;
			if (hasChanges)
			{
				var answer = MessageBox.Show(prompt, AppInfo.AppDescription, MessageBoxButtons.YesNoCancel);
				if (answer == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
				else if (answer == DialogResult.Yes)
				{
					e.Cancel = !Save(SchemeImage.FileName);
				}
			}

			if (!e.Cancel)
			{
				Settings.Default.SchemeSplitterMainPosition = splitContainerMain.SplitterDistance;
				Settings.Default.SchemeSplitterInfoPosition = splitContainerInfo.SplitterDistance;
				Settings.Default.SchemeInfoVisible = !panelInfo.IsCollapsed;
				Settings.Default.SchemePaletteCompactMode = paletteUserControl.CompactMode;
				Settings.Default.Save();
				PatternGridController.GridPainter.SaveSettings();
				paletteUserControl.Controller.SaveSettings();
			}
		}

		void ribbonOrbOptionButtonExit_Click(object sender, EventArgs e)
		{
			FindForm()?.Close();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				clipboardMonitor?.Dispose();
				clipboardMonitor = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Ribbon - View

		void buttonViewSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Symbols;
			buttonViewSymbols.Checked = true;
		}

		void buttonViewDark_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.WhiteSymbols;
			buttonViewDark.Checked = true;
		}

		void buttonViewColoredSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.ColoredSymbols;
			buttonViewColoredSymbols.Checked = true;
		}

		void buttonViewHalfTone_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.HalfTones;
			buttonViewHalfTone.Checked = true;
		}

		void buttonViewFull_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Full;
			buttonViewFull.Checked = true;
		}

		void buttonViewColors_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Colors;
			buttonViewColors.Checked = true;
		}

		void buttonViewCross_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Cross;
			buttonViewCross.Checked = true;
		}

		void checkBoxGridRulers_Click(object sender, EventArgs e)
		{
			PatternGridController.ShowRulers = checkBoxGridRulers.Checked;
		}

		void checkBoxGridLines_Click(object sender, EventArgs e)
		{
			PatternGridController.ShowLines = checkBoxGridLines.Checked;
		}

		void upDownGridCellSize_DownButtonClicked(object sender, MouseEventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize--;
					upDownGridCellSize.TextBoxText = PatternGridController.CellSize.ToString();
					zoomSliderGridCellSize.ZoomValue = PatternGridController.CellSize;
				}
			}
		}

		void upDownGridCellSize_UpButtonClicked(object sender, MouseEventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize++;
					upDownGridCellSize.TextBoxText = PatternGridController.CellSize.ToString();
					zoomSliderGridCellSize.ZoomValue = PatternGridController.CellSize;
				}
			}
		}

		void zoomSliderGridCellSize_ZoomChanged(object sender, EventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize = zoomSliderGridCellSize.ZoomValue;
					zoomSliderGridCellSize.ZoomValue = PatternGridController.CellSize;
					upDownGridCellSize.TextBoxText = PatternGridController.CellSize.ToString();
				}
			}
		}

		DisposableLock zoomChangingLock;

		#endregion

		#region Language

		void menuItemLanguageEn_Click(object sender, EventArgs e)
		{
			SetSelectedLanguage("en", menuItemLanguageEn);
		}

		void menuItemLanguageRu_Click(object sender, EventArgs e)
		{
			SetSelectedLanguage("ru", menuItemLanguageRu);
		}

		void menuItemLanguageUa_Click(object sender, EventArgs e)
		{
			SetSelectedLanguage("uk", menuItemLanguageUa);
		}

		void SetSelectedLanguage(string culture, RibbonItem menuItem)
		{
			AppInfo.SetSelectedLanguage(culture, true);
			menuItem.Checked = true;
		}

		void ribbonMenuItemLanguage_DropDownShowing(object sender, EventArgs e)
		{
			menuItemLanguageEn.Checked = false;
			menuItemLanguageRu.Checked = false;
			menuItemLanguageUa.Checked = false;

			switch (Settings.Default.Locale)
			{
				case "ru":
					menuItemLanguageRu.Checked = true;
					break;
				case "uk":
					menuItemLanguageUa.Checked = true;
					break;
				default:
					menuItemLanguageEn.Checked = true;
					break;
			}
		}

		#endregion

		#region Ribbon - Print Preview

		PrintUserControl printUserControl;

		void buttonPrint_Click(object sender, EventArgs e)
		{
			comboBoxPrinter.Visible = ribbonSeparatorPrinter.Visible = true;
			buttonPrintPrint.Visible = true;
			buttonSaveToPdf.Visible = false;
			SetupPrintPreviewControls();
		}

		void SetupPrintPreviewControls()
		{
			splitContainerMain.Visible = false;

			printUserControl = new PrintUserControl
			{
				ForPdf = false,
				Dock = DockStyle.Fill,
				Controller = new VisualPrintPreviewController(SchemeImage, PatternGridController.GridPainter, paletteUserControl.Controller?.OrderedColors)
			};

			Controls.Add(printUserControl);
			printUserControl.BringToFront();

			buttonPreview.Checked = false;
			ribbonContextPrint.Visible = true;

			comboBoxPrinter.DropDownItems.Clear();
			string defaultPrinter;
			foreach (var availablePrinter in printUserControl.GetAvailablePrinters(out defaultPrinter).OrderBy(p => p))
			{
				comboBoxPrinter.DropDownItems.Add(new RibbonButton(availablePrinter));
			}
			comboBoxPrinter.TextBoxText = defaultPrinter;

			upDownPrintCellSize.TextBoxText = printUserControl.CellSize.ToString("#0.000");
			comboBoxCellSizeUnits.TextBoxText = printUserControl.Unit.ToString();

			printUserControl.Disposed += PrintUserControl_Disposed;
		}

		void buttonPdfPreview_Click(object sender, EventArgs e)
		{
			comboBoxPrinter.Visible = ribbonSeparatorPrinter.Visible = false;
			buttonPrintPrint.Visible = false;
			buttonSaveToPdf.Visible = true;
			SetupPrintPreviewControls();
			printUserControl.ForPdf = true;
			printUserControl.UpdatePagePrintSize(printUserControl.Controller);
		}

		void PrintUserControl_Disposed(object sender, EventArgs e)
		{
			var activeTab = ribbon.ActiveTab;
			ribbonContextPrint.Visible = false;
			splitContainerMain.Visible = true;
			ControlUtilities.UnsubscribeDisposed(sender, PrintUserControl_Disposed);
			printUserControl = null;
			if (activeTab.Visible)
			{
				ribbon.ActiveTab = activeTab;
			}
		}

		void ribbon_ActiveTabChanged(object sender, EventArgs e)
		{
			if (ribbon.ActiveTab != ribbonTabPrint)
			{
				printUserControl?.Dispose();
			}
			if (ribbon.ActiveTab != ribbonTabDraw)
			{
				buttonDrawNone.Checked = true;
				buttonDrawNone_Click(sender, e);
			}
		}

		void ribbon_OrbClicked(object sender, EventArgs e)
		{
			if (printUserControl != null)
			{
				printUserControl.Dispose();
				ribbon.ShowOrbDropDown();
			}
		}

		void buttonPrintBack_Click(object sender, EventArgs e)
		{
			printUserControl?.Dispose();
		}

		void comboBoxPrinter_TextBoxTextChanged(object sender, EventArgs e)
		{
			printUserControl.SetPrinter(comboBoxPrinter.TextBoxText);
			ResetPreviewControls();
		}

		void upDownPrintCellSize_UpButtonClicked(object sender, MouseEventArgs e)
		{
			printUserControl.CellSize += printUserControl.Unit == PrintUserControl.SizeUnit.Inch ? 0.01m : 0.25m;
			upDownPrintCellSize.TextBoxText = printUserControl.CellSize.ToString("#0.000");
			ResetPreviewControls();
		}

		void upDownPrintCellSize_DownButtonClicked(object sender, MouseEventArgs e)
		{
			printUserControl.CellSize -= printUserControl.Unit == PrintUserControl.SizeUnit.Inch ? 0.01m : 0.25m;
			upDownPrintCellSize.TextBoxText = printUserControl.CellSize.ToString("#0.000");
			ResetPreviewControls();
		}

		void upDownPrintCellSize_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownPrintCellSize.TextBoxText, out value))
			{
				printUserControl.CellSize = value;
			}
			upDownPrintCellSize.TextBoxText = printUserControl.CellSize.ToString("#0.000");
		}

		void comboBoxCellSizeUnits_TextBoxTextChanged(object sender, EventArgs e)
		{
			PrintUserControl.SizeUnit unit;
			if (Enum.TryParse(comboBoxCellSizeUnits.TextBoxText, out unit))
			{
				if (printUserControl.Unit != unit)
				{
					printUserControl.Unit = unit;
				}
			}
			var newValue = printUserControl.Unit.ToString();
			if (comboBoxCellSizeUnits.TextBoxText != newValue)
			{
				comboBoxCellSizeUnits.TextBoxText = newValue;
			}
			upDownPrintCellSize.TextBoxText = printUserControl.CellSize.ToString("#0.000");
		}

		void buttonPageSettings_Click(object sender, EventArgs e)
		{
			printUserControl.OpenPageSetup();
			ResetPreviewControls();
		}

		void buttonPreview_Click(object sender, EventArgs e)
		{
			buttonPreview.Checked = !buttonPreview.Checked;
			printUserControl.ShowPreview(buttonPreview.Checked);
			UpdatePreviewControls();
		}

		void UpdatePreviewControls()
		{
			upDownPreviewPage.Visible = buttonPreview.Checked;
			comboBoxPreviewZoom.Visible = buttonPreview.Checked;

			upDownPreviewPage.TextBoxText = "1";
			comboBoxPreviewZoom.TextBoxText = Resources.PrintPreviewZoomAuto;
		}

		void ResetPreviewControls()
		{
			buttonPreview.Checked = false;
			UpdatePreviewControls();
		}

		void buttonPrintPrint_Click(object sender, EventArgs e)
		{
			ResetPreviewControls();
			printUserControl.Print();
		}

		void buttonSaveToPdf_Click(object sender, EventArgs e)
		{
			ResetPreviewControls();
			printUserControl.SaveToPdf();
		}

		void upDownPreviewPage_UpButtonClicked(object sender, MouseEventArgs e)
		{
			printUserControl.PreviewPage += 1;
			upDownPreviewPage.TextBoxText = printUserControl.PreviewPage.ToString();
		}

		void upDownPreviewPage_DownButtonClicked(object sender, MouseEventArgs e)
		{
			printUserControl.PreviewPage -= 1;
			upDownPreviewPage.TextBoxText = printUserControl.PreviewPage.ToString();
		}

		void comboBoxPreviewZoom_TextBoxTextChanged(object sender, EventArgs e)
		{
			printUserControl.SetZoom(comboBoxPreviewZoom.TextBoxText);
		}

		#endregion

		#region Draw

		void buttonDrawNone_Click(object sender, EventArgs e)
		{
			PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Shift;
			buttonDrawNone.Checked = true;
			scrollControlGrid.VisualControl.OverrideCursor = null;
		}

		void buttonDrawPencil_Click(object sender, EventArgs e)
		{
			PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Pen;
			buttonDrawPencil.Checked = true;
			scrollControlGrid.VisualControl.OverrideCursor = CustomCursorHelper.GetCursor(CustomCursorHelper.CursorType.Pen, PatternGridController.MouseColor?.Argb ?? 0x00ffffff);
		}

		void buttonDrawFill_Click(object sender, EventArgs e)
		{
			PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Fill;
			buttonDrawFill.Checked = true;
			scrollControlGrid.VisualControl.OverrideCursor = CustomCursorHelper.GetCursor(CustomCursorHelper.CursorType.Fill, PatternGridController.MouseColor?.Argb ?? 0x00ffffff);
		}

		void paletteUserControl_SelectedColorChanged(object sender, PaletteUserControl.ColorEventArgs e)
		{
			PatternGridController.MouseColor = e.Color;

			buttonDrawPencil.Enabled = buttonDrawFill.Enabled = buttonChangeThreadSymbol.Enabled = buttonChangeThreadColor.Enabled = e.Color != null;
			buttonRemoveThread.Enabled = e.Color != null && e.Color.OccurrencesCount == 0;
			buttonMergeThreads.Enabled = paletteUserControl.SelectedColors.Take(2).Count() > 1;

			if (e.Color == null)
			{
				buttonDrawNone.Checked = true;
				buttonDrawNone_Click(sender, e);
			}
		}

		void UndoRedo_StateChanged(object sender, EventArgs e)
		{
			buttonUndo2.Enabled = buttonUndo.Enabled = PatternGridController.UndoRedo.CanUndo;
			buttonUndo2.ToolTip = buttonUndo.ToolTip = buttonUndo.Enabled ? PatternGridController.UndoRedo.UndoDescription : "";

			buttonRedo2.Enabled = buttonRedo.Enabled = PatternGridController.UndoRedo.CanRedo;
			buttonRedo2.ToolTip = buttonRedo.ToolTip = buttonRedo.Enabled ? PatternGridController.UndoRedo.RedoDescription : "";

			buttonSave.Image = Resources.Save48;
		}

		void buttonUndo_Click(object sender, EventArgs e)
		{
			PatternGridController.Undo();
			paletteUserControl.UpdatePalette();
			paletteUserControl.ResetSelection();
			UpdateImageInfo();
		}

		void buttonRedo_Click(object sender, EventArgs e)
		{
			PatternGridController.Redo();
			paletteUserControl.UpdatePalette();
			paletteUserControl.ResetSelection();
			UpdateImageInfo();
		}

		void buttonDrawSelect_Click(object sender, EventArgs e)
		{
			PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Select;
			buttonDrawSelect.Checked = true;
			scrollControlGrid.VisualControl.OverrideCursor = null;
		}

		void buttonDrawSelectAll_Click(object sender, EventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				buttonDrawSelect_Click(sender, e);
				PatternGridController.SelectedRect = new Rectangle(0, 0, SchemeImage.Size.Width, SchemeImage.Size.Width);
			}
		}

		void imageBoxPreview_ImageTouched(object sender, VisualControl.ControllerTouchedEventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				PatternGridController.HPosition = e.TouchPoint.X - PatternGridController.BigHStep / 2;
				PatternGridController.VPosition = e.TouchPoint.Y - PatternGridController.BigVStep / 2;
			}
		}

		#endregion

		#region Selection

		void buttonCut_Click(object sender, EventArgs e)
		{
			if (CopyImageToClipbord())
			{
				var backgroundColor = GetCurrentColor(Resources.DialogHintSelectBackgroundThread);
				if (backgroundColor != null)
				{
					using (SchemeImage.Palette.SuppressRemoveColorsWithoutOccurrences())
					using (PatternGridController.UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionCut))
					using (PatternGridController.SuspendUpdateVisualImage())
					{
						ImagePainter.FillRect(SchemeImage, PatternGridController.SelectedRect, backgroundColor);
					}

					PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Shift;
				}
			}
		}

		void buttonCopy_Click(object sender, EventArgs e)
		{
			if (CopyImageToClipbord())
			{
				PatternGridController.MouseMode = VisualPatternGridController.MouseActionMode.Shift;
			}
		}

		bool CopyImageToClipbord()
		{
			if (PatternGridController.SelectedRect.Width <= 0 || PatternGridController.SelectedRect.Height <= 0)
			{
				return false;
			}

			var cutImage = new CodedImage();
			ImageCropper.Crop(SchemeImage, PatternGridController.SelectedRect, ImageCropper.CropKind.Rectangle, cutImage);
			cutImage.Palette = SchemeImage.Palette.Clone();
			cutImage.CompletePalette();

			var dataObject = new ImageDataObject(cutImage);

			try
			{
				Clipboard.SetDataObject(dataObject, false, 2, 1);
			}
			catch (ExternalException)
			{
				System.Media.SystemSounds.Beep.Play();
				return false;
			}

			return true;
		}

		void buttonPaste_Click(object sender, EventArgs e)
		{
			CodedImage block = ImageDataObject.GetImageFromDataObject(Clipboard.GetDataObject());
			if (block == null)
			{
				System.Media.SystemSounds.Beep.Play();
				return;
			}

			scrollControlGrid.VisualControl.OverrideCursor = null;
			PatternGridController.InsertBlockAndBeginMoveSelection(block);
		}

		#region Clipboard monitor

		void BeginClipboardMonitor()
		{
			clipboardMonitor = ClipboardMonitor.StartMonitor(this, ClipboardUpdated);
			ClipboardUpdated();
		}

		void ClipboardUpdated()
		{
			buttonPaste.Enabled = clipboardMonitor == null || Clipboard.ContainsData(ImageDataObject.DataFormatType);
		}

		IDisposable clipboardMonitor;

		#endregion

		#endregion

		#region Threads

		void buttonChangeThreadSymbol_Click(object sender, EventArgs e)
		{
			var selectedColor = PatternGridController.MouseColor;
			if (selectedColor == null)
			{
				return;
			}

			using (var symbolsDialog = new SymbolsDialog(SchemeFont, SchemeImage.Palette.Cast<CodedColor>().Select(color => color.SymbolChar).ToList()))
			{
				if (symbolsDialog.ShowDialog(this) == DialogResult.OK)
				{
					SchemeImage.Palette.ChangeColorAttributes(
						selectedColor,
						symbolsDialog.SelectedSymbol,
						selectedColor.ColorCode,
						selectedColor.ColorName);
				}
			}
		}

		void buttonChangeThreadColor_Click(object sender, EventArgs e)
		{
			var targetColors = paletteUserControl.SelectedColors.ToList();
			if (targetColors.Count == 0)
			{
				return;
			}

			var selectedColor = SelectColor(targetColors, null);
			if (selectedColor != null)
			{
				using (PatternGridController.UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionChangeThread))
				{
					CodedColor newColor = null;

					using (PatternGridController.SuspendUpdateVisualImage())
					{
						foreach (var color in SchemeImage.Palette)
						{
							if (color.Equals(selectedColor))
							{
								newColor = color;
								if (newColor.ColorCode != selectedColor.ColorCode)
								{
									SchemeImage.Palette.ChangeColorAttributes(
										newColor,
										newColor.SymbolChar,
										selectedColor.ColorCode,
										selectedColor.ColorName);
								}
								break;
							}
						}

						if (newColor == null)
						{
							newColor = selectedColor;
							newColor.SymbolChar = targetColors[0].SymbolChar;
							newColor = (CodedColor)SchemeImage.Palette.Add(newColor);
						}

						foreach (var color in targetColors)
						{
							paletteUserControl.Controller.ReplaceColor(SchemeImage, color, newColor);
						}

						SchemeImage.TriggerImageChanged();
						if (targetColors.Count > 1 && newColor != null)
						{
							PatternGridController.MouseColor = newColor;
							buttonChangeThreadSymbol_Click(sender, e);
						}
						paletteUserControl.ResetSelection();
						UpdateImageInfo();
					}
				}
			}
		}

		void buttonThreadsShowAll_Click(object sender, EventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				foreach (var color in SchemeImage.Palette)
				{
					SchemeImage.Palette.ChangeColorVisibility(color, true);
				}
				paletteUserControl.UpdatePalette();
			}
		}

		void buttonThreadsHideAll_Click(object sender, EventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				foreach (var color in SchemeImage.Palette)
				{
					SchemeImage.Palette.ChangeColorVisibility(color, false);
				}
				paletteUserControl.UpdatePalette();
			}
		}

		void buttonThreadsShowSelected_Click(object sender, EventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				foreach (var color in paletteUserControl.SelectedColors)
				{
					SchemeImage.Palette.ChangeColorVisibility(color, true);
				}
				paletteUserControl.UpdatePalette();
			}
		}

		void buttonThreadsHideSelected_Click(object sender, EventArgs e)
		{
			using (PatternGridController.SuspendUpdateVisualImage())
			{
				foreach (var color in paletteUserControl.SelectedColors)
				{
					SchemeImage.Palette.ChangeColorVisibility(color, false);
				}
				paletteUserControl.UpdatePalette();
			}
		}

		void buttonAddThread_Click(object sender, EventArgs e)
		{
			var selectedColor = SelectColor(null, Resources.DialogHintAddNewThread);
			if (selectedColor != null && SchemeImage.Palette[selectedColor.Argb] == null)
			{
				AddThread(selectedColor);
			}
		}

		void AddThread(CodedColor selectedColor)
		{
			using (PatternGridController.UndoRedo.BeginMultiActionsUndoRedoStep(UndoRedoProvider.UndoRedoActionAddTherad))
			{
				var newColor = (CodedColor)SchemeImage.Palette.Add(selectedColor);
				SchemeImage.Palette.ChangeColorVisibility(newColor, true);
				PatternGridController.MouseColor = newColor;
				buttonChangeThreadSymbol_Click(buttonChangeThreadSymbol, EventArgs.Empty);
				paletteUserControl.UpdatePalette();
				paletteUserControl.ResetSelection();
				UpdateImageInfo();
			}
		}

		void buttonRemoveThread_Click(object sender, EventArgs e)
		{
			if (PatternGridController.MouseColor != null)
			{
				SchemeImage.Palette.Remove(PatternGridController.MouseColor.GetHashCode());
				paletteUserControl.UpdatePalette();
				paletteUserControl.ResetSelection();
				UpdateImageInfo();
			}
		}

		void buttonManagePalettes_Click(object sender, EventArgs e)
		{
			using (var threadsManageDialog = new ThreadsManagementDialog())
			{
				threadsManageDialog.ShowDialog(this);
			}
		}

		CodedColor SelectColor(ICollection<CodedColor> targetColors, string hint)
		{
			using (var threadsDialog = new ThreadsDialog(SchemeImage.Palette.Name, targetColors, SchemeImage.Palette.Cast<CodedColor>().ToList()))
			{
				if (!string.IsNullOrEmpty(hint))
				{
					threadsDialog.ChangeHint(hint);
				}
				return threadsDialog.ShowDialog(this) == DialogResult.OK ? threadsDialog.SelectedColor : null;
			}
		}

		CodedColor GetCurrentColor(string hint = null)
		{
			if (PatternGridController.MouseColor == null)
			{

				var selectedColor = SelectColor(null, hint);
				if (selectedColor != null)
				{
					var existingColor = SchemeImage.Palette[selectedColor.Argb];
					if (existingColor == null)
					{
						AddThread(selectedColor);
					}
					else
					{
						PatternGridController.MouseColor = existingColor;
					}
				}
			}

			return PatternGridController.MouseColor;
		}

		#endregion

		#region Registration

		void ribbonMenuIRegister_Click(object sender, EventArgs e)
		{
			RegistrationHelper.ShowRegisterDialog(this);
			if (RegistrationHelper.IsRegistered)
			{
				ribbonMenuRegister.Visible = false;
			}
		}

		#endregion

		#region Check for updates

		void ribbonOrbMenuItemAbout_Click(object sender, EventArgs e)
		{
			using (var aboutDialog = new AboutDialog())
			{
				aboutDialog.ShowDialog(this);
			}
		}

		void ribbonOrbMenuItemCheckForUpdates_Click(object sender, EventArgs e)
		{
			AppInfo.CheckForUpdates(UpdateAvailable, UpdatesCheckError, true);
		}

		void UpdateAvailable(string details)
		{
			if (string.IsNullOrEmpty(details))
			{
				return;
			}

			MainForm.ShowUpdateDialog(details, this);
		}

		void UpdatesCheckError(Exception ex)
		{
			MessageBox.Show(Resources.ErrorCheckForUpdates + ": " + ex.Message, AppInfo.AppDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion

		#region Options

		void ribbonOrbMenuItemOptions_Click(object sender, EventArgs e)
		{
			using (var optionsDialog = new OpitonsDialog())
			{
				if (optionsDialog.ShowDialog(this) == DialogResult.OK)
				{
					using (PatternGridController.SuspendUpdateVisualImage())
					{
						PatternGridController.GridPainter.LineArgb = GridPainterSettings.Default.LineArgb.ToArgb();
						PatternGridController.GridPainter.Line5Argb = GridPainterSettings.Default.Line5Argb.ToArgb();
						PatternGridController.GridPainter.Line10Argb = GridPainterSettings.Default.Line10Argb.ToArgb();
						PatternGridController.GridPainter.NumbersArgb = GridPainterSettings.Default.NumbersArgb.ToArgb();
						PatternGridController.GridPainter.Line10DoubleWidth = GridPainterSettings.Default.Line10DoubleWidth;
						PatternGridController.GridPainter.SelectionArgb1 = GridPainterSettings.Default.SelectionArgb1.ToArgb();
						PatternGridController.GridPainter.SelectionArgb2 = GridPainterSettings.Default.SelectionArgb2.ToArgb();
						PatternGridController.UpdateVisualImage();
					}
				}
			}
		}

		#endregion
	}
}
