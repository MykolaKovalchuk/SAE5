namespace Ravlyk.Drawing.WinForms
{
	partial class ZoomSlider
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
			this.buttonZoomOut = new Ravlyk.Drawing.WinForms.ZoomSlider.ZoomButton();
			this.trackBarZoom = new Ravlyk.Drawing.WinForms.ZoomSlider.ZoomTrackBar();
			this.buttonZoomIn = new Ravlyk.Drawing.WinForms.ZoomSlider.ZoomButton();
			((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonZoomOut.Location = new System.Drawing.Point(0, 0);
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(46, 46);
			this.buttonZoomOut.TabIndex = 1;
			this.buttonZoomOut.TabStop = false;
			this.buttonZoomOut.Text = "-";
			this.buttonZoomOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.buttonZoomOut.UseVisualStyleBackColor = true;
			this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
			// 
			// trackBarZoom
			// 
			this.trackBarZoom.AutoSize = false;
			this.trackBarZoom.LargeChange = 3;
			this.trackBarZoom.Location = new System.Drawing.Point(46, 0);
			this.trackBarZoom.Maximum = 2;
			this.trackBarZoom.Name = "trackBarZoom";
			this.trackBarZoom.Size = new System.Drawing.Size(180, 46);
			this.trackBarZoom.TabIndex = 2;
			this.trackBarZoom.TabStop = false;
			this.trackBarZoom.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarZoom.Value = 1;
			this.trackBarZoom.ValueChanged += new System.EventHandler(this.trackBarZoom_ValueChanged);
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonZoomIn.Location = new System.Drawing.Point(226, 0);
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(46, 46);
			this.buttonZoomIn.TabIndex = 3;
			this.buttonZoomIn.TabStop = false;
			this.buttonZoomIn.Text = "+";
			this.buttonZoomIn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.buttonZoomIn.UseVisualStyleBackColor = true;
			this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
			// 
			// ZoomSlider
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.trackBarZoom);
			this.Controls.Add(this.buttonZoomIn);
			this.Controls.Add(this.buttonZoomOut);
			this.Name = "ZoomSlider";
			this.Size = new System.Drawing.Size(274, 46);
			((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ZoomButton buttonZoomOut;
		private ZoomTrackBar trackBarZoom;
		private ZoomButton buttonZoomIn;
	}
}
