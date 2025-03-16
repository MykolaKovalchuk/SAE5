namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class AboutDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.panelTop = new System.Windows.Forms.Panel();
			this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelRegistrationInfo = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTranslatedBy = new System.Windows.Forms.Label();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.BackColor = System.Drawing.Color.Transparent;
			this.panelTop.Controls.Add(this.pictureBoxTitle);
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Name = "panelTop";
			// 
			// pictureBoxTitle
			// 
			this.pictureBoxTitle.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Title;
			resources.ApplyResources(this.pictureBoxTitle, "pictureBoxTitle");
			this.pictureBoxTitle.Name = "pictureBoxTitle";
			this.pictureBoxTitle.TabStop = false;
			// 
			// panelBottom
			// 
			this.panelBottom.BackColor = System.Drawing.Color.Transparent;
			this.panelBottom.Controls.Add(this.buttonFeedback);
			this.panelBottom.Controls.Add(this.buttonWeb);
			this.panelBottom.Controls.Add(this.panelButtons);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// buttonFeedback
			// 
			this.buttonFeedback.BackColor = System.Drawing.Color.White;
			this.buttonFeedback.FlatAppearance.BorderSize = 0;
			this.buttonFeedback.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			resources.ApplyResources(this.buttonFeedback, "buttonFeedback");
			this.buttonFeedback.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Feedback24;
			this.buttonFeedback.IsSelected = false;
			this.buttonFeedback.Name = "buttonFeedback";
			this.buttonFeedback.UseVisualStyleBackColor = true;
			this.buttonFeedback.Click += new System.EventHandler(this.buttonFeedback_Click);
			// 
			// buttonWeb
			// 
			this.buttonWeb.BackColor = System.Drawing.Color.White;
			this.buttonWeb.FlatAppearance.BorderSize = 0;
			this.buttonWeb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			resources.ApplyResources(this.buttonWeb, "buttonWeb");
			this.buttonWeb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Web24;
			this.buttonWeb.IsSelected = false;
			this.buttonWeb.Name = "buttonWeb";
			this.buttonWeb.UseVisualStyleBackColor = true;
			this.buttonWeb.Click += new System.EventHandler(this.buttonWeb_Click);
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.buttonOk);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonOk
			// 
			this.buttonOk.BackColor = System.Drawing.Color.White;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Checkmark24;
			this.buttonOk.IsSelected = false;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// labelVersion
			// 
			resources.ApplyResources(this.labelVersion, "labelVersion");
			this.labelVersion.BackColor = System.Drawing.Color.Transparent;
			this.labelVersion.Name = "labelVersion";
			// 
			// labelRegistrationInfo
			// 
			resources.ApplyResources(this.labelRegistrationInfo, "labelRegistrationInfo");
			this.labelRegistrationInfo.BackColor = System.Drawing.Color.Transparent;
			this.labelRegistrationInfo.Name = "labelRegistrationInfo";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Name = "label1";
			// 
			// labelTranslatedBy
			// 
			resources.ApplyResources(this.labelTranslatedBy, "labelTranslatedBy");
			this.labelTranslatedBy.Name = "labelTranslatedBy";
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.ua_flag_bg;
			this.CancelButton = this.buttonOk;
			this.Controls.Add(this.labelTranslatedBy);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelRegistrationInfo);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "AboutDialog";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private System.Windows.Forms.PictureBox pictureBoxTitle;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelRegistrationInfo;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelTranslatedBy;
	}
}