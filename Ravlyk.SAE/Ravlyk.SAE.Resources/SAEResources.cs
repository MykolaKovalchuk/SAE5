using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;

namespace Ravlyk.SAE.Resources
{
	public static class SAEResources
	{
		#region Fonts

		public static IEnumerable<TrueTypeFont> GetAllFonts(string additionalFontsPath)
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var resourceName in GetAllFontsResourceNames())
			{
				var fontRawData = GetFontRawDataFromResource(assembly, resourceName);
				if (fontRawData != null)
				{
					yield return new TrueTypeFont(CutOffPathAndExtension(resourceName), fontRawData, 0);
				}
			}

			if (!string.IsNullOrEmpty(additionalFontsPath) && Directory.Exists(additionalFontsPath))
			{
				foreach (var fileName in GetAllFontFilesName(additionalFontsPath))
				{
					var fontRawData = GetFontRawDataFromFile(fileName);
					if (fontRawData != null)
					{
						TrueTypeFont font = null;

						try
						{
							font = new TrueTypeFont(CutOffPathAndExtension(fileName), fontRawData, 0);
						}
						catch
						{
							// Skip any file with faulty data
						}

						if (font != null)
						{
							yield return font;
						}
					}
				}
			}
		}

		public static byte[] GetFontRawData(string fontName, string additionalFontsPath)
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var resourceName in GetAllFontsResourceNames())
			{
				if (fontName.Equals(CutOffPathAndExtension(resourceName), StringComparison.OrdinalIgnoreCase))
				{
					return GetFontRawDataFromResource(assembly, resourceName);
				}
			}

			if (!string.IsNullOrEmpty(additionalFontsPath) && Directory.Exists(additionalFontsPath))
			{
				foreach (var fileName in GetAllFontFilesName(additionalFontsPath))
				{
					if (fontName.Equals(CutOffPathAndExtension(fileName), StringComparison.OrdinalIgnoreCase))
					{
						return GetFontRawDataFromFile(fileName);
					}
				}
			}

			return null;
		}

		static byte[] GetFontRawDataFromResource(Assembly assembly, string resourceName)
		{
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream != null)
				{
					var buffer = new byte[stream.Length];
					stream.Read(buffer, 0, (int)stream.Length);
					return buffer;
				}
			}

			return null;
		}

		static byte[] GetFontRawDataFromFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				try
				{
					using (var stream = new FileStream(fileName, FileMode.Open))
					{
						var buffer = new byte[stream.Length];
						stream.Read(buffer, 0, (int)stream.Length);
						return buffer;
					}
				}
				catch
				{
					// Skip any file with faulty data
				}
			}

			return null;
		}

		public static IEnumerable<string> GetAllFontNames(string additionalFontsPath)
		{
			return GetAllFontsResourceNames()
				.Concat(GetAllFontFilesName(additionalFontsPath))
				.Select(CutOffPathAndExtension);
		}

		static IEnumerable<string> GetAllFontsResourceNames()
		{
			var assembly = Assembly.GetExecutingAssembly();
			return assembly.GetManifestResourceNames().Where(name => name.EndsWith(FontFileExtension, StringComparison.OrdinalIgnoreCase));
		}

		static IEnumerable<string> GetAllFontFilesName(string additionalFontsPath)
		{
			if (!String.IsNullOrEmpty(additionalFontsPath) && Directory.Exists(additionalFontsPath))
			{
				return Directory.GetFiles(additionalFontsPath, "*" + FontFileExtension, SearchOption.TopDirectoryOnly);
			}

			return Enumerable.Empty<string>();
		}

		static string CutOffPathAndExtension(string fullName)
		{
			var result = Path.GetFileNameWithoutExtension(fullName); // BUG: May have problems on MacOS, iOS, maybe Android

			var ns = typeof(SAEResources).Namespace;
			if (result.StartsWith(ns))
			{
				result = result.Substring(ns.Length);
			}

			result = result.Trim('.');
			var nextDot = result.IndexOf('.');
			if (nextDot >= 0)
			{
				result = result.Substring(nextDot);
			}
			result = result.Trim('.');

			return result;
		}

		public static TrueTypeFont GetImageFont(CodedImage image, string additionalFontsPath)
		{
			var fontName = image.Palette.SymbolsFont;
			if (fontName.Equals("Znaky SAE", StringComparison.OrdinalIgnoreCase))
			{
				fontName = "ZnakySAE";
			}

			return GetAllFonts(additionalFontsPath).FirstOrDefault(font => font.Name == fontName) ?? GetAllFonts(additionalFontsPath).First();
		}

		#endregion

		#region Palettes

		public static IEnumerable<CodedPalette> GetAllPalettes(string additionalPalettesPath)
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var name in assembly.GetManifestResourceNames())
			{
				if (name.EndsWith(ThreadFileExtension, StringComparison.OrdinalIgnoreCase))
				{
					using (var stream = assembly.GetManifestResourceStream(name))
					{
						if (stream != null)
						{
							using (var reader = XmlReader.Create(stream))
							{
								var palette = PaletteSerializer.ReadFromXml(reader);
								palette.IsSystem = true;
								yield return palette;
							}
						}
					}
				}
			}

			if (!string.IsNullOrEmpty(additionalPalettesPath) && Directory.Exists(additionalPalettesPath))
			{
				foreach (var fileName in Directory.GetFiles(additionalPalettesPath, "*" + ThreadFileExtension, SearchOption.TopDirectoryOnly))
				{
					CodedPalette palette = null;
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Open))
						using (var reader = XmlReader.Create(stream))
						{
							palette = PaletteSerializer.ReadFromXml(reader);
							palette.IsSystem = false;
							palette.FileName = fileName;
						}
					}
					catch
					{
						// Skip any file with faulty data
					}
					if (palette != null)
					{
						yield return palette;
					}
				}
			}
		}

		#endregion

		public static IndexedImage GetCrossImage()
		{
			return GetImageResource("cross.sa4");
		}

		public static IndexedImage GetImageResource(string resourceName)
		{
			using (var stream = GetRawResourceStream(resourceName))
			{
				if (stream != null)
				{
					return ImageSerializer.LoadFromStream(stream);
				}
			}
			return null;
		}

		public static Stream GetRawResourceStream(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var name in assembly.GetManifestResourceNames())
			{
				if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
				{
					return assembly.GetManifestResourceStream(name);
				}
			}
			return null;
		}

		public const string ThreadFileExtension = ".thread";
		public const string FontFileExtension = ".ttf";
	}
}
