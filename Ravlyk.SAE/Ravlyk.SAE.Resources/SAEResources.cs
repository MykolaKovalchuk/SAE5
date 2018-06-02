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
		public static IEnumerable<TrueTypeFont> GetAllFonts()
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var name in assembly.GetManifestResourceNames())
			{
				if (name.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase))
				{
					using (var stream = assembly.GetManifestResourceStream(name))
					{
						if (stream != null)
						{
							var buffer = new byte[stream.Length];
							stream.Read(buffer, 0, (int)stream.Length);
							yield return new TrueTypeFont(CutOffPathAndExtension(name), buffer, 0);
						}
					}
				}
			}
		}

		public static IEnumerable<string> GetAllFontNames()
		{
			var assembly = Assembly.GetExecutingAssembly();
			return assembly.GetManifestResourceNames().Where(name => name.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)).Select(CutOffPathAndExtension);
		}

		public static byte[] GetFontRawData(string fontName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var name in assembly.GetManifestResourceNames())
			{
				if (name.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) && fontName.Equals(CutOffPathAndExtension(name), StringComparison.OrdinalIgnoreCase))
				{
					using (var stream = assembly.GetManifestResourceStream(name))
					{
						if (stream != null)
						{
							var buffer = new byte[stream.Length];
							stream.Read(buffer, 0, (int)stream.Length);
							return buffer;
						}
					}
				}
			}

			return null;
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

			if (!String.IsNullOrEmpty(additionalPalettesPath) && Directory.Exists(additionalPalettesPath))
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

		public static TrueTypeFont GetImageFont(CodedImage image)
		{
			var fontName = image.Palette.SymbolsFont;
			if (fontName == "Znaky SAE")
			{
				fontName = "ZnakySAE";
			}
			return GetAllFonts().FirstOrDefault(font => font.Name == fontName) ?? GetAllFonts().First();
		}

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
	}
}
