namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class ThreadsDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadsDialog));
			this.panelTop = new System.Windows.Forms.Panel();
			this.labelHint = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.panelInfo = new System.Windows.Forms.Panel();
			this.groupBoxColorComparisonMode = new System.Windows.Forms.GroupBox();
			this.radioButtonExact = new System.Windows.Forms.RadioButton();
			this.radioButtonVisual = new System.Windows.Forms.RadioButton();
			this.comboBoxKit = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panelLegend = new System.Windows.Forms.Panel();
			this.groupBoxLegend = new System.Windows.Forms.GroupBox();
			this.labelDifference = new System.Windows.Forms.Label();
			this.pictureBoxDifference = new System.Windows.Forms.Label();
			this.labelReplacedThread = new System.Windows.Forms.Label();
			this.labelUsedThread = new System.Windows.Forms.Label();
			this.pictureBoxReplacedThread = new System.Windows.Forms.PictureBox();
			this.pictureBoxUsedThread = new System.Windows.Forms.PictureBox();
			this.imageListThreadsUsage = new System.Windows.Forms.ImageList(this.components);
			this.paletteListView = new Ravlyk.Drawing.WinForms.PaletteListView();
			this.columnImage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnDifference = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.panelInfo.SuspendLayout();
			this.groupBoxColorComparisonMode.SuspendLayout();
			this.panelLegend.SuspendLayout();
			this.groupBoxLegend.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxReplacedThread)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUsedThread)).BeginInit();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Controls.Add(this.labelHint);
			this.panelTop.Controls.Add(this.pictureBox1);
			this.panelTop.Name = "panelTop";
			// 
			// labelHint
			// 
			resources.ApplyResources(this.labelHint, "labelHint");
			this.labelHint.Name = "labelHint";
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
			this.panelBottom.Controls.Add(this.panelButtons);
			this.panelBottom.Name = "panelBottom";
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
			// panelInfo
			// 
			resources.ApplyResources(this.panelInfo, "panelInfo");
			this.panelInfo.Controls.Add(this.groupBoxColorComparisonMode);
			this.panelInfo.Controls.Add(this.comboBoxKit);
			this.panelInfo.Controls.Add(this.label2);
			this.panelInfo.Controls.Add(this.panelLegend);
			this.panelInfo.Name = "panelInfo";
			// 
			// groupBoxColorComparisonMode
			// 
			resources.ApplyResources(this.groupBoxColorComparisonMode, "groupBoxColorComparisonMode");
			this.groupBoxColorComparisonMode.Controls.Add(this.radioButtonExact);
			this.groupBoxColorComparisonMode.Controls.Add(this.radioButtonVisual);
			this.groupBoxColorComparisonMode.Name = "groupBoxColorComparisonMode";
			this.groupBoxColorComparisonMode.TabStop = false;
			// 
			// radioButtonExact
			// 
			resources.ApplyResources(this.radioButtonExact, "radioButtonExact");
			this.radioButtonExact.Name = "radioButtonExact";
			this.radioButtonExact.UseVisualStyleBackColor = true;
			// 
			// radioButtonVisual
			// 
			resources.ApplyResources(this.radioButtonVisual, "radioButtonVisual");
			this.radioButtonVisual.Checked = true;
			this.radioButtonVisual.Name = "radioButtonVisual";
			this.radioButtonVisual.TabStop = true;
			this.radioButtonVisual.UseVisualStyleBackColor = true;
			this.radioButtonVisual.CheckedChanged += new System.EventHandler(this.radioButtonVisual_CheckedChanged);
			// 
			// comboBoxKit
			// 
			resources.ApplyResources(this.comboBoxKit, "comboBoxKit");
			this.comboBoxKit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxKit.FormattingEnabled = true;
			this.comboBoxKit.Name = "comboBoxKit";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// panelLegend
			// 
			resources.ApplyResources(this.panelLegend, "panelLegend");
			this.panelLegend.Controls.Add(this.groupBoxLegend);
			this.panelLegend.Name = "panelLegend";
			// 
			// groupBoxLegend
			// 
			resources.ApplyResources(this.groupBoxLegend, "groupBoxLegend");
			this.groupBoxLegend.Controls.Add(this.labelDifference);
			this.groupBoxLegend.Controls.Add(this.pictureBoxDifference);
			this.groupBoxLegend.Controls.Add(this.labelReplacedThread);
			this.groupBoxLegend.Controls.Add(this.labelUsedThread);
			this.groupBoxLegend.Controls.Add(this.pictureBoxReplacedThread);
			this.groupBoxLegend.Controls.Add(this.pictureBoxUsedThread);
			this.groupBoxLegend.Name = "groupBoxLegend";
			this.groupBoxLegend.TabStop = false;
			// 
			// labelDifference
			// 
			resources.ApplyResources(this.labelDifference, "labelDifference");
			this.labelDifference.Name = "labelDifference";
			// 
			// pictureBoxDifference
			// 
			resources.ApplyResources(this.pictureBoxDifference, "pictureBoxDifference");
			this.pictureBoxDifference.Name = "pictureBoxDifference";
			// 
			// labelReplacedThread
			// 
			resources.ApplyResources(this.labelReplacedThread, "labelReplacedThread");
			this.labelReplacedThread.Name = "labelReplacedThread";
			// 
			// labelUsedThread
			// 
			resources.ApplyResources(this.labelUsedThread, "labelUsedThread");
			this.labelUsedThread.Name = "labelUsedThread";
			// 
			// pictureBoxReplacedThread
			// 
			resources.ApplyResources(this.pictureBoxReplacedThread, "pictureBoxReplacedThread");
			this.pictureBoxReplacedThread.Name = "pictureBoxReplacedThread";
			this.pictureBoxReplacedThread.TabStop = false;
			// 
			// pictureBoxUsedThread
			// 
			resources.ApplyResources(this.pictureBoxUsedThread, "pictureBoxUsedThread");
			this.pictureBoxUsedThread.Name = "pictureBoxUsedThread";
			this.pictureBoxUsedThread.TabStop = false;
			// 
			// imageListThreadsUsage
			// 
			this.imageListThreadsUsage.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListThreadsUsage.ImageStream")));
			this.imageListThreadsUsage.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListThreadsUsage.Images.SetKeyName(0, "Bobbin16.png");
			this.imageListThreadsUsage.Images.SetKeyName(1, "Trash16.png");
			// 
			// paletteListView
			// 
			resources.ApplyResources(this.paletteListView, "paletteListView");
			this.paletteListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.paletteListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnImage,
            this.columnColor,
            this.columnCode,
            this.columnDifference,
            this.columnName});
			this.paletteListView.FullRowSelect = true;
			this.paletteListView.HideSelection = false;
			this.paletteListView.MultiSelect = false;
			this.paletteListView.Name = "paletteListView";
			this.paletteListView.OwnerDraw = true;
			this.paletteListView.StateImageList = this.imageListThreadsUsage;
			this.paletteListView.UseCompatibleStateImageBehavior = false;
			this.paletteListView.View = System.Windows.Forms.View.Details;
			this.paletteListView.VirtualMode = true;
			this.paletteListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.paletteListView_ColumnClick);
			this.paletteListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.paletteListView_DrawColumnHeader);
			this.paletteListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.paletteListView_DrawSubItem);
			this.paletteListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.paletteListView_RetrieveVirtualItem);
			this.paletteListView.SelectedIndexChanged += new System.EventHandler(this.paletteListView_SelectedIndexChanged);
			// 
			// columnImage
			// 
			resources.ApplyResources(this.columnImage, "columnImage");
			// 
			// columnColor
			// 
			resources.ApplyResources(this.columnColor, "columnColor");
			// 
			// columnCode
			// 
			resources.ApplyResources(this.columnCode, "columnCode");
			// 
			// columnDifference
			// 
			resources.ApplyResources(this.columnDifference, "columnDifference");
			// 
			// columnName
			// 
			resources.ApplyResources(this.columnName, "columnName");
			// 
			// ThreadsDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.paletteListView);
			this.Controls.Add(this.panelInfo);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "ThreadsDialog";
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.panelInfo.ResumeLayout(false);
			this.panelInfo.PerformLayout();
			this.groupBoxColorComparisonMode.ResumeLayout(false);
			this.groupBoxColorComparisonMode.PerformLayout();
			this.panelLegend.ResumeLayout(false);
			this.groupBoxLegend.ResumeLayout(false);
			this.groupBoxLegend.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxReplacedThread)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxUsedThread)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label labelHint;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private System.Windows.Forms.Panel panelInfo;
		private System.Windows.Forms.ComboBox comboBoxKit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panelLegend;
		private Drawing.WinForms.PaletteListView paletteListView;
		private System.Windows.Forms.GroupBox groupBoxLegend;
		private System.Windows.Forms.ColumnHeader columnImage;
		private System.Windows.Forms.ColumnHeader columnColor;
		private System.Windows.Forms.ColumnHeader columnCode;
		private System.Windows.Forms.ColumnHeader columnDifference;
		private System.Windows.Forms.ColumnHeader columnName;
		private System.Windows.Forms.ImageList imageListThreadsUsage;
		private System.Windows.Forms.Label labelDifference;
		private System.Windows.Forms.Label pictureBoxDifference;
		private System.Windows.Forms.Label labelReplacedThread;
		private System.Windows.Forms.Label labelUsedThread;
		private System.Windows.Forms.PictureBox pictureBoxReplacedThread;
		private System.Windows.Forms.PictureBox pictureBoxUsedThread;
		private System.Windows.Forms.GroupBox groupBoxColorComparisonMode;
		private System.Windows.Forms.RadioButton radioButtonExact;
		private System.Windows.Forms.RadioButton radioButtonVisual;
	}
}