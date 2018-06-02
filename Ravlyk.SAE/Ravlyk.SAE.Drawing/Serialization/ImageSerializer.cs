using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing.Serialization
{
	/// <summary>
	/// Serializes <see cref="IndexedImage"/> object to and from XML.
	/// </summary>
	public static class ImageSerializer
	{
		#region XML elements constants

		const string XmlImageElement = "IMAGE";
		const string XmlSizeElement = "SIZE";
		const string XmlWidthElement = "Width";
		const string XmlHeightElement = "Height";
		const string XmlPixelsElement = "PIXELS";
		const string XmlSourceImageFileName = "SOURCEFILE";
		const string XmlAdditionalDataCollection = "ADDITIONALDATA";
		const string XmlAdditionalDataElement = "DATA";
		const string XmlAdditionalDataElementName = "NAME";
		const string XmlAdditionalDataElementValue = "VALUE";

		#endregion

		#region WriteToXml

		/// <summary>
		/// Writes <see cref="IndexedImage"/> object to XML.
		/// </summary>
		/// <param name="image">Image object to serialize.</param>
		/// <param name="writer">XML writer.</param>
		public static void WriteToXml(this CodedImage image, XmlWriter writer)
		{
			writer.WriteStartElement(XmlImageElement);

			image.Palette.WriteToXml(writer);

			writer.WriteStartElement(XmlSizeElement);
			writer.WriteSingleElement(XmlWidthElement, image.Size.Width);
			writer.WriteSingleElement(XmlHeightElement, image.Size.Height);
			writer.WriteEndElement();

			writer.WriteSingleElement(XmlPixelsElement, PixelsToString(image));

			writer.WriteSingleElement(XmlSourceImageFileName, image.SourceImageFileName);
			WriteAdditionalDataToXml(image, writer);

			writer.WriteEndElement();
		}

		static string PixelsToString(CodedImage image)
		{
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				StringBuilder sb = new StringBuilder(pixels.Length * 8);
				for (int i = 0; i < pixels.Length; i++)
				{
					sb.Append(pixels[i].ToString("X8"));
				}
				return sb.ToString();
			}
		}

		static void WriteAdditionalDataToXml(CodedImage image, XmlWriter writer)
		{
			if (image.AdditionalData.Count > 0)
			{
				writer.WriteStartElement(XmlAdditionalDataCollection);

				foreach (var dataPair in image.AdditionalData)
				{
					writer.WriteStartElement(XmlAdditionalDataElement);
					writer.WriteSingleElement(XmlAdditionalDataElementName, dataPair.Key);
					writer.WriteSingleElement(XmlAdditionalDataElementValue, dataPair.Value);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}

		#endregion

		#region ReadFromXml

		/// <summary>
		/// Reads <see cref="IndexedImage"/> object from Xml.
		/// </summary>
		/// <param name="reader">Xml reader.</param>
		/// <returns>New Image object.</returns>
		public static CodedImage ReadFromXml(XmlReader reader)
		{
			while (string.IsNullOrEmpty(reader.Name) && !reader.EOF)
			{
				reader.Read();
				if (reader.Name == "xml")
				{
					reader.Skip();
				}
			}
			Debug.Assert(reader.Name == XmlImageElement, "Wrong XML input for IndexedImage deserialization.");

			CodedImage image = new CodedImage();
			reader.ReadStartElement();

			while (reader.IsStartElement())
			{
				switch (reader.Name)
				{
					case PaletteSerializer.XmlPaletteElement:
						image.Palette = PaletteSerializer.ReadFromXml(reader);
						break;
					case XmlSizeElement:
						reader.ReadStartElement();
						Debug.Assert(reader.Name == XmlWidthElement);
						int width = reader.ReadElementContentAsInt();
						Debug.Assert(reader.Name == XmlHeightElement);
						int height = reader.ReadElementContentAsInt();
						reader.ReadEndElement();
						image.Size = new Size(width, height);
						break;
					case XmlPixelsElement:
						string pixelsString = reader.ReadElementContentAsString();
						SetPixelsFromString(image, pixelsString);
						break;
					case XmlSourceImageFileName:
						image.SourceImageFileName = reader.ReadElementContentAsString();
						break;
					case XmlAdditionalDataCollection:
						ReadAdditionalDataFromXml(image, reader);
						break;
					default:
						reader.Skip();
						break;
				}
			}

			reader.ReadEndElement();
			return image;
		}

		static void SetPixelsFromString(IndexedImage image, string pixelsString)
		{
			if (image.Size.Width * image.Size.Height < pixelsString.Length / 8)
			{
				image.Size = new Size(1, pixelsString.Length / 8);
			}

			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				for (int i = 0; i < pixelsString.Length / 8; i++)
				{
					pixels[i] = int.Parse(pixelsString.Substring(i * 8, 8), NumberStyles.AllowHexSpecifier);
				}
			}
		}

		static void ReadAdditionalDataFromXml(CodedImage image, XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.IsStartElement())
			{
				switch (reader.Name)
				{
					case XmlAdditionalDataElement:
						reader.ReadStartElement();
						Debug.Assert(reader.Name == XmlAdditionalDataElementName);
						var dataName = reader.ReadElementContentAsString();
						Debug.Assert(reader.Name == XmlAdditionalDataElementValue);
						var dataValue = reader.ReadElementContentAsString();
						reader.ReadEndElement();
						image.AdditionalData[dataName] = dataValue;
						break;
					default:
						reader.Skip();
						break;
				}
			}
			reader.ReadEndElement();
		}

		#endregion

		#region SaveToStream

		/// <summary>
		/// Saves <see cref="IndexedImage"/> object to stream.
		/// </summary>
		/// <param name="image">Image object to save.</param>
		/// <param name="stream">The destination stream.</param>
		public static void SaveToStream(this CodedImage image, Stream stream)
		{
			using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress))
			using (XmlWriter xmlWriter = XmlWriter.Create(zipStream))
			{
				image.WriteToXml(xmlWriter);
			}
		}

		#endregion

		#region LoadFromStream

		/// <summary>
		/// Creates new <see cref="Ravlyk.Drawing.IndexedImage"/> objects by loading it from stream.
		/// </summary>
		/// <param name="stream">The source stream.</param>
		/// <returns>New Image object.</returns>
		public static CodedImage LoadFromStream(Stream stream)
		{
			CodedImage indexedImage;
			using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
			using (XmlReader xmlReader = XmlReader.Create(zipStream))
			{
				indexedImage = ReadFromXml(xmlReader);
			}
			return indexedImage;
		}

		#endregion

		#region WriteToString

		/// <summary>
		/// Writes <see cref="IndexedImage"/> object to string.
		/// </summary>
		/// <param name="image">Image object to serialize.</param>
		/// <returns>String containing serialized image.</returns>
		public static string WriteToString(this CodedImage image)
		{
			using (var stringWriter = new StringWriter())
			{
				using (var xmlWriter = XmlWriter.Create(stringWriter))
				{
					image.WriteToXml(xmlWriter);
				}
				stringWriter.Flush();
				return stringWriter.ToString();
			}
		}

		#endregion

		#region ReadFromString

		/// <summary>
		/// Reads <see cref="IndexedImage"/> object from string.
		/// </summary>
		/// <param name="imageData">String with image data.</param>
		/// <returns>New Image object.</returns>
		public static CodedImage ReadFromString(string imageData)
		{
			using (var stringReader = new StringReader(imageData))
			using (var xmlReader = XmlReader.Create(stringReader))
			{
				return ReadFromXml(xmlReader);
			}
		}

		#endregion
	}
}
