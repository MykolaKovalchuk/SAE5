using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Data object for <see cref="CodedImage"/>.
	/// </summary>
	public class ImageDataObject : DataObject
	{
		/// <summary>
		/// Inintializes data object from image.
		/// </summary>
		/// <param name="image">Source image.</param>
		public ImageDataObject(CodedImage image)
		{
			image.CompletePalette();

			SetData(DataFormatType, image.WriteToString());
			SetData(DataFormats.Bitmap, image.ToBitmap());

			var csv = ToCsv(image);
			SetData(DataFormats.CommaSeparatedValue, csv);
			SetData(DataFormats.Text, csv);
		}

		/// <summary>
		/// Internal data format type.
		/// </summary>
		public static string DataFormatType => DataFormats.GetFormat(typeof(CodedImage).FullName).Name;

		static string ToCsv(CodedImage image)
		{
			const char Delimiter = '\t';

			var sb = new StringBuilder();

			for (int y = 0; y < image.Size.Height; y++)
			{
				for (int x = 0; x < image.Size.Width; x++)
				{
					if (x > 0)
					{
						sb.Append(Delimiter);
					}
					sb.Append(image[x, y].SymbolChar);
				}
				sb.AppendLine();
			}

			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine(image.Palette.Name);
			sb.Append("Symbol").Append(Delimiter).Append("Code").AppendLine();

			foreach (var color in image.Palette)
			{
				sb.Append(color.SymbolChar).Append(Delimiter).Append(color.ColorCode).AppendLine();
			}

			return sb.ToString();
		}

		/// <summary>
		/// Retrieves <see cref="CodedImage"/> from <see cref="IDataObject"/>.
		/// </summary>
		/// <param name="dataObject">DataObject to retrieve image from.</param>
		/// <returns>New CodedImage instance retrieved from DataObject if it has supported format, or null.</returns>
		public static CodedImage GetImageFromDataObject(IDataObject dataObject)
		{
			if (dataObject != null && dataObject.GetDataPresent(DataFormatType))
			{
				var data = dataObject.GetData(DataFormatType) as string;
				if (!string.IsNullOrEmpty(data))
				{
					try
					{
						return ImageSerializer.ReadFromString(data);
					}
					catch { }
				}
			}

			return null;
		}
	}
}
