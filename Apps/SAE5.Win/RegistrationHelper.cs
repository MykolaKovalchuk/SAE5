using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Dialogs;

namespace Ravlyk.SAE5.WinForms
{
	static class RegistrationHelper
	{
		public static bool IsRegistered => true;

		public static string RegisteredTo => "Open Source Community";

		public static int TrialDaysLeft => TrialDays;

		public static void ShowRegisterDialog(Control parent)
		{
			using (var registerDialog = new RegisterDialog())
			{
				registerDialog.ShowDialog(parent);
			}
		}

		public static void CheckRegistration(Control parent)
		{
			if (!IsRegistered && TrialDaysLeft <= ShowRegisterDialogWhenLeftDays)
			{
				System.Media.SystemSounds.Beep.Play();
				ShowRegisterDialog(parent);
			}
		}

		public static object TryRegister(string user, string key)
		{
			return null;
		}

		const int TrialDays = 365;
		const int ShowRegisterDialogWhenLeftDays = 5;
	}
}
