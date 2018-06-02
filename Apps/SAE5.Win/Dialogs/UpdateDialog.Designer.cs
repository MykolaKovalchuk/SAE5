using System.Windows.Forms;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class UpdateDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDialog));
			this.panelTop = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panelContent = new System.Windows.Forms.Panel();
			this.textBoxDetails = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonDownload = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panelContent.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Controls.Add(this.label1);
			this.panelTop.Controls.Add(this.pictureBox1);
			this.panelTop.Name = "panelTop";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// panelContent
			// 
			resources.ApplyResources(this.panelContent, "panelContent");
			this.panelContent.Controls.Add(this.textBoxDetails);
			this.panelContent.Name = "panelContent";
			// 
			// textBoxDetails
			// 
			resources.ApplyResources(this.textBoxDetails, "textBoxDetails");
			this.textBoxDetails.BackColor = System.Drawing.Color.White;
			this.textBoxDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxDetails.Name = "textBoxDetails";
			this.textBoxDetails.ReadOnly = true;
			// 
			// panelBottom
			// 
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Controls.Add(this.panelButtons);
			this.panelBottom.Name = "panelBottom";
			// 
			// panelButtons
			// 
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Controls.Add(this.buttonDownload);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonDownload
			// 
			resources.ApplyResources(this.buttonDownload, "buttonDownload");
			this.buttonDownload.BackColor = System.Drawing.Color.White;
			this.buttonDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonDownload.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Downloading24;
			this.buttonDownload.IsSelected = false;
			this.buttonDownload.Name = "buttonDownload";
			this.buttonDownload.UseVisualStyleBackColor = true;
			this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackColor = System.Drawing.Color.White;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonCancel.IsSelected = false;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// UpdateDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.panelContent);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "UpdateDialog";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panelContent.ResumeLayout(false);
			this.panelContent.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Panel panelTop;
		private Label label1;
		private PictureBox pictureBox1;
		private Panel panelContent;
		private Panel panelBottom;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private Drawing.WinForms.FlatDialogButton buttonDownload;
		private TextBox textBoxDetails;
		private Panel panelButtons;
	}
}