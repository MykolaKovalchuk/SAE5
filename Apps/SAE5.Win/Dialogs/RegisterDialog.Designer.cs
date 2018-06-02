namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class RegisterDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterDialog));
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonBuy = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonRegister = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
			this.panelTop = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labelTimeLeft = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxRegKey = new System.Windows.Forms.TextBox();
			this.labelRegistrationKey = new System.Windows.Forms.Label();
			this.textBoxEmail = new System.Windows.Forms.TextBox();
			this.labelEmail = new System.Windows.Forms.Label();
			this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
			this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
			this.panelBottom.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Controls.Add(this.buttonBuy);
			this.panelBottom.Controls.Add(this.panelButtons);
			this.panelBottom.Name = "panelBottom";
			// 
			// buttonBuy
			// 
			resources.ApplyResources(this.buttonBuy, "buttonBuy");
			this.buttonBuy.BackColor = System.Drawing.Color.White;
			this.buttonBuy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonBuy.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.BankCard24;
			this.buttonBuy.IsSelected = false;
			this.buttonBuy.Name = "buttonBuy";
			this.buttonBuy.UseVisualStyleBackColor = true;
			this.buttonBuy.Click += new System.EventHandler(this.buttonBuy_Click);
			// 
			// panelButtons
			// 
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Controls.Add(this.buttonRegister);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Name = "panelButtons";
			// 
			// buttonRegister
			// 
			resources.ApplyResources(this.buttonRegister, "buttonRegister");
			this.buttonRegister.BackColor = System.Drawing.Color.White;
			this.buttonRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonRegister.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Key24;
			this.buttonRegister.IsSelected = false;
			this.buttonRegister.Name = "buttonRegister";
			this.buttonRegister.UseVisualStyleBackColor = true;
			this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackColor = System.Drawing.Color.White;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonCancel.IsSelected = false;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
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
			// labelTimeLeft
			// 
			resources.ApplyResources(this.labelTimeLeft, "labelTimeLeft");
			this.labelTimeLeft.Name = "labelTimeLeft";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.textBoxRegKey);
			this.groupBox1.Controls.Add(this.labelRegistrationKey);
			this.groupBox1.Controls.Add(this.textBoxEmail);
			this.groupBox1.Controls.Add(this.labelEmail);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// textBoxRegKey
			// 
			resources.ApplyResources(this.textBoxRegKey, "textBoxRegKey");
			this.textBoxRegKey.Name = "textBoxRegKey";
			this.textBoxRegKey.TextChanged += new System.EventHandler(this.textBoxEmail_TextChanged);
			// 
			// labelRegistrationKey
			// 
			resources.ApplyResources(this.labelRegistrationKey, "labelRegistrationKey");
			this.labelRegistrationKey.Name = "labelRegistrationKey";
			// 
			// textBoxEmail
			// 
			resources.ApplyResources(this.textBoxEmail, "textBoxEmail");
			this.textBoxEmail.Name = "textBoxEmail";
			this.textBoxEmail.TextChanged += new System.EventHandler(this.textBoxEmail_TextChanged);
			// 
			// labelEmail
			// 
			resources.ApplyResources(this.labelEmail, "labelEmail");
			this.labelEmail.Name = "labelEmail";
			// 
			// buttonFeedback
			// 
			resources.ApplyResources(this.buttonFeedback, "buttonFeedback");
			this.buttonFeedback.BackColor = System.Drawing.Color.White;
			this.buttonFeedback.FlatAppearance.BorderSize = 0;
			this.buttonFeedback.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonFeedback.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Feedback24;
			this.buttonFeedback.IsSelected = false;
			this.buttonFeedback.Name = "buttonFeedback";
			this.buttonFeedback.UseVisualStyleBackColor = true;
			this.buttonFeedback.Click += new System.EventHandler(this.buttonFeedback_Click);
			// 
			// buttonWeb
			// 
			resources.ApplyResources(this.buttonWeb, "buttonWeb");
			this.buttonWeb.BackColor = System.Drawing.Color.White;
			this.buttonWeb.FlatAppearance.BorderSize = 0;
			this.buttonWeb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.buttonWeb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Web24;
			this.buttonWeb.IsSelected = false;
			this.buttonWeb.Name = "buttonWeb";
			this.buttonWeb.UseVisualStyleBackColor = true;
			this.buttonWeb.Click += new System.EventHandler(this.buttonWeb_Click);
			// 
			// RegisterDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.buttonFeedback);
			this.Controls.Add(this.buttonWeb);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelTimeLeft);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "RegisterDialog";
			this.panelBottom.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelBottom;
		private Drawing.WinForms.FlatDialogButton buttonBuy;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonRegister;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelTimeLeft;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxRegKey;
		private System.Windows.Forms.Label labelRegistrationKey;
		private System.Windows.Forms.TextBox textBoxEmail;
		private System.Windows.Forms.Label labelEmail;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
	}
}