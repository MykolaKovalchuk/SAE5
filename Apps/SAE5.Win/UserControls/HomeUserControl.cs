using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE5.WinForms.Properties;
using SAE5.Win;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class HomeUserControl : UserControl
	{
		public HomeUserControl(MainForm mainForm)
		{
			InitializeComponent();

			this.mainForm = mainForm;
			labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion);
		}

		readonly MainForm mainForm;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

			toolTip.SetToolTip(buttonWeb, Resources.HintHomeButtonWeb);
			toolTip.SetToolTip(buttonFeedback, Resources.HintHomeButtonFeedback);
			toolTip.SetToolTip(buttonFacebook, Resources.HintHomeButtonFacebook);
			toolTip.SetToolTip(buttonTwitter, Resources.HintHomeButtonTwitter);
			toolTip.SetToolTip(buttonLinkedIn, Resources.HintHomeButtonLinkedIn);
			toolTip.SetToolTip(buttonRegister, Resources.HintHomeButtonRegister);

			toolTip.SetToolTip(buttonEnglish, "English language");
			toolTip.SetToolTip(buttonRussian, "Русский язык");
			toolTip.SetToolTip(buttonUkrainian, "Українська мова");

			InitializeRecentFilesButtons(toolTip);

			buttonRegister.Visible = !RegistrationHelper.IsRegistered;

			SetLanguageButtons();
		}

		#region Recent files

		void InitializeRecentFilesButtons(ToolTip toolTip)
		{
			if (string.IsNullOrEmpty(Settings.Default.LastOpenFiles))
			{
				var startupPath = Path.Combine(AppInfo.StartupPath, "Samples");
				if (!string.IsNullOrEmpty(startupPath) && Directory.Exists(startupPath))
				{
					var sampleFileNames = Directory.GetFiles(startupPath, "*.sa4").Take(10);
					Settings.Default.LastOpenFiles = string.Join(Settings.FilesSeparator.ToString(), sampleFileNames);
					Settings.Default.Save();
				}
			}

			foreach (var fileName in Settings.Default.LastOpenFiles.Split(Settings.FilesSeparator))
			{
				if (File.Exists(fileName))
				{
					IndexedImage image;
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
						{
							image = ImageSerializer.LoadFromStream(stream);
						}

						if (image != null && image.Size.Width > 0 && image.Size.Height > 0)
						{
							Bitmap bitmap;
							if (image.Size.Width > 200 || image.Size.Height > 200)
							{
								var maxLength = Math.Max(image.Size.Width, image.Size.Height);
								var newSize = new Size(image.Size.Width * 200 / maxLength, image.Size.Height * 200 / maxLength);
								bitmap = new ImageResampler().Resample(image, newSize, ImageResampler.FilterType.Box).ToBitmap();
							}
							else
							{
								bitmap = image.ToBitmap();
							}

							var imageButton = new FlatButton();
							imageButton.Size = new System.Drawing.Size(250, 250);
							imageButton.Image = bitmap;
							imageButton.Text = Environment.NewLine + Path.GetFileNameWithoutExtension(fileName);
							imageButton.Tag = fileName;
							imageButton.TextAlign = ContentAlignment.BottomCenter;
							imageButton.ImageAlign = ContentAlignment.MiddleCenter;
							imageButton.FlatAppearance.BorderSize = 0;
							imageButton.Click += ImageButton_Click;

							var tooltip = fileName + Environment.NewLine +
								string.Format(Resources.ImageInfoTooltip, image.Size.Width, image.Size.Height, image.Palette.Count);
							toolTip.SetToolTip(imageButton, tooltip);

							panelLastOpenFiles.Controls.Add(imageButton);
						}
					}
					catch
					{
						continue;
					}
				}
			}
		}

		#endregion

		#region Buttons clicked event handlers

		void buttonNew_Click(object sender, EventArgs e)
		{
			NewButtonClicked?.Invoke(this, e);
		}

		public event EventHandler NewButtonClicked;

		void buttonOpen_Click(object sender, EventArgs e)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = Resources.FileFilterSAE;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					OnOpenButtonClicked(openFileDialog.FileName);
				}
			}
		}

		void ImageButton_Click(object sender, EventArgs e)
		{
			OnOpenButtonClicked(((FlatButton)sender).Tag.ToString());
		}

		void OnOpenButtonClicked(string fileName)
		{
			OpenButtonClicked?.Invoke(this, new OpenFileEventArgs(fileName));
		}

		public event EventHandler<OpenFileEventArgs> OpenButtonClicked;

		public class OpenFileEventArgs : EventArgs
		{
			public OpenFileEventArgs(string fileName)
			{
				FileName = fileName;
			}

			public string FileName { get; }
		}

		#endregion

		#region Link buttons

		void buttonWeb_Click(object sender, EventArgs e)
		{
			AppInfo.GoToWebsite();
		}

		void buttonFeedback_Click(object sender, EventArgs e)
		{
			AppInfo.EmailToSupport();
		}

		void buttonFacebook_Click(object sender, EventArgs e)
		{
			Process.Start("http://facebook.com/stitcharteasy5");
		}

		void buttonTwitter_Click(object sender, EventArgs e)
		{
			Process.Start("http://twitter.com/StitchArtEasy");
		}

		void buttonLinkedIn_Click(object sender, EventArgs e)
		{
			Process.Start("http://linkedin.com/in/mykolakovalchuk");
		}

		#endregion

		#region Updates

		internal void SetUpdatesLink()
		{
			if (string.IsNullOrEmpty(mainForm.UpdateDetails))
			{
				return;
			}

			var linkLabel = new LinkLabel
			{
				Text = Resources.LinkLabelNewVersionAvailable,
				AutoSize = true,
				Left = buttonRegister.Visible ? buttonRegister.Left + buttonRegister.Width + 16 : buttonRegister.Left,
				Top = buttonRegister.Top + 13
			};
			linkLabel.LinkClicked += (sender, e) => MainForm.ShowUpdateDialog(mainForm.UpdateDetails, this);
			panelBottom.Controls.Add(linkLabel);
		}

		#endregion

		#region Registration

		void buttonRegister_Click(object sender, EventArgs e)
		{
			RegistrationHelper.ShowRegisterDialog(this);
			if (RegistrationHelper.IsRegistered)
			{
				buttonRegister.Visible = false;
			}
		}

		#endregion

		#region Language

		void SetLanguageButtons()
		{
			switch (Settings.Default.Locale)
			{
				case "ru":
					buttonRussian.IsSelected = true;
					break;
				case "uk":
					buttonUkrainian.IsSelected = true;
					break;
				case "de":
					buttonGerman.IsSelected = true;
					break;
				default:
					buttonEnglish.IsSelected = true;
					break;
			}
		}

		void buttonEnglish_Click(object sender, EventArgs e)
		{
			SetLanguage("en");
		}

		void buttonRussian_Click(object sender, EventArgs e)
		{
			SetLanguage("ru");
		}

		void buttonUkrainian_Click(object sender, EventArgs e)
		{
			SetLanguage("uk");
		}

		private void buttonGerman_Click(object sender, EventArgs e)
		{
			SetLanguage("de");
		}

		void SetLanguage(string cultureName)
		{
			AppInfo.SetSelectedLanguage(cultureName, false);
			Dispose();
		}

		#endregion
	}
}
