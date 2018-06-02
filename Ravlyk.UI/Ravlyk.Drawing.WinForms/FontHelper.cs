using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using Ravlyk.Adopted.PdfSharp.Fonts;
using Ravlyk.Adopted.PdfSharp.Pdf;
using Ravlyk.SAE.Resources;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Font helper class.
	/// </summary>
	public static class FontHelper
	{
		/// <summary>
		/// Returns FontFamily from font data in resources by its name.
		/// </summary>
		/// <param name="fontName">Font name.</param>
		/// <returns>FontFamily instance.</returns>
		public static FontFamily GetFontFamily(string fontName)
		{
			FontFamily fontFamily;
			if (cachedFontFamilies.TryGetValue(fontName, out fontFamily))
			{
				return fontFamily;
			}

			var fontData = SAEResources.GetFontRawData(fontName);
			if (fontData == null)
			{
				return FontFamily.Families.FirstOrDefault(family => family.Name == fontName);
			}

			var bufferHandle = GCHandle.Alloc(fontData, GCHandleType.Pinned);
			try
			{
				var fontCollection = new PrivateFontCollection();
				fontCollection.AddMemoryFont(Marshal.UnsafeAddrOfPinnedArrayElement(fontData, 0), fontData.Length);
				fontFamily = fontCollection.Families[0];
				cachedFontFamilies.Add(fontName, fontFamily);
				return fontFamily;
			}
			finally
			{
				bufferHandle.Free();
			}
		}

		static readonly IDictionary<string, FontFamily> cachedFontFamilies = new Dictionary<string, FontFamily>();
	}

	/// <summary>
	/// Pdf font resolver.
	/// </summary>
	public class SaeFontResolver : IFontResolver
	{
		/// <summary>
		/// Setups pdf font resolver.
		/// </summary>
		public static void Setup()
		{
			GlobalFontSettings.FontResolver = new SaeFontResolver();
			GlobalFontSettings.DefaultFontEncoding = PdfFontEncoding.Unicode;
		}

		/// <summary>
		/// Returns font data for specified name.
		/// </summary>
		/// <param name="faceName">Font name.</param>
		/// <returns>Byte array with font file data.</returns>
		public byte[] GetFont(string faceName)
		{
			if (SAEResources.GetAllFontNames().Any(s => s.Equals(faceName, StringComparison.OrdinalIgnoreCase)))
			{
				return SAEResources.GetFontRawData(faceName);
			}
			throw new ArgumentException($"Cannot resolve font with faceName '{faceName}'.");
		}

		/// <summary>
		/// Returns font info for specified font family name.
		/// </summary>
		/// <param name="familyName">Font family name.</param>
		/// <param name="isBold">Specifies if font should be bold.</param>
		/// <param name="isItalic">Specifies if font should be italic.</param>
		/// <returns>Font info with font file name specified family name and font options.</returns>
		public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
		{
			if (SAEResources.GetAllFontNames().Any(s => s.Equals(familyName, StringComparison.OrdinalIgnoreCase)))
			{
				return new FontResolverInfo(familyName);
			}
			throw new ArgumentException($"Cannot resolve font with familyName '{familyName}'.");
		}
	}
}
