using System;
using System.Xml;

namespace Ravlyk.SAE.Drawing.Serialization
{
	/// <summary>
	/// Base XML serialization routines.
	/// </summary>
	static class XmlSerialization
	{
		/// <summary>
		/// Writes start element, single cotent, and end element of XML node.
		/// </summary>
		/// <param name="writer">XML writer.</param>
		/// <param name="elementName">Name of XML node.</param>
		/// <param name="value">Element content value.</param>
		internal static void WriteSingleElement(this XmlWriter writer, string elementName, string value)
		{
			writer.WriteStartElement(elementName);
			writer.WriteValue(value);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Writes start element, single cotent, and end element of XML node.
		/// </summary>
		/// <param name="writer">XML writer.</param>
		/// <param name="elementName">Name of XML node.</param>
		/// <param name="value">Element content value.</param>
		internal static void WriteSingleElement(this XmlWriter writer, string elementName, int value)
		{
			writer.WriteStartElement(elementName);
			writer.WriteValue(value);
			writer.WriteEndElement();
		}
	}
}
