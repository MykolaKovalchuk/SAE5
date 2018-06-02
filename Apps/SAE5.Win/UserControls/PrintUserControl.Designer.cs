namespace Ravlyk.SAE5.WinForms.UserControls
{
	partial class PrintUserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.visualControlPrintPreview = new Ravlyk.Drawing.WinForms.VisualControl();
			this.SuspendLayout();
			// 
			// visualControlPrintPreview
			// 
			this.visualControlPrintPreview.Controller = null;
			this.visualControlPrintPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.visualControlPrintPreview.Location = new System.Drawing.Point(0, 0);
			this.visualControlPrintPreview.MinimumSize = new System.Drawing.Size(16, 16);
			this.visualControlPrintPreview.Name = "visualControlPrintPreview";
			this.visualControlPrintPreview.Size = new System.Drawing.Size(1648, 1012);
			this.visualControlPrintPreview.TabIndex = 0;
			this.visualControlPrintPreview.Text = "visualControl1";
			// 
			// PrintUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.visualControlPrintPreview);
			this.Name = "PrintUserControl";
			this.Size = new System.Drawing.Size(1648, 1012);
			this.ResumeLayout(false);

		}

		#endregion
		private Drawing.WinForms.VisualControl visualControlPrintPreview;
	}
}
