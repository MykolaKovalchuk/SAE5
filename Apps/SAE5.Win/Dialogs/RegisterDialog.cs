using System;
using System.Diagnostics;
using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Properties;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class RegisterDialog : Form
	{
		public RegisterDialog()
		{
			InitializeComponent();

			labelTimeLeft.Text = string.Format(Resources.RegoTrialDaysLeft, RegistrationHelper.TrialDaysLeft);
			if (RegistrationHelper.TrialDaysLeft <= 0)
			{
				buttonCancel.Enabled = false;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (!RegistrationHelper.IsRegistered && RegistrationHelper.TrialDaysLeft <= 0)
			{
				Application.Exit();
			}
			else
			{
				base.OnClosed(e);
			}
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		void buttonBuy_Click(object sender, EventArgs e)
		{
			Process.Start("https://insert.shop.url");
		}

		void buttonRegister_Click(object sender, EventArgs e)
		{
			var regoStatus = RegistrationHelper.TryRegister(textBoxEmail.Text, textBoxRegKey.Text);
			if (regoStatus == null)
			{
				MessageBox.Show(Resources.RegoThankyou);
				Close();
			}
			else
			{
				MessageBox.Show(Resources.RegoWrongInfo, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void textBoxEmail_TextChanged(object sender, EventArgs e)
		{
			buttonRegister.Enabled = !string.IsNullOrWhiteSpace(textBoxEmail.Text) && !string.IsNullOrWhiteSpace(textBoxRegKey.Text);
		}

		void buttonWeb_Click(object sender, EventArgs e)
		{
			AppInfo.GoToWebsite();
		}

		void buttonFeedback_Click(object sender, EventArgs e)
		{
			AppInfo.EmailToSupport();
		}
	}
}
