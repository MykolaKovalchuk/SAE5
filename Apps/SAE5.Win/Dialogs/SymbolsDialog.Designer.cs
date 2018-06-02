namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class SymbolsDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolsDialog));
			this.panelTop = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.scrollControlSymbols = new Ravlyk.Drawing.WinForms.ScrollControl();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			// panelBottom
			// 
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Controls.Add(this.panelButtons);
			this.panelBottom.Name = "panelBottom";
			// 
			// panelButtons
			// 
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Controls.Add(this.buttonOk);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.BackColor = System.Drawing.Color.White;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonOk.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Checkmark24;
			this.buttonOk.IsSelected = false;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackColor = System.Drawing.Color.White;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonCancel.IsSelected = false;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// scrollControlSymbols
			// 
			resources.ApplyResources(this.scrollControlSymbols, "scrollControlSymbols");
			this.scrollControlSymbols.Controller = null;
			this.scrollControlSymbols.Name = "scrollControlSymbols";
			// 
			// SymbolsDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.scrollControlSymbols);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "SymbolsDialog";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private Drawing.WinForms.ScrollControl scrollControlSymbols;
	}
}