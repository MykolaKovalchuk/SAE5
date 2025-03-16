using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using SAEWizard = Ravlyk.Drawing.WinForms.SAEWizard;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class WizardUserControl : UserControl, ICanClose
	{
		public WizardUserControl()
		{
			InitializeComponent();
		}

		internal WizardUserControl(CodedImage initialImage)
			: this()
		{
			this.initialImage = initialImage;
		}

		CodedImage initialImage;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			splitContainer.SplitterDistance = splitContainer.Width / 2 - 4;
			splitContainer.SplitterWidth = 8;
			splitContainer.Panel1MinSize = 300;
			splitContainer.Panel2MinSize = 300;

			ribbonLeft.CaptionBarVisible = false;
			ribbonLeft.Minimized = false;
			ribbonLeft.Height = 120;

			ribbonRight.CaptionBarVisible = false;
			ribbonRight.Minimized = false;
			ribbonRight.Height = 120;

			ribbonPanelTransform.Enabled = false;
			ribbonPanelCrop.Enabled = false;
			ribbonTabResultImage.Enabled = false;
			ribbonTabAdvanced.Enabled = false;
			ribbonTabSymbols.Enabled = false;
			ribbonTabPreview.Enabled = false;

			if (initialImage != null)
			{
				var sourceFileName = initialImage.SourceImageFileName;
				using (imageBoxSource.Controller?.SuspendUpdateVisualImage())
				{
					if (!string.IsNullOrEmpty(sourceFileName) && File.Exists(sourceFileName))
					{
						Wizard.LoadSourceImageFromFile(sourceFileName);
					}
					else
					{
						Wizard.ImageSetter.SetNewImage(initialImage);
					}

					RestoreImageSettings(initialImage);
				}
				EnableControls();
				initialImage = null;
			}
		}

		void RestoreImageSettings(CodedImage image)
		{
			Wizard.RestoreImageSettings(image);
			imageBoxSource.Controller?.UpdateParametersAndVisualImage();
		}

		#region Wizard

		public SAEWizard Wizard
		{
			get
			{
				if (wizard == null)
				{
					wizard = new SAEWizard();

					wizard.SetPalettes(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocationSafe), "DMC");
					wizard.SetSymbolFonts(SAEResources.GetAllFonts(Settings.Default.UserFontsLocationSafe));

					wizard.ImageCropper.PropertyChanged += WizardPropertyChanged;
					wizard.ImageResizer.PropertyChanged += WizardPropertyChanged;
					wizard.ImageColorer.PropertyChanged += WizardPropertyChanged;
					wizard.ImageSymboler.PropertyChanged += WizardPropertyChanged;

					checkBoxFixAspect.Checked = wizard.ImageResizer.KeepAspect;
					comboBoxUnits.TextBoxText = wizard.ImageResizer.Unit.ToString();
					upDownStitchesPerUnitHeight.TextBoxText = wizard.ImageResizer.StitchesPerUnitHeight.ToString("0.00");

					InitializeComboBoxKit();

					upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();

					foreach (var filterType in Enum.GetValues(typeof(ImageResampler.FilterType)))
					{
						comboBoxResizeFilter.DropDownItems.Add(new RibbonButton(filterType.ToString()));
					}
					comboBoxResizeFilter.TextBoxText = wizard.ImageResizer.FilterType.ToString();

					foreach (var colorComparisonType in Enum.GetValues(typeof(ImageColorsController.ColorComparisonTypes)))
					{
						comboBoxColorsSubstitute.DropDownItems.Add(new RibbonButton(colorComparisonType.ToString()));
					}
					comboBoxColorsSubstitute.TextBoxText = wizard.ImageColorer.ColorComparisonType.ToString();

					checkBoxEnsureBlackAndWhite.Checked = wizard.ImageColorer.EnsureBlackAndWhiteColors;
					upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();

					foreach (var fontName in wizard.ImageSymboler.AvailableFontsNames.OrderBy(fName => fName))
					{
						comboBoxSymbols.DropDownItems.Add(new RibbonButton(fontName));
					}
					comboBoxSymbols.TextBoxText = wizard.ImageSymboler.SymbolsFontName;
					checkBoxDigits.Checked = wizard.ImageSymboler.IncludeNumbers;
					checkBoxLetters.Checked = wizard.ImageSymboler.IncludeLetters;
					checkBoxSymbols.Checked = wizard.ImageSymboler.IncludeSymbols;

					imageBoxSource.Controller = new VisualZoomCropController(wizard.ImageCropper, new Size(16, 16));
					imageBoxResult.Controller = new VisualZoomController(wizard.ImageColorer.Manipulator, new Size(16, 16));
				}
				return wizard;
			}
		}

		SAEWizard wizard;

		void InitializeComboBoxKit()
		{
			comboBoxKit.DropDownItems.Clear();
			foreach (var paletteName in wizard.ImageColorer.AvailablePalettesNames.OrderBy(pName => pName))
			{
				comboBoxKit.DropDownItems.Add(new RibbonButton(paletteName));
			}
			comboBoxKit.DropDownItems.Add(new RibbonButton(Resources.ManagePalettes));
			comboBoxKit.TextBoxText = wizard.ImageColorer.PaletteName;
		}

		void WizardPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ImageCropController.CropKind):
					buttonRect.Checked = wizard.ImageCropper.CropKind == ImageCropper.CropKind.Rectangle;
					buttonArc.Checked = wizard.ImageCropper.CropKind == ImageCropper.CropKind.Arc;
					break;
				case nameof(ImageSizeController.SchemeWidth):
					upDownWidth.TextBoxText = wizard.ImageResizer.SchemeWidth.ToString(SizeStringFormat);
					break;
				case nameof(ImageSizeController.SchemeHeight):
					upDownHeight.TextBoxText = wizard.ImageResizer.SchemeHeight.ToString(SizeStringFormat);
					break;
				case nameof(ImageSizeController.KeepAspect):
					checkBoxFixAspect.Checked = wizard.ImageResizer.KeepAspect;
					break;
				case nameof(ImageSizeController.Unit):
					comboBoxUnits.TextBoxText = wizard.ImageResizer.Unit.ToString();
					upDownStitchesPerUnitHeight.TextBoxText = wizard.ImageResizer.StitchesPerUnitHeight.ToString("0.00");
					upDownStitchesPerUnitHeight.Visible = wizard.ImageResizer.Unit != ImageSizeController.SizeUnit.Stitch;
					upDownStitchesPerUnitWidth.TextBoxText = wizard.ImageResizer.StitchesPerUnitWidth.ToString("0.00");
					upDownStitchesPerUnitWidth.Visible = wizard.ImageResizer.Unit != ImageSizeController.SizeUnit.Stitch;
					break;
				case nameof(ImageSizeController.StitchesPerUnitHeight):
					upDownStitchesPerUnitHeight.TextBoxText = wizard.ImageResizer.StitchesPerUnitHeight.ToString("0.00");
					break;
				case nameof(ImageSizeController.StitchesPerUnitWidth):
					upDownStitchesPerUnitWidth.TextBoxText = wizard.ImageResizer.StitchesPerUnitWidth.ToString("0.00");
					break;
				case nameof(ImageSizeController.FilterType):
					comboBoxResizeFilter.TextBoxText = wizard.ImageResizer.FilterType.ToString();
					break;
				case nameof(ImageColorsController.PaletteName):
					comboBoxKit.TextBoxText = wizard.ImageColorer.PaletteName;
					break;
				case nameof(ImageColorsController.MaxColorsCount):
					upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();
					break;
				case nameof(ImageColorsController.EnsureBlackAndWhiteColors):
					checkBoxEnsureBlackAndWhite.Checked = wizard.ImageColorer.EnsureBlackAndWhiteColors;
					break;
				case nameof(ImageColorsController.ColorComparisonType):
					comboBoxColorsSubstitute.TextBoxText = wizard.ImageColorer.ColorComparisonType.ToString();
					break;
				case nameof(ImageColorsController.DitherLevel):
					upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();
					break;
				case nameof(ImageSymbolsController.SymbolsFontName):
					comboBoxSymbols.TextBoxText = wizard.ImageSymboler.SymbolsFontName;
					break;
				case nameof(ImageSymbolsController.IncludeSymbols):
					checkBoxSymbols.Checked = wizard.ImageSymboler.IncludeSymbols;
					break;
				case nameof(ImageSymbolsController.IncludeNumbers):
					checkBoxDigits.Checked = wizard.ImageSymboler.IncludeNumbers;
					break;
				case nameof(ImageSymbolsController.IncludeLetters):
					checkBoxLetters.Checked = wizard.ImageSymboler.IncludeLetters;
					break;
			}
		}

		string SizeStringFormat => wizard.ImageResizer.Unit == ImageSizeController.SizeUnit.Stitch ? "0" : "0.00";

		#endregion

		#region Source image controls events handlers

		void buttonOpen_Click(object sender, EventArgs e)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = Resources.FileFilterImages;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						var fileName = openFileDialog.FileName;
						using (imageBoxSource.Controller?.SuspendUpdateVisualImage())
						{
							if (Path.GetExtension(fileName)?.Equals(".sa4", StringComparison.OrdinalIgnoreCase) ?? false)
							{
								CodedImage image;
								using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
								{
									image = ImageSerializer.LoadFromStream(stream);
								}
								if (!string.IsNullOrEmpty(image?.SourceImageFileName) && File.Exists(image.SourceImageFileName))
								{
									var choice = MessageBox.Show(
										string.Format(Resources.WizardLoadSourceImageInsteadOfScheme, fileName, image.SourceImageFileName),
										Resources.WizardLoadImage, MessageBoxButtons.YesNoCancel);

									switch (choice)
									{
										case DialogResult.Cancel:
											return;
										case DialogResult.Yes:
											fileName = image.SourceImageFileName;
											RestoreImageSettings(image);
											break;
										case DialogResult.No:
											Wizard.ImageSetter.SetNewImage(image);
											RestoreImageSettings(image);
											EnableControls();
											return;
									}
								}
							}

							Wizard.LoadSourceImageFromFile(fileName);
						}
						EnableControls();
					}
					catch (Exception ex)
					{
						MessageBox.Show(Resources.ErrorCannotOpenFile + Environment.NewLine + ex.Message);
					}
				}
			}
		}

		void EnableControls()
		{
			ribbonPanelTransform.Enabled = true;
			ribbonPanelCrop.Enabled = true;

			ribbonTabResultImage.Enabled = true;
			ribbonTabAdvanced.Enabled = true;
			ribbonTabSymbols.Enabled = true;
			ribbonTabPreview.Enabled = true;

			buttonNext.Enabled = true;
		}

		void buttonRotateLeft_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.RotateCCW();
		}

		void buttonRotateRight_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.RotateCW();
		}

		void buttonFlipHorizontally_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.FlipHorizontally();
		}

		void buttonFlipVertically_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.FlipVertically();
		}

		void buttonCropRect_Click(object sender, EventArgs e)
		{
			buttonRect.Checked = !buttonRect.Checked;
			((VisualZoomCropController)imageBoxSource.Controller).CropKind = buttonRect.Checked ? ImageCropper.CropKind.Rectangle : ImageCropper.CropKind.None;
		}

		void buttonCropCircle_Click(object sender, EventArgs e)
		{
			buttonArc.Checked = !buttonArc.Checked;
			((VisualZoomCropController)imageBoxSource.Controller).CropKind = buttonArc.Checked ? ImageCropper.CropKind.Arc : ImageCropper.CropKind.None;
		}

		#endregion

		#region Preview controls events handlers

		void ribbonRight_ActiveTabChanged(object sender, EventArgs e)
		{
			if (wizard == null && ribbonRight.ActiveTab != ribbonTabResultImage)
			{
				ribbonRight.ActiveTab = ribbonTabResultImage;
				System.Media.SystemSounds.Beep.Play();
				return;
			}

			if (ribbonRight.ActiveTab == ribbonTabResultImage || ribbonRight.ActiveTab == ribbonTabAdvanced)
			{
				tabControlRight.SelectedTab = tabPageResultImage;
			}
			else if (ribbonRight.ActiveTab == ribbonTabSymbols)
			{
				tabControlRight.SelectedTab = tabPageSymbols;
			}
			else if (ribbonRight.ActiveTab == ribbonTabPreview)
			{
				tabControlRight.SelectedTab = tabPagePreview;
			}
		}

		void tabControlRight_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == tabPageSymbols && scrollControlSymbols.Controller == null)
			{
				scrollControlSymbols.Controller = new VisualSymbolsController(Wizard.ImageSymboler, new Size(scrollControlSymbols.Width, scrollControlSymbols.Height));
				scrollControlSymbols.Controller.VisualImageChanged += Symbols_VisualImageChanged;
			}
			else if (e.TabPage == tabPagePreview && scrollControlGridPreview.Controller == null)
			{
				var patternGridController = new VisualPatternGridController(Wizard.ImageSymboler.Manipulator, Wizard.ImageSymboler.SymbolsFont, SAEResources.GetCrossImage());
				scrollControlGridPreview.Controller = patternGridController;

				switch (patternGridController.PaintMode)
				{
					case PatternGridPainter.StitchesPaintMode.Symbols:
						buttonSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.WhiteSymbols:
						buttonWhiteSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.ColoredSymbols:
						buttonColorSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.HalfTones:
						buttonHalfTone.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Full:
						buttonFull.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Colors:
						buttonColors.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Cross:
						buttonCross.Checked = true;
						break;
				}
				upDownCellSize.TextBoxText = patternGridController.CellSize.ToString();
				checkBoxRulers.Checked = patternGridController.ShowRulers;
				checkBoxLines.Checked = patternGridController.ShowLines;
			}
		}

		VisualPatternGridController PatternGridController => scrollControlGridPreview.Controller as VisualPatternGridController;

		void buttonRandomSymbols_Click(object sender, EventArgs e)
		{
			using (Wizard.ImageSymboler.SuspendCallManipulations())
			{
				Wizard.ImageSymboler.ClearAllSelection();
				Wizard.ImageSymboler.AddRandomSymbols();
			}
		}

		void buttonClearSymbols_Click(object sender, EventArgs e)
		{
			Wizard.ImageSymboler.ClearAllSelection();
		}

		void Symbols_VisualImageChanged(object sender, EventArgs e)
		{
			labelSelectedNo.Text = string.Format(Resources.LabelsSelectedSymbolsCount,
				Wizard.ImageSymboler.SelectedCount, Wizard.ImageSymboler.Manipulator.ManipulatedImage.Palette.Count);
		}

		#endregion

		#region Finish

		void buttonNext_Click(object sender, EventArgs e)
		{
			tabControlRight.Focus();

			if (Wizard.ImageSymboler.SelectedCount < Wizard.ImageSymboler.Manipulator.ManipulatedImage.Palette.Count)
			{
				Wizard.ImageSymboler.AddRandomSymbols();
			}

			Wizard.FinalImage.SourceImageFileName = Wizard.ImageSetter.ImageSourceDescription;
			Wizard.FinalImage.Palette.Name = Wizard.ImageColorer.PaletteName;
			Wizard.SaveImageSettings(Wizard.FinalImage);

			Wizard.SaveDefaults();

			Finished?.Invoke(this, EventArgs.Empty);
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Cancelled?.Invoke(this, EventArgs.Empty);
		}

		void ICanClose.OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			buttonCancel_Click(this, EventArgs.Empty);
		}

		public event EventHandler Finished;
		public event EventHandler Cancelled;

		#endregion

		#region Result image controls

		void checkBoxFixAspect_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (wizard.ImageResizer.KeepAspect != checkBoxFixAspect.Checked)
			{
				wizard.ImageResizer.KeepAspect = checkBoxFixAspect.Checked;
			}
		}

		void upDownWidth_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownWidth.TextBoxText, out value))
			{
				wizard.ImageResizer.SchemeWidth = value;
			}
			upDownWidth.TextBoxText = wizard.ImageResizer.SchemeWidth.ToString(SizeStringFormat);
		}

		void upDownWidth_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeWidth++;
		}

		void upDownWidth_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeWidth--;
		}

		void upDownHeight_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownHeight.TextBoxText, out value))
			{
				wizard.ImageResizer.SchemeHeight = value;
			}
			upDownHeight.TextBoxText = wizard.ImageResizer.SchemeHeight.ToString(SizeStringFormat);
		}

		void upDownHeight_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeHeight++;
		}

		void upDownHeight_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeHeight--;
		}

		void comboBoxUnits_TextBoxTextChanged(object sender, EventArgs e)
		{
			ImageSizeController.SizeUnit value;
			if (Enum.TryParse(comboBoxUnits.TextBoxText, out value))
			{
				if (wizard.ImageResizer.Unit != value)
				{
					wizard.ImageResizer.Unit = value;
				}
			}
			var newValue = wizard.ImageResizer.Unit.ToString();
			if (comboBoxUnits.TextBoxText != newValue)
			{
				comboBoxUnits.TextBoxText = newValue;
			}
		}

		void upDownStitchesPerUnitHeight_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownStitchesPerUnitHeight.TextBoxText, out value))
			{
				wizard.ImageResizer.StitchesPerUnitHeight = value;
			}
			upDownStitchesPerUnitHeight.TextBoxText = wizard.ImageResizer.StitchesPerUnitHeight.ToString("0.00");
		}

		void upDownStitchesPerUnitHeight_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnitHeight += 0.25m;
		}

		void upDownStitchesPerUnitHeight_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnitHeight -= 0.25m;
		}
		void upDownStitchesPerUnitWidth_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownStitchesPerUnitWidth.TextBoxText, out value))
			{
				wizard.ImageResizer.StitchesPerUnitWidth = value;
			}
			upDownStitchesPerUnitWidth.TextBoxText = wizard.ImageResizer.StitchesPerUnitWidth.ToString("0.00");
		}

		void upDownStitchesPerUnitWidth_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnitWidth += 0.25m;
		}

		void upDownStitchesPerUnitWidth_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnitWidth -= 0.25m;
		}

		void comboBoxKit_TextBoxTextChanged(object sender, EventArgs e)
		{
			if (inComboBoxKitTextChange)
			{
				return;
			}

			if (comboBoxKit.TextBoxText == Resources.ManagePalettes)
			{
				inComboBoxKitTextChange = true;
				try
				{
					using (var threadsManageDialog = new ThreadsManagementDialog())
					{
						threadsManageDialog.ShowDialog(this);
					}
					wizard.ImageColorer.AddColorPalettes(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocation), true);
					InitializeComboBoxKit();
				}
				finally
				{
					inComboBoxKitTextChange = false;
				}
			}
			else
			{
				wizard.ImageColorer.PaletteName = comboBoxKit.TextBoxText;
			}
		}
		bool inComboBoxKitTextChange;

		void upDownMaxColors_TextBoxValidated(object sender, EventArgs e)
		{
			int value;
			if (int.TryParse(upDownMaxColors.TextBoxText, out value))
			{
				wizard.ImageColorer.MaxColorsCount = value;
			}
			upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();
		}

		void upDownMaxColors_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.MaxColorsCount++;
		}

		void upDownMaxColors_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.MaxColorsCount--;
		}

		void comboBoxResizeFilter_TextBoxTextChanged(object sender, EventArgs e)
		{
			ImageResampler.FilterType filterType;
			if (Enum.TryParse(comboBoxResizeFilter.TextBoxText, out filterType))
			{
				wizard.ImageResizer.FilterType = filterType;
			}
		}

		void comboBoxColorsSubstitute_TextBoxTextChanged(object sender, EventArgs e)
		{
			ImageColorsController.ColorComparisonTypes colorComparisonType;
			if (Enum.TryParse(comboBoxColorsSubstitute.TextBoxText, out colorComparisonType))
			{
				wizard.ImageColorer.ColorComparisonType = colorComparisonType;
			}
		}

		void checkBoxEnsureBlackAndWhite_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageColorer.EnsureBlackAndWhiteColors = checkBoxEnsureBlackAndWhite.Checked;
		}

		void upDownDither_TextBoxValidated(object sender, EventArgs e)
		{
			int value;
			if (int.TryParse(upDownDither.TextBoxText, out value))
			{
				wizard.ImageColorer.DitherLevel = value;
			}
			upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();
		}

		void upDownDither_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.DitherLevel++;
		}

		void upDownDither_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.DitherLevel--;
		}

		void comboBoxSymbols_TextBoxTextChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.SymbolsFontName = comboBoxSymbols.TextBoxText;
		}

		void checkBoxDigits_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeNumbers = checkBoxDigits.Checked;
		}

		void checkBoxLetters_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeLetters = checkBoxLetters.Checked;
		}

		void checkBoxSymbols_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeSymbols = checkBoxSymbols.Checked;
		}

		void buttonSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Symbols;
			buttonSymbols.Checked = true;
		}

		void buttonWhiteSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.WhiteSymbols;
			buttonWhiteSymbols.Checked = true;
		}

		void buttonColorSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.ColoredSymbols;
			buttonColorSymbols.Checked = true;
		}

		void buttonHalfTone_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.HalfTones;
			buttonHalfTone.Checked = true;
		}

		void buttonFull_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Full;
			buttonFull.Checked = true;
		}

		void buttonColors_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Colors;
			buttonColors.Checked = true;
		}

		void buttonCross_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Cross;
			buttonCross.Checked = true;
		}

		void upDownCellSize_UpButtonClicked(object sender, MouseEventArgs e)
		{
			var controller = PatternGridController;
			controller.CellSize++;
			upDownCellSize.TextBoxText = controller.CellSize.ToString();
		}

		void upDownCellSize_DownButtonClicked(object sender, MouseEventArgs e)
		{
			var controller = PatternGridController;
			controller.CellSize--;
			upDownCellSize.TextBoxText = controller.CellSize.ToString();
		}

		void checkBoxRulers_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			PatternGridController.ShowRulers = checkBoxRulers.Checked;
		}

		void checkBoxLines_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			PatternGridController.ShowLines = checkBoxLines.Checked;
		}

		#endregion
	}
}
