using System;
using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Properties;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();

			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };
			toolTip.SetToolTip(buttonWeb, Resources.HintHomeButtonWeb);
			toolTip.SetToolTip(buttonFeedback, Resources.HintHomeButtonFeedback);

			labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion);

			if (RegistrationHelper.IsRegistered)
			{
				labelRegistrationInfo.Text = string.Format(Resources.RegoRegisteredTo, RegistrationHelper.RegisteredTo);
			}
			else
			{
				labelRegistrationInfo.Text =
					Resources.RegoTrialVersion + Environment.NewLine +
					string.Format(Resources.RegoTrialDaysLeft, RegistrationHelper.TrialDaysLeft);
			}
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
