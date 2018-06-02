namespace Ravlyk.Drawing.WinForms
{
	partial class CollapsiblePanel
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
			this.buttonCollapse = new Ravlyk.Drawing.WinForms.FlatButton();
			this.SuspendLayout();
			// 
			// buttonCollapse
			// 
			this.buttonCollapse.BackColor = System.Drawing.Color.White;
			this.buttonCollapse.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonCollapse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCollapse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCollapse.Image = global::Ravlyk.Drawing.WinForms.Properties.Resources.Down16;
			this.buttonCollapse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonCollapse.IsSelected = false;
			this.buttonCollapse.Location = new System.Drawing.Point(0, 0);
			this.buttonCollapse.Margin = new System.Windows.Forms.Padding(6);
			this.buttonCollapse.Name = "buttonCollapse";
			this.buttonCollapse.Size = new System.Drawing.Size(300, 44);
			this.buttonCollapse.TabIndex = 0;
			this.buttonCollapse.Text = "Collapse";
			this.buttonCollapse.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonCollapse.UseVisualStyleBackColor = true;
			this.buttonCollapse.Click += new System.EventHandler(this.buttonCollapse_Click);
			// 
			// CollapsiblePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.buttonCollapse);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "CollapsiblePanel";
			this.Size = new System.Drawing.Size(300, 288);
			this.ResumeLayout(false);

		}

		#endregion

		private FlatButton buttonCollapse;
	}
}
