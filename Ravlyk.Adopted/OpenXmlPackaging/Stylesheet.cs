using System;
using System.Linq;
using System.Xml.Linq;
using System.IO.Packaging;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents the Stylesheet
	/// </summary>
	public class Stylesheet
	{
		#region Private Members

		readonly Package _package;
		readonly PackagePart _workbookPart;
		readonly Dictionary<Style, Style> _styles;

		/// <summary>
		/// Gets the style XML.
		/// </summary>
		XDocument _styleXml;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Stylesheet"/> class.
		/// </summary>
		/// <param name="package">The package.</param>
		internal Stylesheet(Package package)
		{
			_package = package;
			if (_package.PartExists(Constants.WorkbookUri))
			{
				_workbookPart = package.GetPart(Constants.WorkbookUri);
			}
			else
			{
				throw new Exception("Please add a workbook before instantiating Stylesheet");
			}

			_styles = new Dictionary<Style, Style>();

			CreateStylesheet();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the style to the list if it does not exist.
		/// </summary>
		/// <param name="style">Style element.</param>
		/// <returns>Style Element that have been added or already exisits in list.</returns>
		public Style AddStyle(Style style)
		{
			Style existingStyle = null;
			if (style != null && !_styles.TryGetValue(style, out existingStyle))
			{
				existingStyle = style;
				_styles.Add(style, style);
			}
			return existingStyle;
		}

		/// <summary>
		/// Saves this Stylesheet.
		/// </summary>
		public void Save()
		{
			FormatCells();
			PackagePart worksheetPart = _package.CreatePart(Constants.StylesUri, "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");
			_workbookPart.CreateRelationship(new Uri("/xl/styles.xml", UriKind.Relative), TargetMode.Internal, Constants.RelationshipXNamespace.NamespaceName + "/styles");

			using (Stream stream = worksheetPart.GetStream(FileMode.Create, FileAccess.Write))
			{
				using (XmlWriter writer = XmlWriter.Create(stream))
				{
					_styleXml.WriteTo(writer);
				}
			}
		}

		#endregion

		#region Private and Internal Methods

		/// <summary>
		/// Adds the style.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="styleElement">The style element.</param>
		void AddStyle<T>(T styleElement) where T : XElementWriter
		{

			// Get the parent node
			var parentNode = _styleXml.Descendants(Constants.MainXNamespace + styleElement.ParentNode).FirstOrDefault() ??
				new XElement(Constants.MainXNamespace + styleElement.ParentNode);

			var element = styleElement.Element;
			// Add the style element
			parentNode.Add(element);

			if (styleElement.HasCount)
			{
				if (parentNode.Attribute("count") == null)
				{
					parentNode.Add(new XAttribute("count", "0"));
				}

				// Get the count
				var count = Int32.Parse(parentNode.Attribute("count").Value);
				count++;

				// update count value
				parentNode.Attribute("count").SetValue(count);

				// Set the index of element in the list
				styleElement.Index = count - 1;

				styleElement.PostProcess(element);
			}
		}

		/// <summary>
		/// Formats the cell.
		/// </summary>
		internal void FormatCells()
		{
			if (_styles.Count > 0)
			{
				foreach (var format in _styles.Values)
				{
					bool shouldApplyFormat = false;

					if (format.Borders != null)
					{
						AddStyle(format.Borders);
						shouldApplyFormat = true;
					}

					if (format.Fill != null)
					{
						AddStyle(format.Fill);
						shouldApplyFormat = true;
					}

					if (format.Font != null)
					{
						AddStyle(format.Font);
						shouldApplyFormat = true;
					}

					if (format.NumberFormat != null)
					{
						AddStyle(format.NumberFormat);
						shouldApplyFormat = true;
					}

					if (shouldApplyFormat)
					{
						AddStyle<Style>(format);
					}
				}
			}
		}

		/// <summary>
		/// Creates the stylesheet with basic structure.
		/// </summary>
		void CreateStylesheet()
		{
			_styleXml = new XDocument(
				new XElement(Constants.MainXNamespace + "styleSheet",
					new XElement(Constants.MainXNamespace + "numFmts", new XAttribute("count", "0")),
					new XElement(Constants.MainXNamespace + "fonts", new XAttribute("count", "1"),
						new XElement(Constants.MainXNamespace + "font")),
					new XElement(Constants.MainXNamespace + "fills", new XAttribute("count", "2"),
						new XElement(Constants.MainXNamespace + "fill",
							new XElement(Constants.MainXNamespace + "patternFill",
								new XAttribute("patternType", "none"))),
						new XElement(Constants.MainXNamespace + "fill",
							new XElement(Constants.MainXNamespace + "patternFill",
								new XAttribute("patternType", "gray125")))),
					new XElement(Constants.MainXNamespace + "borders", new XAttribute("count", "1"),
						new XElement(Constants.MainXNamespace + "border")),
					new XElement(Constants.MainXNamespace + "cellXfs", new XAttribute("count", "1"),
						new XElement(Constants.MainXNamespace + "xf",
							new XAttribute("fillId", "0"))),
					new XElement(Constants.MainXNamespace + "cellStyles", new XAttribute("count", "1"),
						new XElement(Constants.MainXNamespace + "cellStyle",
							new XAttribute("name", "Normal"),
							new XAttribute("xfId", "0")))
					));
		}

		#endregion
	}
}
