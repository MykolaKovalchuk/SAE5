namespace Ravlyk.Drawing.WinForms
{
	partial class ZoomImageBox
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
			this.panelControls = new System.Windows.Forms.Panel();
			this.zoomSlider = new Ravlyk.Drawing.WinForms.ZoomSliderEx();
			this.VisualControl = new Ravlyk.Drawing.WinForms.VisualControl();
			this.panelControls.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelControls
			// 
			this.panelControls.Controls.Add(this.zoomSlider);
			this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControls.Location = new System.Drawing.Point(0, 223);
			this.panelControls.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(250, 27);
			this.panelControls.TabIndex = 1;
			// 
			// zoomSlider
			// 
			this.zoomSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.zoomSlider.FixedZoomValues = new int[] {
        1,
        10,
        20,
        30,
        40,
        50,
        60,
        70,
        80,
        90,
        100,
        125,
        150,
        200,
        250,
        300,
        400,
        600,
        800,
        1200,
        1600};
			this.zoomSlider.Location = new System.Drawing.Point(66, 2);
			this.zoomSlider.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.zoomSlider.Name = "zoomSlider";
			this.zoomSlider.Size = new System.Drawing.Size(184, 24);
			this.zoomSlider.TabIndex = 0;
			this.zoomSlider.ZoomValue = 100;
			this.zoomSlider.ZoomChanged += new System.EventHandler(this.zoomSlider_ZoomChanged);
			// 
			// VisualControl
			// 
			this.VisualControl.BackColor = System.Drawing.Color.DimGray;
			this.VisualControl.Controller = null;
			this.VisualControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VisualControl.Location = new System.Drawing.Point(0, 0);
			this.VisualControl.MinimumSize = new System.Drawing.Size(16, 16);
			this.VisualControl.Name = "VisualControl";
			this.VisualControl.OverrideCursor = null;
			this.VisualControl.Size = new System.Drawing.Size(250, 223);
			this.VisualControl.TabIndex = 0;
			this.VisualControl.Text = "visualControl1";
			this.VisualControl.ControllerTouched += new System.EventHandler<Ravlyk.Drawing.WinForms.VisualControl.ControllerTouchedEventArgs>(this.VisualControl_ControllerTouched);
			// 
			// ZoomImageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.VisualControl);
			this.Controls.Add(this.panelControls);
			this.Name = "ZoomImageBox";
			this.Size = new System.Drawing.Size(250, 250);
			this.panelControls.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// Wrapped control.
		/// </summary>
		protected VisualControl VisualControl;
		private System.Windows.Forms.Panel panelControls;
		private ZoomSliderEx zoomSlider;
	}
}
