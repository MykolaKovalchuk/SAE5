using System;
using System.Diagnostics;
using System.Xml;

using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Serialization
{
	/// <summary>
	/// Serializes <see cref="Color"/> object to and from XML.
	/// </summary>
	public static class ColorSerializer
	{
		#region XML elements constants

		internal const string XmlColorElement = "COLOR";
		const string XmlCodeElement = "CODE";
		const string XmlNameElement = "NAME";
		const string XmlSymbolElement = "SYMBOL";
		const string XmlAlphaElement = "ALPHA";
		const string XmlRedElement = "RED";
		const string XmlGreenElement = "GREEN";
		const string XmlBlueElement = "BLUE";

		#endregion

		#region WriteToXml

		/// <summary>
		/// Writes <see cref="Color"/> object to XML.
		/// </summary>
		/// <param name="color">Color object to serialize to XML.</param>
		/// <param name="writer">XML writer.</param>
		public static void WriteToXml(this CodedColor color, XmlWriter writer)
		{
			writer.WriteStartElement(XmlColorElement);

			writer.WriteSingleElement(XmlCodeElement, color.ColorCode);
			writer.WriteSingleElement(XmlNameElement, color.ColorName);
			writer.WriteSingleElement(XmlSymbolElement, color.SymbolChar);
			if (color.A > 0) writer.WriteSingleElement(XmlAlphaElement, color.A);
			writer.WriteSingleElement(XmlRedElement, color.R);
			writer.WriteSingleElement(XmlGreenElement, color.G);
			writer.WriteSingleElement(XmlBlueElement, color.B);

			writer.WriteEndElement();
		}

		#endregion

		#region ReadFromXml

		/// <summary>
		/// Reads <see cref="Color"/> object from XML.
		/// </summary>
		/// <param name="reader">XML reader.</param>
		/// <returns>New Color object.</returns>
		public static CodedColor ReadFromXml(XmlReader reader)
		{
			while (string.IsNullOrEmpty(reader.Name) && !reader.EOF)
			{
				reader.Read();
				if (reader.Name == "xml")
				{
					reader.Skip();
				}
			}
			Debug.Assert(reader.Name == XmlColorElement, "Wrong XML input for Color deserialization.");

			string colorCode = string.Empty;
			string colorName = string.Empty;
			char symbolChar = ' ';
			byte a = 0, r = 0, g = 0, b = 0;

			reader.ReadStartElement();

			while (reader.IsStartElement())
			{
				switch (reader.Name)
				{
					case XmlCodeElement:
						colorCode = reader.ReadElementContentAsString();
						break;
					case XmlNameElement:
						colorName = reader.ReadElementContentAsString();
						break;
					case XmlSymbolElement:
						symbolChar = (char)reader.ReadElementContentAsInt();
						break;
					case XmlAlphaElement:
						a = (byte)reader.ReadElementContentAsInt();
						break;
					case XmlRedElement:
						r = (byte)reader.ReadElementContentAsInt();
						break;
					case XmlGreenElement:
						g = (byte)reader.ReadElementContentAsInt();
						break;
					case XmlBlueElement:
						b = (byte)reader.ReadElementContentAsInt();
						break;
					default:
						reader.Skip();
						break;
				}
			}

			reader.ReadEndElement();

			return new CodedColor(a, r, g, b) { ColorCode = colorCode, ColorName = colorName, SymbolChar = symbolChar };
		}

		#endregion
	}
}
