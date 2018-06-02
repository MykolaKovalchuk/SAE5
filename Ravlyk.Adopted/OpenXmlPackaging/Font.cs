using System.Drawing;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents a <font> element in Stylesheet
	/// </summary>
	public class Font : XElementWriter
	{

		#region Public Properties

		public string Name { get; set; }

		public decimal Size { get; set; }

		public FontStyles Style { get; set; }

		public Color Color { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Font"/> class with
		/// Name  = Calibri,
		/// Size  = 11,
		/// Style = FontStyles.Regular,
		/// Color = Color.Black
		/// </summary>
		public Font()
		{
			Name = "Calibri";
			Size = 11;
			Style = FontStyles.Regular;
			Color = Color.Black;
		}

		#endregion

		#region XElementWriter Members

		internal override string ParentNode => "fonts";

		internal override XElement Element
		{
			get
			{

				var element = new XElement(Constants.MainXNamespace + "font",
					new XElement(Constants.MainXNamespace + "name", new XAttribute("val", Name)),
					new XElement(Constants.MainXNamespace + "sz", new XAttribute("val", Size)),
					new XElement(Constants.MainXNamespace + "color", new XAttribute("rgb", Utilities.GetColorInHex(Color))));

				AddFontStyles(element);

				return element;
			}
		}

		#endregion

		#region Private Methods

		void AddFontStyles(XContainer element)
		{
			if (Style != FontStyles.Regular)
			{
				if (Style.HasFlag(FontStyles.Bold))
				{
					element.Add(new XElement(Constants.MainXNamespace + "b"));
				}
				if (Style.HasFlag(FontStyles.Italic))
				{
					element.Add(new XElement(Constants.MainXNamespace + "i"));
				}
				if (Style.HasFlag(FontStyles.Strikeout))
				{
					element.Add(new XElement(Constants.MainXNamespace + "strike"));
				}
				if (Style.HasFlag(FontStyles.Underline))
				{
					element.Add(new XElement(Constants.MainXNamespace + "u"));
				}
				if (Style.HasFlag(FontStyles.DoubleUnderline))
				{
					element.Add(new XElement(Constants.MainXNamespace + "u", new XAttribute("val", "double")));
				}
				if (Style.HasFlag(FontStyles.Superscript))
				{
					element.Add(new XElement(Constants.MainXNamespace + "vertAlign", new XAttribute("val", "superscript")));
				}
				if (Style.HasFlag(FontStyles.Subscript))
				{
					element.Add(new XElement(Constants.MainXNamespace + "vertAlign", new XAttribute("val", "subscript")));
				}
			}
		}

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return Name?.GetHashCode() ?? 0
				^ Size.GetHashCode()
				^ Style.GetHashCode()
				^ Color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var fontO = obj as Font;
			return fontO != null
				&& Name == fontO.Name
				&& Size == fontO.Size
				&& Style == fontO.Style
				&& Color == fontO.Color;
		}

		#endregion
	}
}
