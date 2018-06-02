using System;
using System.Diagnostics;
using System.Xml;

using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Serialization
{
	/// <summary>
	/// Serializes <see cref="Palette"/> object to and from XML
	/// </summary>
	public static class PaletteSerializer
	{
		#region XML elements constants

		internal const string XmlPaletteElement = "PALETTE";
		const string XmlNameElement = "NAME";
		const string XmlFontElement = "SYMBOLS_FONT";

		#endregion

		#region WriteToXml

		/// <summary>
		/// Writes <see cref="Palette"/> object to XML.
		/// </summary>
		/// <param name="palette">Palette object to serialize.</param>
		/// <param name="writer">XML writer.</param>
		public static void WriteToXml(this CodedPalette palette, XmlWriter writer)
		{
			writer.WriteStartElement(XmlPaletteElement);

			writer.WriteSingleElement(XmlNameElement, palette.Name);
			if (!string.IsNullOrEmpty(palette.SymbolsFont))
			{
				writer.WriteSingleElement(XmlFontElement, palette.SymbolsFont);
			}

			foreach (CodedColor color in palette)
			{
				color.WriteToXml(writer);
			}

			writer.WriteEndElement();
		}

		#endregion

		#region ReadFromXml

		/// <summary>
		/// Reads <see cref="Palette"/> object from XML.
		/// </summary>
		/// <param name="reader">XML reader.</param>
		/// <returns>New Palette object.</returns>
		public static CodedPalette ReadFromXml(XmlReader reader)
		{
			while (string.IsNullOrEmpty(reader.Name) && !reader.EOF)
			{
				reader.Read();
				if (reader.Name == "xml")
				{
					reader.Skip();
				}
			}
			Debug.Assert(reader.Name == XmlPaletteElement, "Wrong XML input for Palette deserialization.");

			CodedPalette palette = new CodedPalette();
			reader.ReadStartElement();

			while (reader.IsStartElement())
			{
				switch (reader.Name)
				{
					case XmlNameElement:
						palette.Name = reader.ReadElementContentAsString();
						break;
					case XmlFontElement:
						palette.SymbolsFont = reader.ReadElementContentAsString();
						break;
					case ColorSerializer.XmlColorElement:
						palette.Add(ColorSerializer.ReadFromXml(reader));
						break;
					default:
						reader.Skip();
						break;
				}
			}

			reader.ReadEndElement();
			return palette;
		}

		#endregion
	}
}
