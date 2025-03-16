using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE5.WinForms.Properties
{
	sealed partial class Settings
	{
		public const char FilesSeparator = ';';
		const int LastOpenFilesMaxCount = 10;

		public void AddLastOpenFile(string fileName)
		{
			var lastOpenFiles = LastOpenFiles.Split(FilesSeparator).ToList();
			if (lastOpenFiles.Contains(fileName))
			{
				lastOpenFiles.Remove(fileName);
			}
			lastOpenFiles.Insert(0, fileName);
			while (lastOpenFiles.Count > LastOpenFilesMaxCount)
			{
				lastOpenFiles.RemoveAt(lastOpenFiles.Count - 1);
			}
			LastOpenFiles = string.Join(FilesSeparator.ToString(), lastOpenFiles);
			Save();
		}

		public CultureInfo Culture
		{
			get
			{
				if (!string.IsNullOrEmpty(Locale))
				{
					try
					{
						return new CultureInfo(Locale);
					}
					catch (CultureNotFoundException) { }
				}
				return Thread.CurrentThread.CurrentUICulture;
			}
			set
			{
				if (value != null)
				{
					Locale = value.Name;
					Save();
				}
			}
		}

		public string UserPalettesLocationSafe
		{
			get
			{
				if (string.IsNullOrEmpty(UserPalettesLocation) || !Directory.Exists(UserPalettesLocation))
				{
					UserPalettesLocation = CreateUserLocationSafe("Palettes");
					Save();
				}
				return UserPalettesLocation;
			}
		}

		public string UserFontsLocationSafe
		{
			get
			{
				if (string.IsNullOrEmpty(UserFontsLocation) || !Directory.Exists(UserFontsLocation))
				{
					UserFontsLocation = CreateUserLocationSafe("Fonts");
					Save();
				}
				return UserFontsLocation;
			}
		}

		string CreateUserLocationSafe(string folderName)
		{
			var location = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stitch Art Easy"), folderName);
			try
			{
				if (!Directory.Exists(location))
				{
					Directory.CreateDirectory(location);
				}
			}
			catch
			{
				// Cannot create folder - ignore, no user files will be available till user selects new folder.
			}
			return location;
		}

		public void UpdateIfNeeded()
		{
			if (UpdateSettings)
			{
				Upgrade();
				GridPainterSettings.Default.Upgrade();
				SAEWizardSettings.Default.Upgrade();

				UpdateSettings = false;

				Save();
				GridPainterSettings.Default.Save();
				SAEWizardSettings.Default.Save();
			}
		}
	}
}
