using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing.Properties;
using Ravlyk.SAE5.WinForms.Properties;
using GdiColor = System.Drawing.Color;
using Rectangle = Ravlyk.Common.Rectangle;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class OpitonsDialog : Form
	{
		public OpitonsDialog()
		{
			InitializeComponent();

			checkBoxCheckForUpdates.Checked = Settings.Default.CheckForUpdatesAtStartup;

			switch (Settings.Default.Locale)
			{
				case "ru":
					comboBoxLanguage.SelectedIndex = 1;
					break;
				case "uk":
					comboBoxLanguage.SelectedIndex = 2;
					break;
				case "de":
					comboBoxLanguage.SelectedIndex = 3;
					break;
				default:
					comboBoxLanguage.SelectedIndex = 0;
					break;
			}

			textBoxUserPalettesLocation.Text = Settings.Default.UserPalettesLocationSafe;
			textBoxUserFontsLocation.Text = Settings.Default.UserFontsLocationSafe;

			SetButtonColor(buttonLineArgb, GridPainterSettings.Default.LineArgb.ToArgb());
			SetButtonColor(buttonLine5Argb, GridPainterSettings.Default.Line5Argb.ToArgb());
			SetButtonColor(buttonLine10Argb, GridPainterSettings.Default.Line10Argb.ToArgb());
			SetButtonColor(buttonNumbersArgb, GridPainterSettings.Default.NumbersArgb.ToArgb());
			SetButtonColor(buttonSelectionArgb1, GridPainterSettings.Default.SelectionArgb1.ToArgb());
			SetButtonColor(buttonSelectionArgb2, GridPainterSettings.Default.SelectionArgb2.ToArgb());
			checkBoxLine10Double.Checked = GridPainterSettings.Default.Line10DoubleWidth;

			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

			toolTip.SetToolTip(buttonReset, Resources.HintOptionsReset);
		}

		static void SetButtonColor(ButtonBase button, int argb)
		{
			const int ImageWidth = 20;

			var image = new IndexedImage { Size = new Size(ImageWidth, ImageWidth) };
			ImagePainter.FillRect(image, new Rectangle(0, 0, ImageWidth, ImageWidth), argb);

			var oldBitmap = button.Image;
			button.Image = image.ToBitmap();
			oldBitmap?.Dispose();

			button.Tag = argb;
		}

		void buttonGlidLinesColor_Click(object sender, EventArgs e)
		{
			var button = sender as ButtonBase;
			if (button != null)
			{
				using (var colorDialog = new ColorDialog())
				{
					var argb = button.Tag as int? ?? GdiColor.Black.ToArgb();
					var color = GdiColor.FromArgb(argb);
					colorDialog.Color = color;
					colorDialog.FullOpen = true;

					if (colorDialog.ShowDialog(this) == DialogResult.OK)
					{
						SetButtonColor(button, colorDialog.Color.ToArgb());
					}
				}
			}
		}

		void buttonOk_Click(object sender, EventArgs e)
		{
			Settings.Default.CheckForUpdatesAtStartup = checkBoxCheckForUpdates.Checked;
			Settings.Default.UserPalettesLocation = textBoxUserPalettesLocation.Text;
			Settings.Default.UserFontsLocation = textBoxUserFontsLocation.Text;

			switch (comboBoxLanguage.SelectedIndex)
			{
				case 1:
					AppInfo.SetSelectedLanguage("ru", true);
					break;
				case 2:
					AppInfo.SetSelectedLanguage("uk", true);
					break;
				case 3:
					AppInfo.SetSelectedLanguage("de", true);
					break;
				default:
					AppInfo.SetSelectedLanguage("en", true);
					break;
			}

			Settings.Default.Save();

			GridPainterSettings.Default.LineArgb = buttonLineArgb.Tag is int ? GdiColor.FromArgb((int)buttonLineArgb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.Line5Argb = buttonLine5Argb.Tag is int ? GdiColor.FromArgb((int)buttonLine5Argb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.Line10Argb = buttonLine10Argb.Tag is int ? GdiColor.FromArgb((int)buttonLine10Argb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.NumbersArgb = buttonNumbersArgb.Tag is int ? GdiColor.FromArgb((int)buttonNumbersArgb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.SelectionArgb1 = buttonSelectionArgb1.Tag is int ? GdiColor.FromArgb((int)buttonSelectionArgb1.Tag) : GdiColor.Black;
			GridPainterSettings.Default.SelectionArgb2 = buttonSelectionArgb2.Tag is int ? GdiColor.FromArgb((int)buttonSelectionArgb2.Tag) : GdiColor.White;
			GridPainterSettings.Default.Line10DoubleWidth = checkBoxLine10Double.Checked;

			GridPainterSettings.Default.Save();
		}

		void buttonReset_Click(object sender, EventArgs e)
		{
			checkBoxCheckForUpdates.Checked = true;
			comboBoxLanguage.SelectedIndex = 0;

			SetButtonColorWithDefaultValue(buttonLineArgb, GridPainterSettings.Default, nameof(GridPainterSettings.LineArgb));
			SetButtonColorWithDefaultValue(buttonLine5Argb, GridPainterSettings.Default, nameof(GridPainterSettings.Line5Argb));
			SetButtonColorWithDefaultValue(buttonLine10Argb, GridPainterSettings.Default, nameof(GridPainterSettings.Line10Argb));
			SetButtonColorWithDefaultValue(buttonNumbersArgb, GridPainterSettings.Default, nameof(GridPainterSettings.NumbersArgb));
			SetButtonColorWithDefaultValue(buttonSelectionArgb1, GridPainterSettings.Default, nameof(GridPainterSettings.SelectionArgb1));
			SetButtonColorWithDefaultValue(buttonSelectionArgb2, GridPainterSettings.Default, nameof(GridPainterSettings.SelectionArgb2));
			checkBoxLine10Double.Checked = true;

			System.Media.SystemSounds.Beep.Play();
		}

		void SetButtonColorWithDefaultValue(ButtonBase button, ApplicationSettingsBase settings, string propertyName)
		{
			var property = settings.Properties[propertyName];
			if (property?.DefaultValue != null)
			{
				try
				{
					var color = (GdiColor?)new ColorConverter().ConvertFrom(property.DefaultValue);
					if (color.HasValue)
					{
						SetButtonColor(button, color.Value.ToArgb());
					}
				}
				catch (InvalidCastException) { }
				catch (NotSupportedException) { }
			}
		}

		void buttonSelectUserPalettesLocation_Click(object sender, EventArgs e)
		{
			using (var folderDialog = new FolderBrowserDialog { ShowNewFolderButton = true })
			{
				folderDialog.SelectedPath = textBoxUserPalettesLocation.Text;
				if (folderDialog.ShowDialog(this) == DialogResult.OK)
				{
					textBoxUserPalettesLocation.Text = folderDialog.SelectedPath;
				}
			}
		}

		private void buttonSelectUserFontsLocation_Click(object sender, EventArgs e)
		{
			using (var folderDialog = new FolderBrowserDialog { ShowNewFolderButton = true })
			{
				folderDialog.SelectedPath = textBoxUserFontsLocation.Text;
				if (folderDialog.ShowDialog(this) == DialogResult.OK)
				{
					textBoxUserFontsLocation.Text = folderDialog.SelectedPath;
				}
			}
		}
	}
}
