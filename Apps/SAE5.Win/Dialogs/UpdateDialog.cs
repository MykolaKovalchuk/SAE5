using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class UpdateDialog : Form
	{
		public UpdateDialog(string details)
		{
			InitializeComponent();
			textBoxDetails.Text = details;
			textBoxDetails.SelectionLength = 0;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			textBoxDetails.SelectionStart = 0;
			textBoxDetails.SelectionLength = 0;
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		void buttonDownload_Click(object sender, EventArgs e)
		{
			OpenDownloadPage();
			Close();
		}

		void OpenDownloadPage()
		{
			Process.Start("http://stitcharteasy.com/download.aspx");
		}
	}
}
