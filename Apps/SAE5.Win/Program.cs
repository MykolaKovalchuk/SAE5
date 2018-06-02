using System;
using System.Threading;
using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Properties;

namespace SAE5.Win
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] parameters)
		{
			if (parameters.Length > 0)
			{
				MainForm.StartupParameter = parameters[0];
			}

			Settings.Default.UpdateIfNeeded();

			var culture = Settings.Default.Culture;
			if (culture != null && !culture.Equals(Thread.CurrentThread.CurrentUICulture))
			{
				Thread.CurrentThread.CurrentUICulture = culture;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
