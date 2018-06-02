using System;
using System.Windows.Forms;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class InputDialog : Form
	{
		public InputDialog(string prompt, string caption = "")
		{
			InitializeComponent();

			labelPrompt.Text = prompt;
			Text = caption;
		}

		public string Answer => textBoxAnswer.Text;

		void textBoxAnswer_TextChanged(object sender, EventArgs e)
		{
			buttonOk.Enabled = !string.IsNullOrEmpty(textBoxAnswer.Text);
		}
	}
}
