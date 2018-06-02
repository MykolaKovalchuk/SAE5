namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class InputDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputDialog));
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.labelPrompt = new System.Windows.Forms.Label();
			this.textBoxAnswer = new System.Windows.Forms.TextBox();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.panelButtons);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.buttonOk);
			this.panelButtons.Controls.Add(this.buttonCancel);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonOk
			// 
			this.buttonOk.BackColor = System.Drawing.Color.White;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonOk.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Checkmark24;
			this.buttonOk.IsSelected = false;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.BackColor = System.Drawing.Color.White;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.IsSelected = false;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelPrompt
			// 
			resources.ApplyResources(this.labelPrompt, "labelPrompt");
			this.labelPrompt.Name = "labelPrompt";
			// 
			// textBoxAnswer
			// 
			resources.ApplyResources(this.textBoxAnswer, "textBoxAnswer");
			this.textBoxAnswer.Name = "textBoxAnswer";
			this.textBoxAnswer.TextChanged += new System.EventHandler(this.textBoxAnswer_TextChanged);
			// 
			// InputDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.textBoxAnswer);
			this.Controls.Add(this.labelPrompt);
			this.Controls.Add(this.panelBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputDialog";
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private System.Windows.Forms.Label labelPrompt;
		private System.Windows.Forms.TextBox textBoxAnswer;
	}
}