namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class ThreadsManagementDialog
	{
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadsManagementDialog));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.gridViewPalettes = new System.Windows.Forms.DataGridView();
			this.ColumnPaletteName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnIsSystem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.gridViewThreads = new System.Windows.Forms.DataGridView();
			this.ribbonThreads = new System.Windows.Forms.Ribbon();
			this.ribbonTabThreads = new System.Windows.Forms.RibbonTab();
			this.ribbonPanelFile = new System.Windows.Forms.RibbonPanel();
			this.buttonSaveAll = new System.Windows.Forms.RibbonButton();
			this.buttonCancelAll = new System.Windows.Forms.RibbonButton();
			this.ribbonSeparator3 = new System.Windows.Forms.RibbonSeparator();
			this.buttonUserPalettesLocation = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelPalette = new System.Windows.Forms.RibbonPanel();
			this.buttonAddPalette = new System.Windows.Forms.RibbonButton();
			this.buttonCopyPalette = new System.Windows.Forms.RibbonButton();
			this.buttonDeletePalette = new System.Windows.Forms.RibbonButton();
			this.ribbonSeparator2 = new System.Windows.Forms.RibbonSeparator();
			this.buttonImportPalette = new System.Windows.Forms.RibbonButton();
			this.buttonExportPalette = new System.Windows.Forms.RibbonButton();
			this.ribbonPanelThreads = new System.Windows.Forms.RibbonPanel();
			this.buttonCopyThread = new System.Windows.Forms.RibbonButton();
			this.ColumnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnSelectColor = new System.Windows.Forms.DataGridViewButtonColumn();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridViewPalettes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewThreads)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.gridViewPalettes);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.gridViewThreads);
			// 
			// gridViewPalettes
			// 
			this.gridViewPalettes.AllowUserToAddRows = false;
			this.gridViewPalettes.AllowUserToDeleteRows = false;
			this.gridViewPalettes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridViewPalettes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
			this.gridViewPalettes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridViewPalettes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPaletteName,
            this.ColumnIsSystem});
			resources.ApplyResources(this.gridViewPalettes, "gridViewPalettes");
			this.gridViewPalettes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.gridViewPalettes.MultiSelect = false;
			this.gridViewPalettes.Name = "gridViewPalettes";
			this.gridViewPalettes.CurrentCellChanged += new System.EventHandler(this.gridViewPalettes_CurrentCellChanged);
			this.gridViewPalettes.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridViewPalettes_DataBindingComplete);
			// 
			// ColumnPaletteName
			// 
			this.ColumnPaletteName.DataPropertyName = "PaletteName";
			resources.ApplyResources(this.ColumnPaletteName, "ColumnPaletteName");
			this.ColumnPaletteName.Name = "ColumnPaletteName";
			// 
			// ColumnIsSystem
			// 
			this.ColumnIsSystem.DataPropertyName = "IsSystem";
			resources.ApplyResources(this.ColumnIsSystem, "ColumnIsSystem");
			this.ColumnIsSystem.Name = "ColumnIsSystem";
			this.ColumnIsSystem.ReadOnly = true;
			this.ColumnIsSystem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// gridViewThreads
			// 
			this.gridViewThreads.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridViewThreads.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
			this.gridViewThreads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridViewThreads.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCode,
            this.ColumnName,
            this.ColumnColor,
            this.ColumnSelectColor});
			resources.ApplyResources(this.gridViewThreads, "gridViewThreads");
			this.gridViewThreads.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.gridViewThreads.Name = "gridViewThreads";
			this.gridViewThreads.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridViewThreads_CellClick);
			this.gridViewThreads.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridViewThreads_CellPainting);
			// 
			// ribbonThreads
			// 
			this.ribbonThreads.BorderMode = System.Windows.Forms.RibbonWindowMode.InsideWindow;
			this.ribbonThreads.CaptionBarVisible = false;
			resources.ApplyResources(this.ribbonThreads, "ribbonThreads");
			this.ribbonThreads.Minimized = false;
			this.ribbonThreads.Name = "ribbonThreads";
			// 
			// 
			// 
			this.ribbonThreads.OrbDropDown.BorderRoundness = 8;
			this.ribbonThreads.OrbDropDown.Location = ((System.Drawing.Point)(resources.GetObject("ribbonThreads.OrbDropDown.Location")));
			this.ribbonThreads.OrbDropDown.Name = "";
			this.ribbonThreads.OrbDropDown.Size = ((System.Drawing.Size)(resources.GetObject("ribbonThreads.OrbDropDown.Size")));
			this.ribbonThreads.OrbDropDown.TabIndex = ((int)(resources.GetObject("ribbonThreads.OrbDropDown.TabIndex")));
			this.ribbonThreads.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
			this.ribbonThreads.OrbText = "Home";
			this.ribbonThreads.OrbVisible = false;
			this.ribbonThreads.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
			this.ribbonThreads.Tabs.Add(this.ribbonTabThreads);
			this.ribbonThreads.TabsMargin = new System.Windows.Forms.Padding(5, 2, 20, 0);
			this.ribbonThreads.TabSpacing = 4;
			this.ribbonThreads.ThemeColor = System.Windows.Forms.RibbonTheme.Black;
			// 
			// ribbonTabThreads
			// 
			this.ribbonTabThreads.Name = "ribbonTabThreads";
			this.ribbonTabThreads.Panels.Add(this.ribbonPanelFile);
			this.ribbonTabThreads.Panels.Add(this.ribbonPanelPalette);
			this.ribbonTabThreads.Panels.Add(this.ribbonPanelThreads);
			resources.ApplyResources(this.ribbonTabThreads, "ribbonTabThreads");
			// 
			// ribbonPanelFile
			// 
			this.ribbonPanelFile.ButtonMoreEnabled = false;
			this.ribbonPanelFile.ButtonMoreVisible = false;
			this.ribbonPanelFile.Items.Add(this.buttonSaveAll);
			this.ribbonPanelFile.Items.Add(this.buttonCancelAll);
			this.ribbonPanelFile.Items.Add(this.ribbonSeparator3);
			this.ribbonPanelFile.Items.Add(this.buttonUserPalettesLocation);
			this.ribbonPanelFile.Name = "ribbonPanelFile";
			resources.ApplyResources(this.ribbonPanelFile, "ribbonPanelFile");
			// 
			// buttonSaveAll
			// 
			this.buttonSaveAll.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Save32;
			this.buttonSaveAll.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Save32;
			this.buttonSaveAll.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonSaveAll.Name = "buttonSaveAll";
			this.buttonSaveAll.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonSaveAll.SmallImage")));
			resources.ApplyResources(this.buttonSaveAll, "buttonSaveAll");
			this.buttonSaveAll.Click += new System.EventHandler(this.buttonSaveAll_Click);
			// 
			// buttonCancelAll
			// 
			this.buttonCancelAll.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Refresh32;
			this.buttonCancelAll.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Refresh32;
			this.buttonCancelAll.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonCancelAll.Name = "buttonCancelAll";
			this.buttonCancelAll.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonCancelAll.SmallImage")));
			resources.ApplyResources(this.buttonCancelAll, "buttonCancelAll");
			this.buttonCancelAll.Click += new System.EventHandler(this.buttonCancelAll_Click);
			// 
			// ribbonSeparator3
			// 
			this.ribbonSeparator3.Name = "ribbonSeparator3";
			// 
			// buttonUserPalettesLocation
			// 
			this.buttonUserPalettesLocation.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Support32;
			this.buttonUserPalettesLocation.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Support32;
			this.buttonUserPalettesLocation.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonUserPalettesLocation.Name = "buttonUserPalettesLocation";
			this.buttonUserPalettesLocation.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonUserPalettesLocation.SmallImage")));
			resources.ApplyResources(this.buttonUserPalettesLocation, "buttonUserPalettesLocation");
			this.buttonUserPalettesLocation.Click += new System.EventHandler(this.buttonUserPalettesLocation_Click);
			// 
			// ribbonPanelPalette
			// 
			this.ribbonPanelPalette.ButtonMoreEnabled = false;
			this.ribbonPanelPalette.ButtonMoreVisible = false;
			this.ribbonPanelPalette.Items.Add(this.buttonAddPalette);
			this.ribbonPanelPalette.Items.Add(this.buttonCopyPalette);
			this.ribbonPanelPalette.Items.Add(this.buttonDeletePalette);
			this.ribbonPanelPalette.Items.Add(this.ribbonSeparator2);
			this.ribbonPanelPalette.Items.Add(this.buttonImportPalette);
			this.ribbonPanelPalette.Items.Add(this.buttonExportPalette);
			this.ribbonPanelPalette.Name = "ribbonPanelPalette";
			resources.ApplyResources(this.ribbonPanelPalette, "ribbonPanelPalette");
			// 
			// buttonAddPalette
			// 
			this.buttonAddPalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.AddFile32;
			this.buttonAddPalette.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.AddFile32;
			this.buttonAddPalette.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonAddPalette.Name = "buttonAddPalette";
			this.buttonAddPalette.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonAddPalette.SmallImage")));
			resources.ApplyResources(this.buttonAddPalette, "buttonAddPalette");
			this.buttonAddPalette.Click += new System.EventHandler(this.buttonAddPalette_Click);
			// 
			// buttonCopyPalette
			// 
			this.buttonCopyPalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Copy32;
			this.buttonCopyPalette.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Copy32;
			this.buttonCopyPalette.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonCopyPalette.Name = "buttonCopyPalette";
			this.buttonCopyPalette.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonCopyPalette.SmallImage")));
			resources.ApplyResources(this.buttonCopyPalette, "buttonCopyPalette");
			this.buttonCopyPalette.Click += new System.EventHandler(this.buttonCopyPalette_Click);
			// 
			// buttonDeletePalette
			// 
			this.buttonDeletePalette.Enabled = false;
			this.buttonDeletePalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Trash32;
			this.buttonDeletePalette.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Trash32;
			this.buttonDeletePalette.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonDeletePalette.Name = "buttonDeletePalette";
			this.buttonDeletePalette.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonDeletePalette.SmallImage")));
			resources.ApplyResources(this.buttonDeletePalette, "buttonDeletePalette");
			this.buttonDeletePalette.Click += new System.EventHandler(this.buttonDeletePalette_Click);
			// 
			// ribbonSeparator2
			// 
			this.ribbonSeparator2.Name = "ribbonSeparator2";
			// 
			// buttonImportPalette
			// 
			this.buttonImportPalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Import32;
			this.buttonImportPalette.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Import32;
			this.buttonImportPalette.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonImportPalette.Name = "buttonImportPalette";
			this.buttonImportPalette.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonImportPalette.SmallImage")));
			resources.ApplyResources(this.buttonImportPalette, "buttonImportPalette");
			this.buttonImportPalette.Click += new System.EventHandler(this.buttonImportPalette_Click);
			// 
			// buttonExportPalette
			// 
			this.buttonExportPalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Export32;
			this.buttonExportPalette.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Export32;
			this.buttonExportPalette.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonExportPalette.Name = "buttonExportPalette";
			this.buttonExportPalette.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonExportPalette.SmallImage")));
			resources.ApplyResources(this.buttonExportPalette, "buttonExportPalette");
			this.buttonExportPalette.Click += new System.EventHandler(this.buttonExportPalette_Click);
			// 
			// ribbonPanelThreads
			// 
			this.ribbonPanelThreads.ButtonMoreEnabled = false;
			this.ribbonPanelThreads.ButtonMoreVisible = false;
			this.ribbonPanelThreads.Items.Add(this.buttonCopyThread);
			this.ribbonPanelThreads.Name = "ribbonPanelThreads";
			resources.ApplyResources(this.ribbonPanelThreads, "ribbonPanelThreads");
			// 
			// buttonCopyThread
			// 
			this.buttonCopyThread.Enabled = false;
			this.buttonCopyThread.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Plus32;
			this.buttonCopyThread.LargeImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Plus32;
			this.buttonCopyThread.MinimumSize = new System.Drawing.Size(55, 0);
			this.buttonCopyThread.Name = "buttonCopyThread";
			this.buttonCopyThread.SmallImage = ((System.Drawing.Image)(resources.GetObject("buttonCopyThread.SmallImage")));
			resources.ApplyResources(this.buttonCopyThread, "buttonCopyThread");
			this.buttonCopyThread.Click += new System.EventHandler(this.buttonCopyThread_Click);
			// 
			// ColumnCode
			// 
			this.ColumnCode.DataPropertyName = "ColorCode";
			resources.ApplyResources(this.ColumnCode, "ColumnCode");
			this.ColumnCode.Name = "ColumnCode";
			// 
			// ColumnName
			// 
			this.ColumnName.DataPropertyName = "ColorName";
			resources.ApplyResources(this.ColumnName, "ColumnName");
			this.ColumnName.Name = "ColumnName";
			// 
			// ColumnColor
			// 
			this.ColumnColor.DataPropertyName = "Darkness";
			resources.ApplyResources(this.ColumnColor, "ColumnColor");
			this.ColumnColor.Name = "ColumnColor";
			this.ColumnColor.ReadOnly = true;
			// 
			// ColumnSelectColor
			// 
			resources.ApplyResources(this.ColumnSelectColor, "ColumnSelectColor");
			this.ColumnSelectColor.Name = "ColumnSelectColor";
			this.ColumnSelectColor.Text = "...";
			this.ColumnSelectColor.UseColumnTextForButtonValue = true;
			// 
			// ThreadsManagementDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.ribbonThreads);
			this.KeyPreview = true;
			this.Name = "ThreadsManagementDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThreadsManagementDialog_FormClosing);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridViewPalettes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewThreads)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Ribbon ribbonThreads;
		private System.Windows.Forms.RibbonTab ribbonTabThreads;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView gridViewThreads;
		private System.Windows.Forms.DataGridView gridViewPalettes;
		private System.Windows.Forms.RibbonPanel ribbonPanelPalette;
		private System.Windows.Forms.RibbonPanel ribbonPanelThreads;
		private System.Windows.Forms.RibbonButton buttonAddPalette;
		private System.Windows.Forms.RibbonButton buttonCopyPalette;
		private System.Windows.Forms.RibbonButton buttonDeletePalette;
		private System.Windows.Forms.RibbonSeparator ribbonSeparator2;
		private System.Windows.Forms.RibbonButton buttonImportPalette;
		private System.Windows.Forms.RibbonButton buttonExportPalette;
		private System.Windows.Forms.RibbonPanel ribbonPanelFile;
		private System.Windows.Forms.RibbonButton buttonSaveAll;
		private System.Windows.Forms.RibbonButton buttonCancelAll;
		private System.Windows.Forms.RibbonSeparator ribbonSeparator3;
		private System.Windows.Forms.RibbonButton buttonUserPalettesLocation;
		private System.Windows.Forms.RibbonButton buttonCopyThread;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPaletteName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnIsSystem;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnColor;
		private System.Windows.Forms.DataGridViewButtonColumn ColumnSelectColor;
	}
}