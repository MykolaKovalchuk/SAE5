namespace Ravlyk.SAE5.WinForms.UserControls
{
	partial class HomeUserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeUserControl));
			this.panelTop = new System.Windows.Forms.Panel();
			this.labelVersion = new System.Windows.Forms.Label();
			this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelLanguages = new System.Windows.Forms.Panel();
			this.buttonUkrainian = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonRussian = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonEnglish = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonRegister = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonLinkedIn = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonTwitter = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonFacebook = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.panelMain = new System.Windows.Forms.Panel();
			this.panelLastOpenFiles = new System.Windows.Forms.FlowLayoutPanel();
			this.panelLastOpenFilesTop = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOpen = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonNew = new Ravlyk.Drawing.WinForms.FlatButton();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.panelLanguages.SuspendLayout();
			this.panelMain.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Controls.Add(this.labelVersion);
			this.panelTop.Controls.Add(this.pictureBoxTitle);
			this.panelTop.Name = "panelTop";
			// 
			// labelVersion
			// 
			resources.ApplyResources(this.labelVersion, "labelVersion");
			this.labelVersion.BackColor = System.Drawing.Color.Transparent;
			this.labelVersion.Name = "labelVersion";
			// 
			// pictureBoxTitle
			// 
			resources.ApplyResources(this.pictureBoxTitle, "pictureBoxTitle");
			this.pictureBoxTitle.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Title;
			this.pictureBoxTitle.Name = "pictureBoxTitle";
			this.pictureBoxTitle.TabStop = false;
			// 
			// panelBottom
			// 
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Controls.Add(this.panelLanguages);
			this.panelBottom.Controls.Add(this.buttonRegister);
			this.panelBottom.Controls.Add(this.buttonLinkedIn);
			this.panelBottom.Controls.Add(this.buttonTwitter);
			this.panelBottom.Controls.Add(this.buttonFacebook);
			this.panelBottom.Controls.Add(this.buttonFeedback);
			this.panelBottom.Controls.Add(this.buttonWeb);
			this.panelBottom.Name = "panelBottom";
			// 
			// panelLanguages
			// 
			resources.ApplyResources(this.panelLanguages, "panelLanguages");
			this.panelLanguages.Controls.Add(this.buttonUkrainian);
			this.panelLanguages.Controls.Add(this.buttonRussian);
			this.panelLanguages.Controls.Add(this.buttonEnglish);
			this.panelLanguages.Name = "panelLanguages";
			// 
			// buttonUkrainian
			// 
			resources.ApplyResources(this.buttonUkrainian, "buttonUkrainian");
			this.buttonUkrainian.BackColor = System.Drawing.Color.White;
			this.buttonUkrainian.FlatAppearance.BorderSize = 0;
			this.buttonUkrainian.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonUkrainian.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Ua24;
			this.buttonUkrainian.IsSelected = false;
			this.buttonUkrainian.Name = "buttonUkrainian";
			this.buttonUkrainian.UseVisualStyleBackColor = true;
			this.buttonUkrainian.Click += new System.EventHandler(this.buttonUkrainian_Click);
			// 
			// buttonRussian
			// 
			resources.ApplyResources(this.buttonRussian, "buttonRussian");
			this.buttonRussian.BackColor = System.Drawing.Color.White;
			this.buttonRussian.FlatAppearance.BorderSize = 0;
			this.buttonRussian.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonRussian.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Ru24;
			this.buttonRussian.IsSelected = false;
			this.buttonRussian.Name = "buttonRussian";
			this.buttonRussian.UseVisualStyleBackColor = true;
			this.buttonRussian.Click += new System.EventHandler(this.buttonRussian_Click);
			// 
			// buttonEnglish
			// 
			resources.ApplyResources(this.buttonEnglish, "buttonEnglish");
			this.buttonEnglish.BackColor = System.Drawing.Color.White;
			this.buttonEnglish.FlatAppearance.BorderSize = 0;
			this.buttonEnglish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonEnglish.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.USA24;
			this.buttonEnglish.IsSelected = false;
			this.buttonEnglish.Name = "buttonEnglish";
			this.buttonEnglish.UseVisualStyleBackColor = true;
			this.buttonEnglish.Click += new System.EventHandler(this.buttonEnglish_Click);
			// 
			// buttonRegister
			// 
			resources.ApplyResources(this.buttonRegister, "buttonRegister");
			this.buttonRegister.BackColor = System.Drawing.Color.White;
			this.buttonRegister.FlatAppearance.BorderSize = 0;
			this.buttonRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonRegister.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Key24;
			this.buttonRegister.IsSelected = false;
			this.buttonRegister.Name = "buttonRegister";
			this.buttonRegister.UseVisualStyleBackColor = true;
			this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
			// 
			// buttonLinkedIn
			// 
			resources.ApplyResources(this.buttonLinkedIn, "buttonLinkedIn");
			this.buttonLinkedIn.BackColor = System.Drawing.Color.White;
			this.buttonLinkedIn.FlatAppearance.BorderSize = 0;
			this.buttonLinkedIn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonLinkedIn.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.LinkedIn24;
			this.buttonLinkedIn.IsSelected = false;
			this.buttonLinkedIn.Name = "buttonLinkedIn";
			this.buttonLinkedIn.UseVisualStyleBackColor = true;
			this.buttonLinkedIn.Click += new System.EventHandler(this.buttonLinkedIn_Click);
			// 
			// buttonTwitter
			// 
			resources.ApplyResources(this.buttonTwitter, "buttonTwitter");
			this.buttonTwitter.BackColor = System.Drawing.Color.White;
			this.buttonTwitter.FlatAppearance.BorderSize = 0;
			this.buttonTwitter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonTwitter.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Twitter24;
			this.buttonTwitter.IsSelected = false;
			this.buttonTwitter.Name = "buttonTwitter";
			this.buttonTwitter.UseVisualStyleBackColor = true;
			this.buttonTwitter.Click += new System.EventHandler(this.buttonTwitter_Click);
			// 
			// buttonFacebook
			// 
			resources.ApplyResources(this.buttonFacebook, "buttonFacebook");
			this.buttonFacebook.BackColor = System.Drawing.Color.White;
			this.buttonFacebook.FlatAppearance.BorderSize = 0;
			this.buttonFacebook.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonFacebook.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Facebook24;
			this.buttonFacebook.IsSelected = false;
			this.buttonFacebook.Name = "buttonFacebook";
			this.buttonFacebook.UseVisualStyleBackColor = true;
			this.buttonFacebook.Click += new System.EventHandler(this.buttonFacebook_Click);
			// 
			// buttonFeedback
			// 
			resources.ApplyResources(this.buttonFeedback, "buttonFeedback");
			this.buttonFeedback.BackColor = System.Drawing.Color.White;
			this.buttonFeedback.FlatAppearance.BorderSize = 0;
			this.buttonFeedback.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonFeedback.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Feedback24;
			this.buttonFeedback.IsSelected = false;
			this.buttonFeedback.Name = "buttonFeedback";
			this.buttonFeedback.UseVisualStyleBackColor = true;
			this.buttonFeedback.Click += new System.EventHandler(this.buttonFeedback_Click);
			// 
			// buttonWeb
			// 
			resources.ApplyResources(this.buttonWeb, "buttonWeb");
			this.buttonWeb.BackColor = System.Drawing.Color.White;
			this.buttonWeb.FlatAppearance.BorderSize = 0;
			this.buttonWeb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonWeb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Web24;
			this.buttonWeb.IsSelected = false;
			this.buttonWeb.Name = "buttonWeb";
			this.buttonWeb.UseVisualStyleBackColor = true;
			this.buttonWeb.Click += new System.EventHandler(this.buttonWeb_Click);
			// 
			// panelMain
			// 
			resources.ApplyResources(this.panelMain, "panelMain");
			this.panelMain.Controls.Add(this.panelLastOpenFiles);
			this.panelMain.Controls.Add(this.panelLastOpenFilesTop);
			this.panelMain.Controls.Add(this.panelButtons);
			this.panelMain.Name = "panelMain";
			// 
			// panelLastOpenFiles
			// 
			resources.ApplyResources(this.panelLastOpenFiles, "panelLastOpenFiles");
			this.panelLastOpenFiles.Name = "panelLastOpenFiles";
			// 
			// panelLastOpenFilesTop
			// 
			resources.ApplyResources(this.panelLastOpenFilesTop, "panelLastOpenFilesTop");
			this.panelLastOpenFilesTop.Name = "panelLastOpenFilesTop";
			// 
			// panelButtons
			// 
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Controls.Add(this.buttonOpen);
			this.panelButtons.Controls.Add(this.buttonNew);
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonOpen
			// 
			resources.ApplyResources(this.buttonOpen, "buttonOpen");
			this.buttonOpen.BackColor = System.Drawing.Color.White;
			this.buttonOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonOpen.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open100;
			this.buttonOpen.IsSelected = false;
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// buttonNew
			// 
			resources.ApplyResources(this.buttonNew, "buttonNew");
			this.buttonNew.BackColor = System.Drawing.Color.White;
			this.buttonNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonNew.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.EditImage100;
			this.buttonNew.IsSelected = false;
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// HomeUserControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "HomeUserControl";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelLanguages.ResumeLayout(false);
			this.panelMain.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.PictureBox pictureBoxTitle;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatButton buttonOpen;
		private Drawing.WinForms.FlatButton buttonNew;
		private System.Windows.Forms.FlowLayoutPanel panelLastOpenFiles;
		private System.Windows.Forms.Panel panelLastOpenFilesTop;
		internal Drawing.WinForms.FlatButton buttonRegister;
		private Drawing.WinForms.FlatButton buttonLinkedIn;
		private Drawing.WinForms.FlatButton buttonTwitter;
		private Drawing.WinForms.FlatButton buttonFacebook;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
		private System.Windows.Forms.Panel panelLanguages;
		private Drawing.WinForms.FlatButton buttonUkrainian;
		private Drawing.WinForms.FlatButton buttonRussian;
		private Drawing.WinForms.FlatButton buttonEnglish;
	}
}
