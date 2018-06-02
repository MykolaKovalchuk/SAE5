namespace Ravlyk.Drawing.WinForms
{
	partial class ScrollControl
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
			this.VisualControl = new Ravlyk.Drawing.WinForms.VisualControl();
			this.panelH = new System.Windows.Forms.Panel();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.panelSpacer = new System.Windows.Forms.Panel();
			this.panelV = new System.Windows.Forms.Panel();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.panelH.SuspendLayout();
			this.panelV.SuspendLayout();
			this.SuspendLayout();
			// 
			// VisualControl
			// 
			this.VisualControl.Controller = null;
			this.VisualControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VisualControl.Location = new System.Drawing.Point(0, 0);
			this.VisualControl.MinimumSize = new System.Drawing.Size(16, 16);
			this.VisualControl.Name = "VisualControl";
			this.VisualControl.Size = new System.Drawing.Size(366, 366);
			this.VisualControl.TabIndex = 0;
			// 
			// panelH
			// 
			this.panelH.Controls.Add(this.hScrollBar);
			this.panelH.Controls.Add(this.panelSpacer);
			this.panelH.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelH.Location = new System.Drawing.Point(0, 366);
			this.panelH.Name = "panelH";
			this.panelH.Size = new System.Drawing.Size(400, 34);
			this.panelH.TabIndex = 3;
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Enabled = false;
			this.hScrollBar.Location = new System.Drawing.Point(0, 0);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(366, 34);
			this.hScrollBar.TabIndex = 3;
			this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
			// 
			// panelSpacer
			// 
			this.panelSpacer.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelSpacer.Location = new System.Drawing.Point(366, 0);
			this.panelSpacer.Name = "panelSpacer";
			this.panelSpacer.Size = new System.Drawing.Size(34, 34);
			this.panelSpacer.TabIndex = 0;
			// 
			// panelV
			// 
			this.panelV.Controls.Add(this.vScrollBar);
			this.panelV.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelV.Location = new System.Drawing.Point(366, 0);
			this.panelV.Name = "panelV";
			this.panelV.Size = new System.Drawing.Size(34, 366);
			this.panelV.TabIndex = 4;
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Enabled = false;
			this.vScrollBar.Location = new System.Drawing.Point(0, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(34, 366);
			this.vScrollBar.TabIndex = 2;
			this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
			// 
			// ScrollControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.VisualControl);
			this.Controls.Add(this.panelV);
			this.Controls.Add(this.panelH);
			this.Name = "ScrollControl";
			this.Size = new System.Drawing.Size(400, 400);
			this.panelH.ResumeLayout(false);
			this.panelV.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel panelH;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.Panel panelSpacer;
		private System.Windows.Forms.Panel panelV;
		private System.Windows.Forms.VScrollBar vScrollBar;

		/// <summary>
		/// Visual Control.
		/// </summary>
		public VisualControl VisualControl;
	}
}
