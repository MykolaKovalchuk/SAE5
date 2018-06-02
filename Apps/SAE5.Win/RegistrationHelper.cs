using System;
using System.Globalization;
using System.Security;
using System.Windows.Forms;
using Microsoft.Win32;
using Ravlyk.Drawing.Tif;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;

namespace Ravlyk.SAE5.WinForms
{
	static class RegistrationHelper
	{
		public static bool IsRegistered
		{
			get
			{
				if (isRegistered.HasValue)
				{
					return isRegistered.Value;
				}

				using (var registryKey = GetRegistryKey())
				{
					if (registryKey != null)
					{
						var user = registryKey.GetValue(ItemUser) as string;
						var key = registryKey.GetValue(ItemRegKey) as string;

						isRegistered = !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(key) && TifEncoder.IsKeyCorrect(user, key) == TifEncoder.RegoStatus.Ok;
						return isRegistered.Value;
					}
				}

				isRegistered = !string.IsNullOrEmpty(Settings.Default.UserEmail) && !string.IsNullOrEmpty(Settings.Default.UserKey) && TifEncoder.IsKeyCorrect(Settings.Default.UserEmail, Settings.Default.UserKey) == TifEncoder.RegoStatus.Ok;
				return isRegistered.Value;
			}
		}

		public static string RegisteredTo
		{
			get
			{
				if (!IsRegistered)
				{
					return null;
				}

				using (var registryKey = GetRegistryKey())
				{
					return registryKey != null
						? registryKey.GetValue(ItemUser) as string
						: Settings.Default.UserEmail;
				}
			}
		}

		public static int TrialDaysLeft
		{
			get
			{
				if (IsRegistered)
				{
					return int.MaxValue;
				}

				using (var registryKey = GetRegistryKey())
				{
					string dateString;

					if (registryKey != null)
					{
						dateString = registryKey.GetValue(ItemTrialStartDate) as string;
						if (string.IsNullOrEmpty(dateString))
						{
							dateString = DateTime.Now.ToString(DateFormat, CultureInfo.CurrentCulture);
							registryKey.SetValue(ItemTrialStartDate, dateString);
						}
					}
					else
					{
						dateString = Settings.Default.TrialStartDate;
						if (string.IsNullOrEmpty(dateString))
						{
							dateString = DateTime.Now.ToString(DateFormat, CultureInfo.CurrentCulture);
							Settings.Default.TrialStartDate = dateString;
							Settings.Default.Save();
						}
					}

					DateTime date;
					if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out date))
					{
						return Math.Max(TrialDays - (DateTime.Now.Date - date.Date).Days, 0);
					}

					return 0;
				}
			}
		}

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

		public static TifEncoder.RegoStatus TryRegister(string user, string key)
		{
			var status = TifEncoder.IsKeyCorrect(user, key);
			if (status == TifEncoder.RegoStatus.Ok)
			{
				using (var registryKey = GetRegistryKey())
				{
					if (registryKey != null)
					{
						registryKey.SetValue(ItemUser, user);
						registryKey.SetValue(ItemRegKey, key);
					}
					else
					{
						Settings.Default.UserEmail = user;
						Settings.Default.UserKey = key;
						Settings.Default.Save();
					}
					isRegistered = true;
				}
			}

			return status;
		}

		static RegistryKey GetRegistryKey()
		{
			try
			{
				var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
				return baseKey.OpenSubKey(SubkeyPath, true) ?? baseKey.CreateSubKey(SubkeyPath, true);
			}
			catch (SecurityException)
			{
				return null;
			}
		}

		const string SubkeyPath = @"Software\Ravlyk.net\SAE5\Tif";
		const string ItemUser = "Contact";
		const string ItemRegKey = "Guid";
		const string ItemTrialStartDate = "Driver";
		const string DateFormat = "yyyyMMdd";

		const int TrialDays = 10;
		const int ShowRegisterDialogWhenLeftDays = 5;

		static bool? isRegistered;
	}
}
