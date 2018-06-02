namespace Ravlyk.Drawing.WinForms
{
	partial class ZoomSliderEx
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
			this.ZoomSlider = new Ravlyk.Drawing.WinForms.ZoomSlider();
			this.labelZoom = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ZoomSlider
			// 
			this.ZoomSlider.FixedZoomValues = new int[] {
        50,
        100,
        200};
			this.ZoomSlider.Location = new System.Drawing.Point(47, 0);
			this.ZoomSlider.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.ZoomSlider.Name = "ZoomSlider";
			this.ZoomSlider.Size = new System.Drawing.Size(137, 23);
			this.ZoomSlider.TabIndex = 0;
			this.ZoomSlider.ZoomChanged += new System.EventHandler(this.ZoomSlider_ZoomChanged);
			// 
			// labelZoom
			// 
			this.labelZoom.Location = new System.Drawing.Point(0, 0);
			this.labelZoom.Name = "labelZoom";
			this.labelZoom.Size = new System.Drawing.Size(44, 23);
			this.labelZoom.TabIndex = 1;
			this.labelZoom.Text = "100%";
			this.labelZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ZoomSliderEx
			// 
			this.Controls.Add(this.labelZoom);
			this.Controls.Add(this.ZoomSlider);
			this.Name = "ZoomSliderEx";
			this.Size = new System.Drawing.Size(184, 23);
			this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// Internal wrapped <see cref="Ravlyk.Drawing.WinForms.ZoomSlider"/> control.
		/// </summary>
		public ZoomSlider ZoomSlider;
		private System.Windows.Forms.Label labelZoom;
	}
}
