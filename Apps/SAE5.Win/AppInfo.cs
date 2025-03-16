using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Properties;

namespace Ravlyk.SAE5.WinForms
{
	public static class AppInfo
	{
		public static string AppDescription
		{
			get
			{
				var assembly = Assembly.GetEntryAssembly();
				return assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Stitch Art Easy!";
			}
		}

		public static string AppVersion
		{
			get
			{
				var assembly = Assembly.GetEntryAssembly();
				var assemblyName = assembly?.GetName();
				return assemblyName?.Version.ToString() ?? "";
			}
		}

		static string AppVersionNo
		{
			get
			{
				var assembly = Assembly.GetEntryAssembly();
				var assemblyName = assembly?.GetName();
				return assemblyName?.Version.ToString();
			}
		}

		public static string StartupPath => Application.StartupPath;

		public static void GoToWebsite()
		{
			Process.Start("https://github.com/MykolaKovalchuk/SAE5");
		}

		public static void EmailToSupport()
		{
			Process.Start($"https://github.com/MykolaKovalchuk/SAE5/issues");
		}

		public static void CheckForUpdates(Action<string> updateAvailableCallback, Action<Exception> errorCallback, bool synchronous = false)
		{
			if (updateAvailableCallback == null)
			{
				return;
			}

			if (synchronous)
			{
				CheckForUpdatesCore(updateAvailableCallback, errorCallback, true);
			}
			else
			{
				new Thread(() => CheckForUpdatesCore(updateAvailableCallback, errorCallback)).Start();
			}
		}

		static void CheckForUpdatesCore(Action<string> updateAvailableCallback, Action<Exception> errorCallback, bool sayNoUpdates = false)
		{
			const string FileUrlTemplate = "https://raw.githubusercontent.com/MykolaKovalchuk/SAE5/refs/heads/master/Setup/version/winver{0}.txt";
			var fileUrlEn = string.Format(FileUrlTemplate, "");
			Action checkForUpdatesEn = () => CheckForUpdatesCore(updateAvailableCallback, errorCallback, sayNoUpdates, fileUrlEn);

			if (string.IsNullOrEmpty(Settings.Default.Locale) || Settings.Default.Locale.Equals("en", StringComparison.InvariantCultureIgnoreCase))
			{
				checkForUpdatesEn();
			}
			else
			{
				var fileUrlLocal = string.Format(FileUrlTemplate, "." + Settings.Default.Locale.ToLowerInvariant());
				CheckForUpdatesCore(updateAvailableCallback, _ => checkForUpdatesEn(), sayNoUpdates, fileUrlLocal);
			}
		}

		static void CheckForUpdatesCore(Action<string> updateAvailableCallback, Action<Exception> errorCallback, bool sayNoUpdates, string fileUrl)
		{
			try
			{
				string result = null;

				using (var webClient = new WebClient())
				using (var stream = webClient.OpenRead(fileUrl))
				using (var reader = new StreamReader(stream))
				{
					var line = reader.ReadLine();
					if (line != null && line.StartsWith("#"))
					{
						var appVersionInt = GetVersionInt(AppVersionNo);

						var newVersionInt = GetVersionInt(line);
						if (newVersionInt > appVersionInt)
						{
							var updateDetails = new StringBuilder();
							updateDetails.AppendLine(line);

							while (!reader.EndOfStream)
							{
								line = reader.ReadLine();

								if (line != null && line.StartsWith("#"))
								{
									var versionInt = GetVersionInt(line);
									if (versionInt > 0 && versionInt <= appVersionInt)
									{
										break;
									}
								}

								updateDetails.AppendLine(line?.Trim() ?? "");
							}

							result = updateDetails.ToString();
						}
					}
				}

				if (!string.IsNullOrWhiteSpace(result))
				{
					updateAvailableCallback.Invoke(result);
				}
				else if (sayNoUpdates)
				{
					MessageBox.Show(Resources.MessageNoUpdatesAvailable);
				}
			}
			catch (Exception e)
			{
				errorCallback?.Invoke(e);
			}
		}

		static long GetVersionInt(string versionNo)
		{
			var result = 0L;

			var parts = versionNo?.Trim('#').Trim().Split('.');
			if (parts != null && parts.Length > 0)
			{
				foreach (var part in parts)
				{
					long partInt;
					if (!long.TryParse(part, out partInt))
					{
						return -1L;
					}

					result = result * 1000L + partInt;
				}
			}

			return result;
		}

		public static void SetSelectedLanguage(string culture, bool showMessage)
		{
			var isChanged = Settings.Default.Locale != culture;

			Settings.Default.Culture = new CultureInfo(culture);
			Thread.CurrentThread.CurrentUICulture = Settings.Default.Culture;

			if (isChanged && showMessage)
			{
				System.Media.SystemSounds.Beep.Play();
				MessageBox.Show(Resources.InfoRestartToChangeLanguage);
			}
		}
	}
}
