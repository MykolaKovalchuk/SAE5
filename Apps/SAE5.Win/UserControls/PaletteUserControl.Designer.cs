namespace Ravlyk.SAE5.WinForms.UserControls
{
	partial class PaletteUserControl
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

				foreach (var brush in brushes.Values)
				{
					brush.Dispose();
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteUserControl));
			this.panelTop = new System.Windows.Forms.Panel();
			this.labelCaption = new System.Windows.Forms.Label();
			this.listViewPalette = new Ravlyk.Drawing.WinForms.PaletteListView();
			this.columnSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnSymbol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panelTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.labelCaption);
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Name = "panelTop";
			// 
			// labelCaption
			// 
			resources.ApplyResources(this.labelCaption, "labelCaption");
			this.labelCaption.Name = "labelCaption";
			// 
			// listViewPalette
			// 
			this.listViewPalette.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewPalette.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSelected,
            this.columnSymbol,
            this.columnCode,
            this.columnColor,
            this.columnCount});
			resources.ApplyResources(this.listViewPalette, "listViewPalette");
			this.listViewPalette.FullRowSelect = true;
			this.listViewPalette.HideSelection = false;
			this.listViewPalette.Name = "listViewPalette";
			this.listViewPalette.OwnerDraw = true;
			this.listViewPalette.ShowGroups = false;
			this.listViewPalette.ShowItemToolTips = true;
			this.listViewPalette.UseCompatibleStateImageBehavior = false;
			this.listViewPalette.View = System.Windows.Forms.View.Details;
			this.listViewPalette.VirtualMode = true;
			this.listViewPalette.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewPalette_ColumnClick);
			this.listViewPalette.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewPalette_DrawColumnHeader);
			this.listViewPalette.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewPalette_DrawSubItem);
			this.listViewPalette.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewPalette_RetrieveVirtualItem);
			this.listViewPalette.SelectedIndexChanged += new System.EventHandler(this.listViewPalette_SelectedIndexChanged);
			this.listViewPalette.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewPalette_MouseClick);
			// 
			// columnSelected
			// 
			resources.ApplyResources(this.columnSelected, "columnSelected");
			// 
			// columnSymbol
			// 
			resources.ApplyResources(this.columnSymbol, "columnSymbol");
			// 
			// columnCode
			// 
			resources.ApplyResources(this.columnCode, "columnCode");
			// 
			// columnColor
			// 
			resources.ApplyResources(this.columnColor, "columnColor");
			// 
			// columnCount
			// 
			resources.ApplyResources(this.columnCount, "columnCount");
			// 
			// PaletteUserControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listViewPalette);
			this.Controls.Add(this.panelTop);
			this.Name = "PaletteUserControl";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private Drawing.WinForms.PaletteListView listViewPalette;
		private System.Windows.Forms.ColumnHeader columnSymbol;
		private System.Windows.Forms.ColumnHeader columnCode;
		private System.Windows.Forms.ColumnHeader columnColor;
		private System.Windows.Forms.ColumnHeader columnCount;
		private System.Windows.Forms.ColumnHeader columnSelected;
		private System.Windows.Forms.Label labelCaption;
	}
}
