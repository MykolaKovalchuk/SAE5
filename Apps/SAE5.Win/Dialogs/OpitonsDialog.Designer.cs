namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class OpitonsDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpitonsDialog));
			this.panelTop = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonReset = new Ravlyk.Drawing.WinForms.FlatButton();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControlOptinos = new System.Windows.Forms.TabControl();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkBoxCheckForUpdates = new System.Windows.Forms.CheckBox();
			this.tabPageGrid = new System.Windows.Forms.TabPage();
			this.groupBoxSelectionRectColors = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.buttonSelectionArgb1 = new Ravlyk.Drawing.WinForms.FlatButton();
			this.label9 = new System.Windows.Forms.Label();
			this.buttonSelectionArgb2 = new Ravlyk.Drawing.WinForms.FlatButton();
			this.checkBoxLine10Double = new System.Windows.Forms.CheckBox();
			this.buttonNumbersArgb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonLine5Argb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonLine10Argb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonLineArgb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageLocations = new System.Windows.Forms.TabPage();
			this.buttonSelectUserFontsLocation = new Ravlyk.Drawing.WinForms.FlatButton();
			this.textBoxUserFontsLocation = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonSelectUserPalettesLocation = new Ravlyk.Drawing.WinForms.FlatButton();
			this.textBoxUserPalettesLocation = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabControlOptinos.SuspendLayout();
			this.tabPageGeneral.SuspendLayout();
			this.tabPageGrid.SuspendLayout();
			this.groupBoxSelectionRectColors.SuspendLayout();
			this.tabPageLocations.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Controls.Add(this.label1);
			this.panelTop.Controls.Add(this.pictureBox1);
			this.panelTop.Name = "panelTop";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// panelBottom
			// 
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Controls.Add(this.buttonReset);
			this.panelBottom.Controls.Add(this.panelButtons);
			this.panelBottom.Name = "panelBottom";
			// 
			// buttonReset
			// 
			resources.ApplyResources(this.buttonReset, "buttonReset");
			this.buttonReset.BackColor = System.Drawing.Color.White;
			this.buttonReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonReset.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Refresh24;
			this.buttonReset.IsSelected = false;
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.UseVisualStyleBackColor = true;
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			// 
			// panelButtons
			// 
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Controls.Add(this.buttonOk);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.BackColor = System.Drawing.Color.White;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonOk.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Checkmark24;
			this.buttonOk.IsSelected = false;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackColor = System.Drawing.Color.White;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonCancel.IsSelected = false;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.tabControlOptinos);
			this.panel1.Name = "panel1";
			// 
			// tabControlOptinos
			// 
			resources.ApplyResources(this.tabControlOptinos, "tabControlOptinos");
			this.tabControlOptinos.Controls.Add(this.tabPageGeneral);
			this.tabControlOptinos.Controls.Add(this.tabPageGrid);
			this.tabControlOptinos.Controls.Add(this.tabPageLocations);
			this.tabControlOptinos.Name = "tabControlOptinos";
			this.tabControlOptinos.SelectedIndex = 0;
			// 
			// tabPageGeneral
			// 
			resources.ApplyResources(this.tabPageGeneral, "tabPageGeneral");
			this.tabPageGeneral.Controls.Add(this.comboBoxLanguage);
			this.tabPageGeneral.Controls.Add(this.label7);
			this.tabPageGeneral.Controls.Add(this.checkBoxCheckForUpdates);
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.UseVisualStyleBackColor = true;
			// 
			// comboBoxLanguage
			// 
			resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
			this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLanguage.FormattingEnabled = true;
			this.comboBoxLanguage.Items.AddRange(new object[] {
            resources.GetString("comboBoxLanguage.Items"),
            resources.GetString("comboBoxLanguage.Items1"),
            resources.GetString("comboBoxLanguage.Items2"),
            resources.GetString("comboBoxLanguage.Items3")});
			this.comboBoxLanguage.Name = "comboBoxLanguage";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// checkBoxCheckForUpdates
			// 
			resources.ApplyResources(this.checkBoxCheckForUpdates, "checkBoxCheckForUpdates");
			this.checkBoxCheckForUpdates.Name = "checkBoxCheckForUpdates";
			this.checkBoxCheckForUpdates.UseVisualStyleBackColor = true;
			// 
			// tabPageGrid
			// 
			resources.ApplyResources(this.tabPageGrid, "tabPageGrid");
			this.tabPageGrid.Controls.Add(this.groupBoxSelectionRectColors);
			this.tabPageGrid.Controls.Add(this.checkBoxLine10Double);
			this.tabPageGrid.Controls.Add(this.buttonNumbersArgb);
			this.tabPageGrid.Controls.Add(this.label4);
			this.tabPageGrid.Controls.Add(this.buttonLine5Argb);
			this.tabPageGrid.Controls.Add(this.label5);
			this.tabPageGrid.Controls.Add(this.buttonLine10Argb);
			this.tabPageGrid.Controls.Add(this.label3);
			this.tabPageGrid.Controls.Add(this.buttonLineArgb);
			this.tabPageGrid.Controls.Add(this.label2);
			this.tabPageGrid.Name = "tabPageGrid";
			this.tabPageGrid.UseVisualStyleBackColor = true;
			// 
			// groupBoxSelectionRectColors
			// 
			resources.ApplyResources(this.groupBoxSelectionRectColors, "groupBoxSelectionRectColors");
			this.groupBoxSelectionRectColors.Controls.Add(this.label8);
			this.groupBoxSelectionRectColors.Controls.Add(this.buttonSelectionArgb1);
			this.groupBoxSelectionRectColors.Controls.Add(this.label9);
			this.groupBoxSelectionRectColors.Controls.Add(this.buttonSelectionArgb2);
			this.groupBoxSelectionRectColors.Name = "groupBoxSelectionRectColors";
			this.groupBoxSelectionRectColors.TabStop = false;
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// buttonSelectionArgb1
			// 
			resources.ApplyResources(this.buttonSelectionArgb1, "buttonSelectionArgb1");
			this.buttonSelectionArgb1.BackColor = System.Drawing.Color.White;
			this.buttonSelectionArgb1.FlatAppearance.BorderSize = 0;
			this.buttonSelectionArgb1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonSelectionArgb1.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonSelectionArgb1.IsSelected = false;
			this.buttonSelectionArgb1.Name = "buttonSelectionArgb1";
			this.buttonSelectionArgb1.UseVisualStyleBackColor = true;
			this.buttonSelectionArgb1.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			// 
			// buttonSelectionArgb2
			// 
			resources.ApplyResources(this.buttonSelectionArgb2, "buttonSelectionArgb2");
			this.buttonSelectionArgb2.BackColor = System.Drawing.Color.White;
			this.buttonSelectionArgb2.FlatAppearance.BorderSize = 0;
			this.buttonSelectionArgb2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonSelectionArgb2.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonSelectionArgb2.IsSelected = false;
			this.buttonSelectionArgb2.Name = "buttonSelectionArgb2";
			this.buttonSelectionArgb2.UseVisualStyleBackColor = true;
			this.buttonSelectionArgb2.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// checkBoxLine10Double
			// 
			resources.ApplyResources(this.checkBoxLine10Double, "checkBoxLine10Double");
			this.checkBoxLine10Double.Checked = true;
			this.checkBoxLine10Double.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxLine10Double.Name = "checkBoxLine10Double";
			this.checkBoxLine10Double.UseVisualStyleBackColor = true;
			// 
			// buttonNumbersArgb
			// 
			resources.ApplyResources(this.buttonNumbersArgb, "buttonNumbersArgb");
			this.buttonNumbersArgb.BackColor = System.Drawing.Color.White;
			this.buttonNumbersArgb.FlatAppearance.BorderSize = 0;
			this.buttonNumbersArgb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonNumbersArgb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonNumbersArgb.IsSelected = false;
			this.buttonNumbersArgb.Name = "buttonNumbersArgb";
			this.buttonNumbersArgb.UseVisualStyleBackColor = true;
			this.buttonNumbersArgb.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// buttonLine5Argb
			// 
			resources.ApplyResources(this.buttonLine5Argb, "buttonLine5Argb");
			this.buttonLine5Argb.BackColor = System.Drawing.Color.White;
			this.buttonLine5Argb.FlatAppearance.BorderSize = 0;
			this.buttonLine5Argb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonLine5Argb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonLine5Argb.IsSelected = false;
			this.buttonLine5Argb.Name = "buttonLine5Argb";
			this.buttonLine5Argb.UseVisualStyleBackColor = true;
			this.buttonLine5Argb.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// buttonLine10Argb
			// 
			resources.ApplyResources(this.buttonLine10Argb, "buttonLine10Argb");
			this.buttonLine10Argb.BackColor = System.Drawing.Color.White;
			this.buttonLine10Argb.FlatAppearance.BorderSize = 0;
			this.buttonLine10Argb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonLine10Argb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonLine10Argb.IsSelected = false;
			this.buttonLine10Argb.Name = "buttonLine10Argb";
			this.buttonLine10Argb.UseVisualStyleBackColor = true;
			this.buttonLine10Argb.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// buttonLineArgb
			// 
			resources.ApplyResources(this.buttonLineArgb, "buttonLineArgb");
			this.buttonLineArgb.BackColor = System.Drawing.Color.White;
			this.buttonLineArgb.FlatAppearance.BorderSize = 0;
			this.buttonLineArgb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonLineArgb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Circle24;
			this.buttonLineArgb.IsSelected = false;
			this.buttonLineArgb.Name = "buttonLineArgb";
			this.buttonLineArgb.UseVisualStyleBackColor = true;
			this.buttonLineArgb.Click += new System.EventHandler(this.buttonGlidLinesColor_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// tabPageLocations
			// 
			resources.ApplyResources(this.tabPageLocations, "tabPageLocations");
			this.tabPageLocations.Controls.Add(this.buttonSelectUserFontsLocation);
			this.tabPageLocations.Controls.Add(this.textBoxUserFontsLocation);
			this.tabPageLocations.Controls.Add(this.label10);
			this.tabPageLocations.Controls.Add(this.buttonSelectUserPalettesLocation);
			this.tabPageLocations.Controls.Add(this.textBoxUserPalettesLocation);
			this.tabPageLocations.Controls.Add(this.label6);
			this.tabPageLocations.Name = "tabPageLocations";
			this.tabPageLocations.UseVisualStyleBackColor = true;
			// 
			// buttonSelectUserFontsLocation
			// 
			resources.ApplyResources(this.buttonSelectUserFontsLocation, "buttonSelectUserFontsLocation");
			this.buttonSelectUserFontsLocation.BackColor = System.Drawing.Color.White;
			this.buttonSelectUserFontsLocation.FlatAppearance.BorderSize = 0;
			this.buttonSelectUserFontsLocation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonSelectUserFontsLocation.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open16;
			this.buttonSelectUserFontsLocation.IsSelected = false;
			this.buttonSelectUserFontsLocation.Name = "buttonSelectUserFontsLocation";
			this.buttonSelectUserFontsLocation.UseVisualStyleBackColor = true;
			this.buttonSelectUserFontsLocation.Click += new System.EventHandler(this.buttonSelectUserFontsLocation_Click);
			// 
			// textBoxUserFontsLocation
			// 
			resources.ApplyResources(this.textBoxUserFontsLocation, "textBoxUserFontsLocation");
			this.textBoxUserFontsLocation.Name = "textBoxUserFontsLocation";
			this.textBoxUserFontsLocation.ReadOnly = true;
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// buttonSelectUserPalettesLocation
			// 
			resources.ApplyResources(this.buttonSelectUserPalettesLocation, "buttonSelectUserPalettesLocation");
			this.buttonSelectUserPalettesLocation.BackColor = System.Drawing.Color.White;
			this.buttonSelectUserPalettesLocation.FlatAppearance.BorderSize = 0;
			this.buttonSelectUserPalettesLocation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonSelectUserPalettesLocation.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open16;
			this.buttonSelectUserPalettesLocation.IsSelected = false;
			this.buttonSelectUserPalettesLocation.Name = "buttonSelectUserPalettesLocation";
			this.buttonSelectUserPalettesLocation.UseVisualStyleBackColor = true;
			this.buttonSelectUserPalettesLocation.Click += new System.EventHandler(this.buttonSelectUserPalettesLocation_Click);
			// 
			// textBoxUserPalettesLocation
			// 
			resources.ApplyResources(this.textBoxUserPalettesLocation, "textBoxUserPalettesLocation");
			this.textBoxUserPalettesLocation.Name = "textBoxUserPalettesLocation";
			this.textBoxUserPalettesLocation.ReadOnly = true;
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// OpitonsDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "OpitonsDialog";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabControlOptinos.ResumeLayout(false);
			this.tabPageGeneral.ResumeLayout(false);
			this.tabPageGeneral.PerformLayout();
			this.tabPageGrid.ResumeLayout(false);
			this.tabPageGrid.PerformLayout();
			this.groupBoxSelectionRectColors.ResumeLayout(false);
			this.groupBoxSelectionRectColors.PerformLayout();
			this.tabPageLocations.ResumeLayout(false);
			this.tabPageLocations.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private Drawing.WinForms.FlatButton buttonReset;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBoxLine10Double;
		private Drawing.WinForms.FlatButton buttonNumbersArgb;
		private System.Windows.Forms.Label label4;
		private Drawing.WinForms.FlatButton buttonLine5Argb;
		private System.Windows.Forms.Label label5;
		private Drawing.WinForms.FlatButton buttonLine10Argb;
		private System.Windows.Forms.Label label3;
		private Drawing.WinForms.FlatButton buttonLineArgb;
		private System.Windows.Forms.Label label2;
		private Drawing.WinForms.FlatButton buttonSelectUserPalettesLocation;
		private System.Windows.Forms.TextBox textBoxUserPalettesLocation;
		private System.Windows.Forms.Label label6;
		internal System.Windows.Forms.TabControl tabControlOptinos;
		internal System.Windows.Forms.TabPage tabPageLocations;
		internal System.Windows.Forms.TabPage tabPageGrid;
		private System.Windows.Forms.TabPage tabPageGeneral;
		private System.Windows.Forms.CheckBox checkBoxCheckForUpdates;
		private System.Windows.Forms.ComboBox comboBoxLanguage;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBoxSelectionRectColors;
		private System.Windows.Forms.Label label8;
		private Drawing.WinForms.FlatButton buttonSelectionArgb1;
		private System.Windows.Forms.Label label9;
		private Drawing.WinForms.FlatButton buttonSelectionArgb2;
		private Drawing.WinForms.FlatButton buttonSelectUserFontsLocation;
		private System.Windows.Forms.TextBox textBoxUserFontsLocation;
		private System.Windows.Forms.Label label10;
	}
}