namespace PaletteFileEditor
{
	partial class Form1
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
			this.listViewPalette = new System.Windows.Forms.ListView();
			this.ColumnCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnG = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnB = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonLoad = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonSave = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewPalette
			// 
			this.listViewPalette.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnCode,
            this.ColumnName,
            this.ColumnColor,
            this.ColumnR,
            this.ColumnG,
            this.ColumnB});
			this.listViewPalette.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewPalette.Location = new System.Drawing.Point(0, 46);
			this.listViewPalette.Name = "listViewPalette";
			this.listViewPalette.Size = new System.Drawing.Size(638, 405);
			this.listViewPalette.TabIndex = 1;
			this.listViewPalette.UseCompatibleStateImageBehavior = false;
			this.listViewPalette.View = System.Windows.Forms.View.Details;
			this.listViewPalette.VirtualMode = true;
			this.listViewPalette.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewPalette_RetrieveVirtualItem);
			// 
			// ColumnCode
			// 
			this.ColumnCode.Text = "Code";
			this.ColumnCode.Width = 80;
			// 
			// ColumnName
			// 
			this.ColumnName.Text = "Name";
			this.ColumnName.Width = 180;
			// 
			// ColumnColor
			// 
			this.ColumnColor.Text = "Color";
			this.ColumnColor.Width = 80;
			// 
			// ColumnR
			// 
			this.ColumnR.Text = "R";
			// 
			// ColumnG
			// 
			this.ColumnG.Text = "G";
			// 
			// ColumnB
			// 
			this.ColumnB.Text = "B";
			// 
			// buttonLoad
			// 
			this.buttonLoad.Location = new System.Drawing.Point(12, 12);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(75, 23);
			this.buttonLoad.TabIndex = 0;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonSave);
			this.panel1.Controls.Add(this.buttonLoad);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(638, 46);
			this.panel1.TabIndex = 0;
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(93, 12);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 23);
			this.buttonSave.TabIndex = 1;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(638, 451);
			this.Controls.Add(this.listViewPalette);
			this.Controls.Add(this.panel1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewPalette;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.ColumnHeader ColumnCode;
		private System.Windows.Forms.ColumnHeader ColumnName;
		private System.Windows.Forms.ColumnHeader ColumnColor;
		private System.Windows.Forms.ColumnHeader ColumnR;
		private System.Windows.Forms.ColumnHeader ColumnG;
		private System.Windows.Forms.ColumnHeader ColumnB;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonSave;
	}
}

